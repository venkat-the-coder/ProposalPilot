using FluentValidation;
using ProposalPilot.Shared.DTOs.Template;

namespace ProposalPilot.Application.Validators;

/// <summary>
/// Validator for creating a new template
/// </summary>
public class CreateTemplateRequestValidator : AbstractValidator<CreateTemplateRequest>
{
    public CreateTemplateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Template name is required")
            .MaximumLength(200).WithMessage("Template name must not exceed 200 characters")
            .MinimumLength(3).WithMessage("Template name must be at least 3 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters")
            .Must(BeValidCategory).WithMessage("Invalid category. Choose from: Web Development, Marketing, Design, Consulting, Writing, Video Production, Other");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Maximum 10 tags allowed")
            .Must(tags => tags == null || tags.All(t => t.Length <= 50))
            .WithMessage("Each tag must not exceed 50 characters");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Template content is required");

        RuleFor(x => x.EstimatedTimeMinutes)
            .GreaterThan(0).WithMessage("Estimated time must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Estimated time seems unrealistic")
            .When(x => x.EstimatedTimeMinutes.HasValue);
    }

    private bool BeValidCategory(string category)
    {
        var validCategories = new[]
        {
            "Web Development",
            "Marketing",
            "Design",
            "Consulting",
            "Writing",
            "Video Production",
            "Mobile Development",
            "SEO",
            "Social Media",
            "Other"
        };

        return validCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }
}
