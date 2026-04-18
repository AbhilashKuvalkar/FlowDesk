using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Commands.ResolveTicket;

public record ResolveTicketCommand(
    Guid TicketId,
    Guid TenantId
) : IRequest;
