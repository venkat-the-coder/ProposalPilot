using FluentValidation;
using ProposalPilot.Shared.DTOs.Proposal;

namespace ProposalPilot.Application.Validators;

public class UpdateProposalRequestValidator : AbstractValidator<UpdateProposalRequest>
{
    public UpdateProposalRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Description must not exceed 5,000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Sections)
            .Must(sections => sections == null || sections.Count <= 20)
            .WithMessage("Cannot have more than 20 sections")
            .When(x => x.Sections != null);

        RuleForEach(x => x.Sections)
            .ChildRules(section =>
            {
                section.RuleFor(s => s.Key)
                    .MaximumLength(100).WithMessage("Section name must not exceed 100 characters");

                section.RuleFor(s => s.Value)
                    .MaximumLength(50000).WithMessage("Section content must not exceed 50,000 characters");
            })
            .When(x => x.Sections != null);
    }
}
