using FlowDesk.TicketService.Domain;
using FlowDesk.TicketService.Domain.Enums;
using FluentValidation;

namespace FlowDesk.TicketService.Features.Tickets.Queries.GetTicketsByStatus;

public class GetTicketsByStatusQueryValidator : AbstractValidator<GetTicketsByStatusQuery>
{
    public GetTicketsByStatusQueryValidator()
    {
        RuleFor(x => x.TicketStatus)
            .IsInEnum().WithMessage("Invalid ticket status value.")
            .NotEqual(default(TicketStatus)).WithMessage(x => $"{nameof(x.TicketStatus)} is required.");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(x => $"{nameof(x.TenantId)} is required.");
    }
}
