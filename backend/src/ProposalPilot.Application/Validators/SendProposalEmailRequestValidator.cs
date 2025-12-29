using FluentValidation;
using ProposalPilot.Shared.DTOs.Proposal;

namespace ProposalPilot.Application.Validators;

public class SendProposalEmailRequestValidator : AbstractValidator<SendProposalEmailApiRequest>
{
    public SendProposalEmailRequestValidator()
    {
        RuleFor(x => x.RecipientEmail)
            .NotEmpty().WithMessage("Recipient email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.RecipientName)
            .NotEmpty().WithMessage("Recipient name is required")
            .MaximumLength(200).WithMessage("Recipient name must not exceed 200 characters")
            .MinimumLength(2).WithMessage("Recipient name must be at least 2 characters");

        RuleFor(x => x.PersonalMessage)
            .MaximumLength(1000).WithMessage("Personal message must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.PersonalMessage));
    }
}
