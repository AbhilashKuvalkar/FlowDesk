using FlowDesk.TicketService.Domain.Enums;
using FlowDesk.TicketService.Features.Tickets.Queries.GetTicketById;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetTicketsByStatus;

public record GetTicketsByStatusQuery(
    TicketStatus TicketStatus,
    Guid TenantId) : IRequest<List<TicketResponse>>;
