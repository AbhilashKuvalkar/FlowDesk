using FlowDesk.Contracts.Messages;
using FlowDesk.TicketService.Domain.Events;
using MassTransit;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.EventHandlers;

public class TicketCreatedEventHandler : INotificationHandler<TicketCreatedEvent>
{
    private readonly ILogger<TicketCreatedEventHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public TicketCreatedEventHandler(ILogger<TicketCreatedEventHandler> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new TicketCreatedMessage(
                notification.TicketId,
                notification.Title,
                notification.TenantId,
                DateTime.UtcNow),
            cancellationToken);

        _logger.LogInformation(
            "Ticket {TicketId} with title {Title} is created in tenant {TenantId}",
            notification.TicketId,
            notification.Title,
            notification.TenantId);
    }
}
