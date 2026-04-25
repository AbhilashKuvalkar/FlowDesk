namespace FlowDesk.Contracts.Messages;

public record TicketAssignedMessage(
    Guid TicketId,
    Guid AgentId,
    Guid TenantId,
    DateTime AssignedAt);