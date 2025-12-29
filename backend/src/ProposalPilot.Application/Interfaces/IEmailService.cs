namespace ProposalPilot.Application.Interfaces;

/// <summary>
/// Service for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send a proposal to a client via email
    /// </summary>
    Task<EmailResult> SendProposalAsync(SendProposalEmailRequest request);

    /// <summary>
    /// Send a generic email
    /// </summary>
    Task<EmailResult> SendEmailAsync(EmailRequest request);

    /// <summary>
    /// Send a follow-up email for a proposal
    /// </summary>
    Task<EmailResult> SendFollowUpAsync(SendFollowUpEmailRequest request);
}

/// <summary>
/// Request to send a proposal email
/// </summary>
public record SendProposalEmailRequest(
    string RecipientEmail,
    string RecipientName,
    string SenderName,
    string SenderEmail,
    string ProposalTitle,
    string ProposalSummary,
    string ProposalViewUrl,
    string? PersonalMessage = null
);

/// <summary>
/// Request to send a follow-up email
/// </summary>
public record SendFollowUpEmailRequest(
    string RecipientEmail,
    string RecipientName,
    string SenderName,
    string SenderEmail,
    string ProposalTitle,
    string ProposalViewUrl,
    string FollowUpMessage,
    int DaysSinceProposalSent
);

/// <summary>
/// Generic email request
/// </summary>
public record EmailRequest(
    string To,
    string ToName,
    string Subject,
    string HtmlContent,
    string? PlainTextContent = null,
    string? ReplyTo = null
);

/// <summary>
/// Result of sending an email
/// </summary>
public record EmailResult(
    bool Success,
    string? MessageId = null,
    string? ErrorMessage = null
);
