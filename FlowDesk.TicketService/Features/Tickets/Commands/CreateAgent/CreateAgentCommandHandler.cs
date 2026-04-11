using System;
using FlowDesk.TicketService.Domain.Entities;
using FlowDesk.TicketService.Infrastructure.Persistence;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Commands.CreateAgent;

public class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand, Guid>
{
    private readonly AppDbContext _appDbContext;

    public CreateAgentCommandHandler(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
    }

    public Task<Guid> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return DoHandle(request, cancellationToken);
    }

    private async Task<Guid> DoHandle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        var agent = Agent.Create(request.Name, request.Email, request.TenantId);

        await _appDbContext.Agents.AddAsync(agent);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        return agent.Id;
    }
}
