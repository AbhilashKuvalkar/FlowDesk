using FlowDesk.TicketService.Domain.Events;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.EventHandlers;

public class TicketCreatedEventHandler : INotificationHandler<TicketCreatedEvent>
{
    private readonly ILogger<TicketCreatedEventHandler> _logger;

    public TicketCreatedEventHandler(ILogger<TicketCreatedEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket {TicketId} with title {Title} is created in tenant {TenantId}",
            notification.TicketId,
            notification.Title,
            notification.TenantId);

        return Task.CompletedTask;
    }
}
