using FlowDesk.Grpc;
using FlowDesk.TicketService.Domain;
using FlowDesk.TicketService.Domain.Repositories;
using Grpc.Core;
using static FlowDesk.Grpc.SlaService;

namespace FlowDesk.SlaService.Services;

public class SlaPolicyGrpcService : SlaServiceBase
{
    private readonly ILogger<SlaPolicyGrpcService> _logger;
    private readonly ISlaPolicyRepository _slaPolicyRepository;

    public SlaPolicyGrpcService(ILogger<SlaPolicyGrpcService> logger, ISlaPolicyRepository slaPolicyRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _slaPolicyRepository = slaPolicyRepository ?? throw new ArgumentNullException(nameof(slaPolicyRepository));
    }

    public override async Task<GetSlaPolicyResponse> GetSlaPolicyByPriority(GetSlaPolicyRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC request for SLA policy - tenant {TenantId}, priority {Priority}", 
            request.TenantId, request.Priority);

        if (!Guid.TryParse(request.TenantId, out var tenantId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "TenantId must be a valid Guid."));

        if (!Enum.TryParse<TicketPriority>(request.Priority, out var priority))
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Priority '{request.Priority}' is not valid."));

        var policy = await _slaPolicyRepository.GetByPriorityAsync(priority, tenantId, context.CancellationToken);

        if (policy is null)
            return new GetSlaPolicyResponse() { Found = false };

        return new GetSlaPolicyResponse
        {
            Found = true,
            Name = policy.Name,
            PolicyId = policy.Id.ToString(),
            Priority = policy.Priority.ToString(),
            ResolutionTimeMinutes = policy.ResolutionTimeMinutes,
            ResponseTimeMinutes = policy.ResponseTimeMinutes
        };
    }
}
