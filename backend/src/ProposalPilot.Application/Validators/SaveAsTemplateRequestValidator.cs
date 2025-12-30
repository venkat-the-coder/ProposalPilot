using FluentValidation;
using ProposalPilot.Shared.DTOs.Template;

namespace ProposalPilot.Application.Validators;

/// <summary>
/// Validator for saving a proposal as a template
/// </summary>
public class SaveAsTemplateRequestValidator : AbstractValidator<SaveAsTemplateRequest>
{
    public SaveAsTemplateRequestValidator()
    {
        RuleFor(x => x.ProposalId)
            .NotEmpty().WithMessage("Proposal ID is required");

        RuleFor(x => x.TemplateName)
            .NotEmpty().WithMessage("Template name is required")
            .MaximumLength(200).WithMessage("Template name must not exceed 200 characters")
            .MinimumLength(3).WithMessage("Template name must be at least 3 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Maximum 10 tags allowed");
    }
}
