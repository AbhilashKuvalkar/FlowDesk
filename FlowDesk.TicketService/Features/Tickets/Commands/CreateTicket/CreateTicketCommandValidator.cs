using FluentValidation;

namespace FlowDesk.TicketService.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage(x => $"{nameof(x.Title)} cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage(x => $"{nameof(x.Description)} cannot exceed 2000 characters.");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(x => $"{nameof(x.TenantId)} is required.");
    }
}
