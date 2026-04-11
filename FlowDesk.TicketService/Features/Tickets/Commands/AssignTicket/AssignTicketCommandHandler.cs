using FlowDesk.TicketService.Domain.Exceptions;
using FlowDesk.TicketService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.Features.Tickets.Commands.AssignTicket;

public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand>
{
    private readonly AppDbContext _appDbContext;

    public AssignTicketCommandHandler(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
    }

    public Task Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return DoHandle(request, cancellationToken);
    }

    private async Task DoHandle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _appDbContext.Tickets
            .FirstOrDefaultAsync(x => x.Id == request.TicketId
                && x.TenantId == request.TenantId, cancellationToken);

        var agent = await _appDbContext.Agents
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.AgentId &&
                x.TenantId == request.TenantId, cancellationToken);

        if (ticket is null)
            throw new TicketNotFoundException(request.TicketId);

        if (agent is null)
            throw new AgentNotFoundException(request.AgentId);

        ticket.AssignTo(agent);

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }
}
