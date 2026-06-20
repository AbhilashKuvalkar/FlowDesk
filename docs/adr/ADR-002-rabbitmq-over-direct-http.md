# ADR-002: RabbitMQ over Direct HTTP for Asynchronous Notifications

## Status

Accepted

## Date

2025-06-20

## Context

When a ticket is assigned in FlowDesk, the assigned agent must receive an
email notification. The most straightforward implementation is to inject an
email service directly into `AssignTicketCommandHandler` and call it
synchronously after saving the ticket:

```csharp
// naive approach — what we rejected
public async Task Handle(AssignTicketCommand request, CancellationToken ct)
{
    ticket.AssignTo(agent);
    await _unitOfWork.SaveChangesAsync(ct);
    await _emailService.SendAsync(agent.Email, "You have been assigned a ticket");
}
```

This approach has three serious problems in a distributed production system.

**Tight coupling to email infrastructure.** The Ticket Service now depends on
the email provider's availability. If SendGrid is slow or has an outage, ticket
assignment slows down or fails entirely. A core business operation fails because
a notification provider is unavailable.

**No retry on failure.** If the email call throws an exception after
`SaveChangesAsync` succeeds, the exception propagates up through the handler.
Depending on where the exception is caught, the ticket assignment may be rolled
back — the ticket was not assigned because an email failed. That is the wrong
failure mode.

**Blocked thread under load.** Every ticket assignment blocks the request thread
waiting for the email provider to respond. Under high load this becomes a
bottleneck — email latency directly limits ticket assignment throughput.

In addition to the notification concern, future requirements will add more
consequences to ticket assignment: SLA timer starts, audit log entry created,
billing event emitted. Each new consequence added directly to the handler grows
it into a God class with multiple unrelated responsibilities.

## Decision

Decouple ticket assignment from notification delivery using **RabbitMQ as the
message broker** and **MassTransit as the .NET abstraction layer**.

The flow works as follows:

1. `AssignTicketCommandHandler` calls `ticket.AssignTo(agent)` and saves.
2. The `Ticket` entity raises a `TicketAssignedEvent` domain event.
3. `AppDbContext.SaveChangesAsync` dispatches domain events via MediatR after
   the database commit succeeds.
4. `TicketAssignedEventHandler` translates the domain event into a
   `TicketAssignedMessage` contract and publishes it to RabbitMQ via
   `IPublishEndpoint`.
5. `TicketAssignedConsumer` in `FlowDesk.NotificationService` receives the
   message independently and sends the email.

The Ticket Service publishes and forgets. The Notification Service consumes
at its own pace, retries on failure, and scales independently.

### MassTransit retry policy

```csharp
x.AddConsumer<TicketAssignedConsumer>(cfg =>
{
    cfg.UseMessageRetry(r =>
    {
        r.Incremental(
            retryLimit: 3,
            initialInterval: TimeSpan.FromSeconds(1),
            intervalIncrement: TimeSpan.FromSeconds(2));
    });
});
```

After 3 failed retries, the message moves to a dead-letter queue
(`ticket-assigned_error`) where it can be inspected and replayed manually.

### Message contract separation

`TicketAssignedEvent` (domain event) and `TicketAssignedMessage` (integration
event) are deliberately kept as separate types:

- Domain events are internal — they carry domain objects and evolve freely
  as business rules change.
- Message contracts cross service boundaries — they must contain only primitive
  types, be stable, and be versioned carefully. A change to a message contract
  is a breaking change for every consumer.

The `TicketAssignedEventHandler` is the explicit translation layer between
the internal domain model and the external message contract.

## Consequences

### Positive

- Ticket assignment never fails because an email provider is down — the two
  operations are fully decoupled.
- MassTransit retry policy handles transient notification failures automatically
  with incremental backoff — no retry logic in application code.
- Dead-letter queue captures exhausted messages for manual inspection and replay.
  No silent data loss.
- The Notification Service scales independently from the Ticket Service based
  on message volume.
- New consequences (audit logger, SLA timer, billing event) are added by
  registering a new `INotificationHandler<TicketAssignedEvent>`. Zero changes
  to existing handlers or the domain entity. This is the Open/Closed Principle
  applied at the integration level.
- The Ticket Service has no compile-time dependency on any notification
  infrastructure.

### Negative

- **Eventual consistency.** The agent notification arrives seconds after
  assignment, not milliseconds. Acceptable for email but must be communicated
  to product stakeholders.
- **Operational overhead.** Running and monitoring a RabbitMQ broker adds
  infrastructure complexity. Mitigated by Docker Compose for local development
  and managed Azure Service Bus as a production alternative.
- **Debugging complexity.** Tracing a failure across async boundaries requires
  distributed tracing (Application Insights, correlation IDs). Harder than
  stepping through a synchronous call.
- **Guaranteed delivery requires the outbox pattern.** If the application
  crashes after `SaveChangesAsync` but before `IPublishEndpoint.Publish`,
  the message is lost. The transactional outbox pattern (MassTransit
  `AddEntityFrameworkOutbox`) mitigates this.

## Alternatives Considered

### Direct HTTP call from the handler

Rejected. Tight coupling — the Ticket Service fails when the Notification
Service is down. No retry mechanism. No dead-lettering. Scales poorly under
load.

### Azure Service Bus

Viable and production-appropriate for Azure-only deployments. Rejected for
FlowDesk in favour of RabbitMQ because RabbitMQ runs locally in Docker with
zero Azure dependency, reducing developer friction. The MassTransit abstraction
makes switching transport providers a configuration change.

### Kafka

Overkill for notification volume. Kafka's strengths — high throughput, log
compaction, consumer group replay — are not needed here. Kafka is already
present in the broader FlowDesk platform for high-volume event streams. Adding
it for notifications would add operational overhead for no benefit.

### Synchronous REST call to Notification Service

Rejected. Same tight coupling problem as direct email injection. If the
Notification Service is down or slow, ticket assignment degrades with it.
No retry, no dead-lettering, no independent scalability.

## References

- MassTransit documentation: https://masstransit.io/documentation
- Outbox pattern: https://masstransit.io/documentation/patterns/transactional-outbox
- FlowDesk.Contracts project: shared message contract definitions
- Lesson 5: RabbitMQ and MassTransit implementation notes
