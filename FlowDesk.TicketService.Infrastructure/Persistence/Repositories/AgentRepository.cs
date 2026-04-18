using FlowDesk.TicketService.Domain.Entities;
using FlowDesk.TicketService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.Infrastructure.Persistence.Repositories;

public class AgentRepository : IAgentRepository
{
    private readonly AppDbContext _context;

    public AgentRepository(AppDbContext context) => _context = context;

    public async Task<Agent?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
        => await _context.Agents
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

}
