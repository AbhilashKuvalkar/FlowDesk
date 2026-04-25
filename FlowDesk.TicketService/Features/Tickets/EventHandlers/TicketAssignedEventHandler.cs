using FlowDesk.Contracts.Messages;
using FlowDesk.TicketService.Domain.Events;
using MassTransit;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.EventHandlers;

public class TicketAssignedEventHandler : INotificationHandler<TicketAssignedEvent>
{
    private readonly ILogger<TicketAssignedEventHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public TicketAssignedEventHandler(ILogger<TicketAssignedEventHandler> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task Handle(TicketAssignedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket {TicketId} assigned to agent {AgentId} in tenant {TenantId}",
            notification.TicketId,
            notification.AgentId,
            notification.TenantId);

        await _publishEndpoint.Publish(
            new TicketAssignedMessage(
                notification.TicketId,
                notification.AgentId,
                notification.TenantId,
                DateTime.UtcNow),
            cancellationToken);
    }
}
