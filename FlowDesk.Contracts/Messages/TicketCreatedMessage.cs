namespace FlowDesk.Contracts.Messages;

public record TicketCreatedMessage(
    Guid TicketId,
    string Title,
    Guid TenantId,
    DateTime CreatedAt);