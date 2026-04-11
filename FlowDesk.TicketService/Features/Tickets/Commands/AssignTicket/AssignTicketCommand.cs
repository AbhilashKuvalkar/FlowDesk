using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Commands.AssignTicket;

public record AssignTicketCommand(
    Guid TicketId,
    Guid AgentId,
    Guid TenantId
) : IRequest;