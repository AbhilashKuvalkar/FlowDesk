using FlowDesk.TicketService.Domain.Events;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.EventHandlers;

public class TicketAssignedEventHandler : INotificationHandler<TicketAssignedEvent>
{
    private readonly ILogger<TicketAssignedEventHandler> _logger;

    public TicketAssignedEventHandler(ILogger<TicketAssignedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TicketAssignedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket {TicketId} assigned to agent {AgentId} in tenant {TenantId}",
            notification.TicketId,
            notification.AgentId,
            notification.TenantId);

        return Task.CompletedTask;
    }
}
