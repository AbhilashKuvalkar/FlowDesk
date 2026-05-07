using FlowDesk.TicketService.Domain.Entities;
using FlowDesk.TicketService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.Infrastructure.Persistence.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;

    public TicketRepository(AppDbContext context) => _context = context;

    public async Task<Ticket?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken) 
        => await _context.Tickets
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

    public void Add(Ticket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        _context.Tickets.Add(ticket);
    }

    public async Task<Ticket?> GetByIdAsNoTrackingAsync(Guid id, Guid tenantId, CancellationToken cancellationToken)
        => await _context.Tickets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);
}
