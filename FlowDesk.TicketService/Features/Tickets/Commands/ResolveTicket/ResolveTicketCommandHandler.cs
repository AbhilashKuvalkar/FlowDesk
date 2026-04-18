using FlowDesk.TicketService.Domain.Common;
using FlowDesk.TicketService.Domain.Exceptions;
using FlowDesk.TicketService.Domain.Repositories;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Commands.ResolveTicket;

public class ResolveTicketCommandHandler : IRequestHandler<ResolveTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResolveTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public Task Handle(ResolveTicketCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return DoHandle(request, cancellationToken);
    }

    private async Task DoHandle(ResolveTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository
            .GetByIdAsync(request.TicketId, request.TenantId, cancellationToken) ??
                throw new TicketNotFoundException(request.TicketId);

        ticket.Resolve();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
