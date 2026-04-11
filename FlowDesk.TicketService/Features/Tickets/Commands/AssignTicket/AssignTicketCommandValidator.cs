using FluentValidation;

namespace FlowDesk.TicketService.Features.Tickets.Commands.AssignTicket;

public class AssignTicketCommandValidator : AbstractValidator<AssignTicketCommand>
{
    public AssignTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage(x => $"{nameof(x.TicketId)} is required.");

        RuleFor(x => x.AgentId)
            .NotEmpty().WithMessage(x => $"{nameof(x.AgentId)} is required.");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(x => $"{nameof(x.TenantId)} is required.");
    }
}
