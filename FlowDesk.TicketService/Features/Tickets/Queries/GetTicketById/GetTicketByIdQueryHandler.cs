using FlowDesk.TicketService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketResponse?>
{
    private readonly AppDbContext _appDbContext;

    public GetTicketByIdQueryHandler(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
    }

    public Task<TicketResponse?> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return DoHandle(request, cancellationToken);
    }

    private async Task<TicketResponse?> DoHandle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        return await _appDbContext.Tickets
            .AsNoTracking()
            .Where(t => t.Id == request.TicketId && t.TenantId == request.TenantId)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
