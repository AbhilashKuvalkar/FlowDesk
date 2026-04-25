using FlowDesk.Contracts.Messages;
using MassTransit;

namespace FlowDesk.NotificationService.Consumers;

public class TicketAssignedConsumer : IConsumer<TicketAssignedMessage>
{
    private readonly ILogger<TicketAssignedConsumer> _logger;

    public TicketAssignedConsumer(ILogger<TicketAssignedConsumer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<TicketAssignedMessage> context)
    {
        var message = context.Message;

        if (message is null)
            return;

        _logger.LogInformation(
            "Sending assignment notification for ticket {TicketId} to agent {AgentId}",
            message.TicketId,
            message.AgentId);

        // In production: inject IEmailService and send real email
        // For now: simulate async work
        await Task.Delay(100, context.CancellationToken);

        _logger.LogInformation(
            "Notification sent for ticket {TicketId}",
            message.TicketId);
    }
}
