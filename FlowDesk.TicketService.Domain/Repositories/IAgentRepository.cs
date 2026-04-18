using FlowDesk.TicketService.Domain.Entities;

namespace FlowDesk.TicketService.Domain.Repositories;

public interface IAgentRepository
{
    Task<Agent?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);
}
