# ADR-004: gRPC for Internal Service-to-Service Communication

## Status

Accepted

## Date

2025-06-20

## Context

The Ticket Service needs SLA policy data from the SLA Service on every ticket
operation. This includes ticket creation (initial SLA threshold check), ticket
assignment (response window verification), and the agent dashboard (SLA breach
status for every open ticket).

This is a **synchronous, high-frequency, request-response** interaction — the
Ticket Service cannot proceed with a ticket operation until it has received the
SLA policy for that ticket's priority. The SLA policy determines whether the
operation is within compliance bounds.

Three communication options were evaluated:

1. **REST over HTTP/JSON** — the default choice for service-to-service calls
2. **Asynchronous messaging via RabbitMQ** — already present in the platform
3. **gRPC over HTTP/2 with Protobuf serialisation** — binary, contract-first

The choice affects latency, type safety, payload size, and developer experience
for every ticket operation in the system.

## Decision

Use **gRPC with Protobuf serialisation** for the Ticket Service to SLA Service
internal communication.

### Contract definition

The API contract is defined in a `.proto` file at the solution root, shared
between both services:

```protobuf
syntax = "proto3";
option csharp_namespace = "FlowDesk.Grpc";
package sla;

service SlaService {
  rpc GetSlaPolicyByPriority (GetSlaPolicyRequest) returns (GetSlaPolicyResponse);
}

message GetSlaPolicyRequest {
  string tenant_id = 1;
  string priority  = 2;
}

message GetSlaPolicyResponse {
  string policy_id               = 1;
  string name                    = 2;
  string priority                = 3;
  int32  response_time_minutes   = 4;
  int32  resolution_time_minutes = 5;
  bool   found                   = 6;
}
```

**Field number permanence.** Field numbers identify fields in the binary wire
format — not field names. Once deployed, field numbers must never be reused for
a different field type. Reuse causes silent data corruption or deserialisation
exceptions in clients running an older version of the contract.

**The `found` flag pattern.** Protobuf messages cannot be null on the wire.
Rather than throwing `StatusCode.NotFound` for an expected business case
(missing SLA policy), the response includes a `bool found` field. This avoids
exception-based flow control on the client side.

### Code generation

`Grpc.Tools` generates strongly typed C# from the `.proto` file at build time:

```xml
<!-- SLA Service .csproj -->
<Protobuf Include="..\Protos\sla.proto" GrpcServices="Server" />

<!-- Ticket Service .csproj -->
<Protobuf Include="..\Protos\sla.proto" GrpcServices="Client" />
```

The SLA Service gets `SlaService.SlaServiceBase` to implement. The Ticket
Service gets `SlaService.SlaServiceClient` to call. Both get the message
classes. Schema drift between services is a compile error — the `.proto` file
is the single source of truth.

### Abstraction layer

The generated `SlaService.SlaServiceClient` is never injected into application
handlers directly. An `ISlaServiceClient` interface is defined in the Domain
project and implemented in the Ticket Service:

```csharp
// Domain/Services/ISlaServiceClient.cs — no gRPC reference
public interface ISlaServiceClient
{
    Task<SlaPolicyDto?> GetSlaPolicyAsync(
        TicketPriority priority,
        Guid tenantId,
        CancellationToken cancellationToken);
}
```

`SlaServiceClient` (implementation) translates between domain types and
Protobuf types. Handlers see only `SlaPolicyDto` — a clean domain record.
The gRPC transport is entirely behind the interface.

### Client registration

```csharp
builder.Services.AddGrpcClient<SlaService.SlaServiceClient>(options =>
{
    options.Address = new Uri(
        builder.Configuration["GrpcServices:SlaService"]!);
});
```

`AddGrpcClient` uses `IHttpClientFactory` for channel lifecycle management —
connection pooling and periodic DNS refresh. Manual `GrpcChannel` instantiation
is explicitly avoided.

## Protocol comparison

