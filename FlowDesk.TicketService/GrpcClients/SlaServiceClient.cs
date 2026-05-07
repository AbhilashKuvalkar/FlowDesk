using FlowDesk.Grpc;
using FlowDesk.TicketService.Domain;
using FlowDesk.TicketService.Domain.Services;

namespace FlowDesk.TicketService.GrpcClients;

public class SlaServiceClient : ISlaServiceClient
{
    private readonly ILogger<SlaServiceClient> _logger;
    private readonly SlaService.SlaServiceClient _slaServiceClient;

    public SlaServiceClient(ILogger<SlaServiceClient> logger, SlaService.SlaServiceClient slaServiceClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _slaServiceClient = slaServiceClient ?? throw new ArgumentNullException(nameof(slaServiceClient));
    }

    public async Task<SlaPolicyDto?> GetSlaPolicyAsync(TicketPriority priority, Guid tenantId, CancellationToken cancellationToken)
    {
        var request = new GetSlaPolicyRequest
        {
            Priority = priority.ToString(),
            TenantId = tenantId.ToString()
        };

        var response = await _slaServiceClient.GetSlaPolicyByPriorityAsync(request: request, cancellationToken: cancellationToken);

        if (!response.Found)
            return null;

        return new SlaPolicyDto(
            Guid.Parse(response.PolicyId),
            response.Name,
            Enum.Parse<TicketPriority>(response.Priority),
            response.ResponseTimeMinutes,
            response.ResolutionTimeMinutes
        );
    }
}
