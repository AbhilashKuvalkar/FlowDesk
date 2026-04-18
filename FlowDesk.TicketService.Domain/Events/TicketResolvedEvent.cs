using FlowDesk.TicketService.Domain.Common;

namespace FlowDesk.TicketService.Domain.Events;

public record TicketResolvedEvent(Guid TicketId, Guid TenantId, DateTime ResolvedAt) : IDomainEvent;
