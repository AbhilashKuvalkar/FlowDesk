using FlowDesk.TicketService.Domain.Entities;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetTicketById;

public record GetTicketByIdQuery(Guid TicketId, Guid TenantId) : IRequest<TicketResponse?>;
