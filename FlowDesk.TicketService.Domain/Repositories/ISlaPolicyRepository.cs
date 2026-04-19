using FlowDesk.TicketService.Domain.Entities;

namespace FlowDesk.TicketService.Domain.Repositories;

public interface ISlaPolicyRepository
{
    Task<SlaPolicy?> GetByPriorityAsync(
        TicketPriority priority,
        Guid tenantId,
        CancellationToken cancellationToken);

    void Add(SlaPolicy policy);

    Task InvalidateCacheAsync(
        TicketPriority priority,
        Guid tenantId,
        CancellationToken cancellationToken);

    Task Update(
        Guid id, 
        int responseTimeMinutes, 
        int resolutionTimeMinutes, 
        Guid tenantId, 
        CancellationToken cancellationToken);
}
