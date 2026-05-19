using FlowDesk.TicketService.Domain.Repositories;
using FlowDesk.TicketService.Domain.Services;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetSlaStatus;

public class GetSlaStatusQueryHandler : IRequestHandler<GetSlaStatusQuery, SlaStatusResponse?>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ISlaServiceClient _slaServiceClient;

    public GetSlaStatusQueryHandler(ITicketRepository ticketRepository, ISlaServiceClient slaServiceClient)
    {
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _slaServiceClient = slaServiceClient ?? throw new ArgumentNullException(nameof(slaServiceClient));
    }

    public Task<SlaStatusResponse?> Handle(GetSlaStatusQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return DoHandle(request, cancellationToken);
    }

    private async Task<SlaStatusResponse?> DoHandle(GetSlaStatusQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository
            .GetByIdAsNoTrackingAsync(request.TicketId, request.TenantId, cancellationToken);

        if (ticket is null)
            return null;

        var policy = await _slaServiceClient.GetSlaPolicyAsync(ticket.Priority, request.TenantId, cancellationToken);

        if (policy is null)
            return null;

        var isBreaching = SlaBreachCalculator.IsBreaching(ticket.Status, ticket.CreatedAt, policy);
        
        return new SlaStatusResponse(
            ticket.Id,
            ticket.Status,
            policy.ResponseTimeMinutes,
            policy.ResolutionTimeMinutes,
            isBreaching);
    }
}
