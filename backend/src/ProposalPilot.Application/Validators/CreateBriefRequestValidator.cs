using FluentValidation;
using ProposalPilot.Shared.DTOs.Brief;

namespace ProposalPilot.Application.Validators;

public class CreateBriefRequestValidator : AbstractValidator<CreateBriefRequest>
{
    public CreateBriefRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters");

        RuleFor(x => x.RawContent)
            .NotEmpty().WithMessage("Brief content is required")
            .MinimumLength(50).WithMessage("Brief content must be at least 50 characters for meaningful analysis")
            .MaximumLength(50000).WithMessage("Brief content must not exceed 50,000 characters");
    }
}
