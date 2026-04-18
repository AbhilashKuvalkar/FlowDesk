using FlowDesk.TicketService.Domain.Events;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.EventHandlers;

public class TicketResolvedEventHandler : INotificationHandler<TicketResolvedEvent>
{
    private readonly ILogger<TicketResolvedEventHandler> _logger;

    public TicketResolvedEventHandler(ILogger<TicketResolvedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TicketResolvedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket {TicketId} was resolved at {ResolvedAt} in tenant {TenantId}",
            notification.TicketId,
            notification.ResolvedAt,
            notification.TenantId);

        return Task.CompletedTask;
    }
}
