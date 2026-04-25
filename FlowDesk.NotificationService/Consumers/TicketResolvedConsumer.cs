using FlowDesk.Contracts.Messages;
using MassTransit;

namespace FlowDesk.NotificationService.Consumers;

public class TicketResolvedConsumer : IConsumer<TicketResolvedMessage>
{
    private readonly ILogger<TicketResolvedConsumer> _logger;

    public TicketResolvedConsumer(ILogger<TicketResolvedConsumer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<TicketResolvedMessage> context)
    {
        var message = context.Message;

        if (message is null)
            return;

        _logger.LogInformation(
            "Ticket {TicketId} was resolved at: {ResolvedAt}",
            message.TicketId,
            message.ResolvedAt);

        // In production: inject IEmailService and send real email
        // For now: simulate async work
        await Task.Delay(100, context.CancellationToken);

        _logger.LogInformation(
            "Notification sent for ticket {TicketId}",
            message.TicketId);
    }
}
