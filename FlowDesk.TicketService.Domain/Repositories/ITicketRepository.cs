using FlowDesk.TicketService.Domain.Entities;

namespace FlowDesk.TicketService.Domain.Repositories;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);

    void Add(Ticket ticket);
}
