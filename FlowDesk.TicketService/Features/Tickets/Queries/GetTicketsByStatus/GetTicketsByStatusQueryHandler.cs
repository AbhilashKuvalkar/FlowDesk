using FlowDesk.TicketService.Features.Tickets.Queries.GetTicketById;
using FlowDesk.TicketService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetTicketsByStatus;

public class GetTicketsByStatusQueryHandler : IRequestHandler<GetTicketsByStatusQuery, List<TicketResponse>>
{
    private readonly AppDbContext _appDbContext;

    public GetTicketsByStatusQueryHandler(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
    }

    public Task<List<TicketResponse>> Handle(GetTicketsByStatusQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return DoHandle(request, cancellationToken);
    }

    private async Task<List<TicketResponse>> DoHandle(GetTicketsByStatusQuery request, CancellationToken cancellationToken)
    {
        return await _appDbContext.Tickets
            .AsNoTracking()
            .Where(x => x.Status == request.TicketStatus && x.TenantId == request.TenantId)
            .Select(t => new TicketResponse(
                t.Id,
                t.Title,
                t.Description,
                t.Status,
                t.Priority,
                t.Category,
                t.AssignedAgentId,
                t.CreatedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
