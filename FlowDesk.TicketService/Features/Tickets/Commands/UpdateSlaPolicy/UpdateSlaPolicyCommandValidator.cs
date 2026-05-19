using FlowDesk.TicketService.Domain.Enums;
using FluentValidation;

namespace FlowDesk.TicketService.Features.Tickets.Commands.UpdateSlaPolicy;

public class UpdateSlaPolicyCommandValidator : AbstractValidator<UpdateSlaPolicyCommand>
{
    public UpdateSlaPolicyCommandValidator()
    {
        RuleFor(x => x.PolicyId)
            .NotEmpty().WithMessage(x => $"{nameof(x.PolicyId)} is required.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value.")
            .NotEqual(default(TicketPriority)).WithMessage(x => $"{nameof(x.Priority)} is required.");

        RuleFor(x => x.ResponseTimeMinutes)
            .GreaterThan(0).WithMessage(x => $"{nameof(x.ResponseTimeMinutes)} cannot be less than or equal to zero.");

        RuleFor(x => x.ResolutionTimeMinutes)
            .GreaterThan(0).WithMessage(x => $"{nameof(x.ResolutionTimeMinutes)} cannot be less than or equal to zero.");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(x => $"{nameof(x.TenantId)} is required.");
    }
}
