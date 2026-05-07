using FlowDesk.TicketService.Domain;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetSlaPolicy;

public record GetSlaPolicyQuery(TicketPriority TicketPriority, Guid TenantId) : IRequest<SlaPolicyResponse?>;
