using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Commands.CreateAgent;

public record CreateAgentCommand(
    string Name,
    string Email,
    Guid TenantId) : IRequest<Guid>;
