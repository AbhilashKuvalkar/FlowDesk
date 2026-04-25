using FlowDesk.Contracts.Messages;
using FlowDesk.TicketService.Domain.Events;
using MassTransit;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.EventHandlers;

public class TicketResolvedEventHandler : INotificationHandler<TicketResolvedEvent>
{
    private readonly ILogger<TicketResolvedEventHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public TicketResolvedEventHandler(ILogger<TicketResolvedEventHandler> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task Handle(TicketResolvedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Ticket {TicketId} was resolved at {ResolvedAt} in tenant {TenantId}",
            notification.TicketId,
            notification.ResolvedAt,
            notification.TenantId);

        await _publishEndpoint.Publish(
            new TicketResolvedMessage(
                notification.TicketId,
                notification.TenantId,
                DateTime.UtcNow),
            cancellationToken);
    }
}
