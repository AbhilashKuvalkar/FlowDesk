using FluentValidation;

namespace FlowDesk.TicketService.Features.Tickets.Commands.ResolveTicket;

public class ResolveTicketCommandValidator : AbstractValidator<ResolveTicketCommand>
{
    public ResolveTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage(x => $"{nameof(x.TicketId)} is required.");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(x => $"{nameof(x.TenantId)} is required.");
    }
}
