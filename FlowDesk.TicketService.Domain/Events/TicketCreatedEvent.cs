using FlowDesk.TicketService.Domain.Common;

namespace FlowDesk.TicketService.Domain.Events;

public record TicketCreatedEvent(Guid TicketId, string Title, Guid TenantId) : IDomainEvent;