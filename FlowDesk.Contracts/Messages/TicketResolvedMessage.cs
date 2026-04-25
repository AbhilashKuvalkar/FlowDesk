namespace FlowDesk.Contracts.Messages;

public record TicketResolvedMessage(
    Guid TicketId,
    Guid TenantId,
    DateTime ResolvedAt);
