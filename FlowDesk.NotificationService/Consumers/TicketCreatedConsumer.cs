using System;
using FlowDesk.Contracts.Messages;
using MassTransit;

namespace FlowDesk.NotificationService.Consumers;

public class TicketCreatedConsumer : IConsumer<TicketCreatedMessage>
{
    private readonly ILogger<TicketCreatedConsumer> _logger;

    public TicketCreatedConsumer(ILogger<TicketCreatedConsumer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<TicketCreatedMessage> context)
    {
        var message = context.Message;

        if (message is null)
            return;

        _logger.LogInformation(
            "Ticket {TicketId} was created at {CreatedAt} with title: {Title}",
            message.TicketId,
            message.CreatedAt,
            message.Title);

        // In production: inject IEmailService and send real email
        // For now: simulate async work
        await Task.Delay(100, context.CancellationToken);

        _logger.LogInformation(
            "Notification sent for ticket {TicketId}",
            message.TicketId);
    }
}
