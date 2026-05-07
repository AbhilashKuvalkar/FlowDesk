using FlowDesk.TicketService.Domain.Repositories;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetSlaPolicy;

public class GetSlaPolicyQueryHandler : IRequestHandler<GetSlaPolicyQuery, SlaPolicyResponse?>
{
    private readonly ISlaPolicyRepository _cacheService;

    public GetSlaPolicyQueryHandler(ISlaPolicyRepository cacheService)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public Task<SlaPolicyResponse?> Handle(GetSlaPolicyQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return DoHandle(request, cancellationToken);
    }

    private async Task<SlaPolicyResponse?> DoHandle(GetSlaPolicyQuery request, CancellationToken cancellationToken)
    {
        var policy = await _cacheService.GetByPriorityAsync(request.TicketPriority, request.TenantId, cancellationToken);
        
        if (policy is null)
            return null;
        
        return new SlaPolicyResponse(
            policy.Name,
            policy.Priority,
            policy.ResponseTimeMinutes,
            policy.ResolutionTimeMinutes,
            policy.TenantId,
            policy.CreatedAt
        );
    }
}
