using FlowDesk.TicketService.Domain.Common;

namespace FlowDesk.TicketService.Domain.Events;

public record TicketAssignedEvent(Guid TicketId, Guid AgentId, Guid TenantId) : IDomainEvent;