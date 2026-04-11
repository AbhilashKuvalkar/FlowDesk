using FlowDesk.TicketService.Domain.Entities;
using FlowDesk.TicketService.Infrastructure.Persistence;
using MediatR;

namespace FlowDesk.TicketService.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly AppDbContext _appDbContext;

    public CreateTicketCommandHandler(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
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

        await _appDbContext.Tickets.AddAsync(ticket, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