| Concern | REST / JSON | gRPC / Protobuf |
|---|---|---|
| Payload format | Text (JSON) | Binary (Protobuf) |
| Typical payload size | ~500 bytes | ~50 bytes (~10x smaller) |
| Contract enforcement | Optional (OpenAPI) | Mandatory (.proto, compiler-enforced) |
| Code generation | Optional | Automatic via Grpc.Tools |
| Transport protocol | HTTP/1.1 | HTTP/2 (multiplexed) |
| Streaming support | Limited | Native (unary, server, client, bidirectional) |
| Browser support | Native | Requires grpc-web proxy |
| Human readable | Yes | No (binary — use grpcurl for debugging) |
| Schema drift detection | Runtime | Compile time |

## Consequences

### Positive

- Protobuf binary payload is significantly smaller than JSON — lower latency
  per call. Meaningful for a high-frequency internal call on every ticket
  operation.
- Contract-first `.proto` file at solution root means schema drift between
  the Ticket Service and SLA Service is a compile error, not a runtime surprise.
- Strongly typed generated client eliminates stringly-typed HTTP calls and
  manual JSON deserialisation.
- `ISlaServiceClient` abstraction keeps handlers clean — no gRPC types or
  Protobuf dependencies leak into the application layer.
- HTTP/2 multiplexing allows multiple concurrent gRPC calls over a single
  TCP connection — important for the GraphQL dashboard resolver which may
  issue several SLA lookups concurrently.
- `AddGrpcClient` via `IHttpClientFactory` handles connection pooling and
  DNS refresh automatically — no socket exhaustion under load.

### Negative

- Protobuf binary is not human-readable. Debugging requires tooling:
  `grpcurl` for command-line inspection, Postman gRPC mode for interactive
  testing.
- Browser clients cannot call gRPC directly — requires a grpc-web proxy
  (Envoy or YARP). Not an issue for FlowDesk since gRPC is used only for
  internal service-to-service calls, not browser-facing APIs.
- Field number permanence requires ongoing discipline from the team. A
  developer who reuses a field number after removing a field causes silent
  data corruption in clients that haven't been redeployed. Mitigated by
  code review and documented in this ADR.
- The `.proto` file at the solution root creates a shared dependency between
  services at build time. In a true polyglot environment (e.g. a Python
  consumer), the `.proto` file must be copied to the consuming project's
  repository and kept in sync manually.

## Alternatives Considered

### REST over HTTP/JSON

Viable and familiar. Rejected for this specific communication path because:

- JSON text overhead is unnecessary when both sides are under our control and
  performance matters.
- No compiler-enforced contract — schema drift between services is discovered
  at runtime.
- HTTP/1.1 request-response multiplexing is less efficient than HTTP/2 for
  concurrent calls.

REST remains the correct choice for the public-facing API where human
readability, browser compatibility, and tooling ecosystem matter.

### Asynchronous messaging via RabbitMQ

Rejected. SLA policy lookup is a synchronous request-response — the Ticket
Service cannot proceed with the ticket operation until it has received the
policy value. Async messaging is the correct pattern for fire-and-forget
operations (notifications, audit logs) not for values the caller blocks on.
Introducing RabbitMQ here would require a correlation ID, a reply queue, and
a timeout mechanism — all complexity with no benefit over a direct gRPC call.

### Shared database access

Rejected. If the Ticket Service queried the SLA Service's database directly,
both services would share a data store — tight coupling at the persistence
layer. The SLA Service could not change its schema without coordinating with
the Ticket Service. Services cannot be deployed or scaled independently.
Database-level coupling is the most harmful form of coupling in a microservices
architecture.

### GraphQL federation

Overkill for the current scale. GraphQL federation would allow the agent
dashboard to stitch data from multiple services in a single query without
the BFF layer. Rejected in favour of the simpler gRPC + GraphQL BFF approach
given the current three-service topology.

## References

- gRPC documentation: https://grpc.io/docs/
- Protocol Buffers language guide: https://protobuf.dev/programming-guides/proto3/
- Microsoft — gRPC for .NET: https://learn.microsoft.com/en-us/aspnet/core/grpc/
- FlowDesk implementation: `Protos/sla.proto`, `SlaPolicyGrpcService.cs`,
  `SlaServiceClient.cs`, `ISlaServiceClient.cs`
- Lesson 6: gRPC implementation notes
