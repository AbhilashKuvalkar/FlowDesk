using FlowDesk.TicketService.Domain;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetSlaPolicyQuery;

public record GetSlaPolicyQuery(TicketPriority TicketPriority, Guid TenantId) : IRequest<SlaPolicyResponse?>;
