using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetSlaStatus;

public record GetSlaStatusQuery(Guid TicketId, Guid TenantId) : IRequest<SlaStatusResponse?>;
