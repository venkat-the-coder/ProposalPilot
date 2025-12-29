using FluentValidation;
using ProposalPilot.Shared.DTOs.Proposal;

namespace ProposalPilot.Application.Validators;

public class GenerateProposalRequestValidator : AbstractValidator<GenerateProposalRequest>
{
    private static readonly string[] ValidTones = { "professional", "friendly", "formal", "casual", "technical" };
    private static readonly string[] ValidLengths = { "short", "medium", "long", "detailed" };

    public GenerateProposalRequestValidator()
    {
        RuleFor(x => x.BriefId)
            .NotEmpty().WithMessage("Brief ID is required");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client ID is required");

        RuleFor(x => x.PreferredTone)
            .Must(tone => string.IsNullOrEmpty(tone) || ValidTones.Contains(tone.ToLowerInvariant()))
            .WithMessage($"Preferred tone must be one of: {string.Join(", ", ValidTones)}")
            .When(x => !string.IsNullOrEmpty(x.PreferredTone));

        RuleFor(x => x.ProposalLength)
            .Must(length => string.IsNullOrEmpty(length) || ValidLengths.Contains(length.ToLowerInvariant()))
            .WithMessage($"Proposal length must be one of: {string.Join(", ", ValidLengths)}")
            .When(x => !string.IsNullOrEmpty(x.ProposalLength));

        RuleFor(x => x.Emphasis)
            .MaximumLength(500).WithMessage("Emphasis must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Emphasis));
    }
}
