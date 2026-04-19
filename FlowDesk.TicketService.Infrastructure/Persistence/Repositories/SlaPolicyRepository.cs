using FlowDesk.TicketService.Domain;
using FlowDesk.TicketService.Domain.Common;
using FlowDesk.TicketService.Domain.Entities;
using FlowDesk.TicketService.Domain.Exceptions;
using FlowDesk.TicketService.Domain.Repositories;
using FlowDesk.TicketService.Infrastructure.Caching;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.Infrastructure.Persistence.Repositories;

public class SlaPolicyRepository : ISlaPolicyRepository
{
    private readonly AppDbContext _appDbContext;
    private readonly ICacheService _cacheService;
    private const int CacheMinutes = 60;


    public SlaPolicyRepository(AppDbContext appDbContext, ICacheService cacheService)
    {
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public void Add(SlaPolicy policy) 
        => _appDbContext.SlaPolicies.Add(policy);

    public async Task<SlaPolicy?> GetByPriorityAsync(TicketPriority priority, Guid tenantId, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.SlaPolicy(tenantId, priority);
        var cached = await _cacheService.GetAsync<SlaPolicy>(cacheKey, cancellationToken);

        if (cached is not null)
            return cached;

        var policy = await _appDbContext.SlaPolicies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Priority == priority && x.TenantId == tenantId, cancellationToken);

        if (policy is not null)
        {
            await _cacheService.SetAsync<SlaPolicy>(
                cacheKey, 
                policy, 
                TimeSpan.FromMinutes(CacheMinutes), 
                cancellationToken);
        }

        return policy;
    }

    public async Task InvalidateCacheAsync(TicketPriority priority, Guid tenantId, CancellationToken cancellationToken) 
        => await _cacheService.RemoveAsync(CacheKeys.SlaPolicy(tenantId, priority), cancellationToken);

    public async Task Update(Guid id, int responseTimeMinutes, int resolutionTimeMinutes, Guid tenantId, CancellationToken cancellationToken)
    {
        var policy = await _appDbContext.SlaPolicies
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken) 
            ?? throw new PolicyNotFoundException(id);

        policy.Update(responseTimeMinutes, resolutionTimeMinutes);
    }
}
