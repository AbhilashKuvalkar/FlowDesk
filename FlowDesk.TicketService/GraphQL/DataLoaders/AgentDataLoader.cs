using FlowDesk.TicketService.Domain.Entities;
using FlowDesk.TicketService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.GraphQL.DataLoaders;

public class AgentDataLoader : BatchDataLoader<Guid, Agent?>
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public AgentDataLoader(
        IDbContextFactory<AppDbContext> contextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions loaderOptions
    ) : base(batchScheduler, loaderOptions)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    protected override async Task<IReadOnlyDictionary<Guid, Agent?>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken
    )
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var agents = await context.Agents
            .AsNoTracking()
            .Where(w => keys.Contains(w.Id))
            .ToListAsync(cancellationToken);
            
        return agents.ToDictionary(a => a.Id, a => (Agent?)a);
    }
}
