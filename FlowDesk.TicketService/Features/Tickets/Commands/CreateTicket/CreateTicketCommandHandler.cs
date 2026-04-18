using FlowDesk.TicketService.Domain.Common;
using FlowDesk.TicketService.Domain.Entities;
using FlowDesk.TicketService.Domain.Repositories;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork)
    {
        this._ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public Task<Guid> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return DoHandle(request, cancellationToken);
    }

    private async Task<Guid> DoHandle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = Ticket.Create(
            request.Title,
            request.Description,
            request.TicketPriority,
            request.TicketCategory,
            request.TenantId
        );

        _ticketRepository.Add(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
