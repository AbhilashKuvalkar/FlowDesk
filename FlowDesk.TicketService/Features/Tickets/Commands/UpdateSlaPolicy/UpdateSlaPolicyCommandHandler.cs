using FlowDesk.TicketService.Domain.Common;
using FlowDesk.TicketService.Domain.Repositories;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Commands.UpdateSlaPolicy;

public class UpdateSlaPolicyCommandHandler : IRequestHandler<UpdateSlaPolicyCommand>
{
    private readonly ISlaPolicyRepository _slaPolicyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSlaPolicyCommandHandler(ISlaPolicyRepository slaPolicyRepository, IUnitOfWork unitOfWork)
    {
        _slaPolicyRepository = slaPolicyRepository ?? throw new ArgumentNullException(nameof(slaPolicyRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public Task Handle(UpdateSlaPolicyCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return DoHandle(request, cancellationToken);
    }

    private async Task DoHandle(UpdateSlaPolicyCommand request, CancellationToken cancellationToken)
    {
        await _slaPolicyRepository.Update(
            request.PolicyId,
            request.ResponseTimeMinutes,
            request.ResolutionTimeMinutes,
            request.TenantId,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _slaPolicyRepository.InvalidateCacheAsync(request.Priority, request.TenantId, cancellationToken);
    }
}
