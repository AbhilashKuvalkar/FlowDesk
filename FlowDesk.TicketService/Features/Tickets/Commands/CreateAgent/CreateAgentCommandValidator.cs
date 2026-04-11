using FluentValidation;

namespace FlowDesk.TicketService.Features.Tickets.Commands.CreateAgent;

public class CreateAgentCommandValidator : AbstractValidator<CreateAgentCommand>
{
    public CreateAgentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage(x => $"{nameof(x.Name)} cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(200).WithMessage(x => $"{nameof(x.Email)} cannot exceed 200 characters.");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(x => $"{nameof(x.TenantId)} is required.");
    }
}
