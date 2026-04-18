using FlowDesk.TicketService.Domain.Common;
using FlowDesk.TicketService.Domain.Exceptions;
using FlowDesk.TicketService.Domain.Repositories;
using FlowDesk.TicketService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.Features.Tickets.Commands.AssignTicket;

public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand>
{
    private readonly IAgentRepository _agentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignTicketCommandHandler(
        IAgentRepository agentRepository,
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork)
    {
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public Task Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return DoHandle(request, cancellationToken);
    }

    private async Task DoHandle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository
            .GetByIdAsync(request.TicketId, request.TenantId, cancellationToken) ??
            throw new TicketNotFoundException(request.TicketId);

        var agent = await _agentRepository
            .GetByIdAsync(request.AgentId, request.TenantId, cancellationToken) ??
            throw new AgentNotFoundException(request.AgentId);

        ticket.AssignTo(agent);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
