using ProposalPilot.Domain.Enums;

namespace ProposalPilot.Domain.Entities;

/// <summary>
/// Tracks email events from SendGrid for proposals
/// </summary>
public class EmailLog : BaseEntity
{
    public Guid ProposalId { get; set; }
    public Proposal Proposal { get; set; } = null!;

    // Recipient info
    public string RecipientEmail { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;

    // Email details
    public string Subject { get; set; } = string.Empty;
    public EmailType EmailType { get; set; } = EmailType.Proposal;
    public EmailStatus Status { get; set; } = EmailStatus.Sent;

    // SendGrid tracking
    public string? SendGridMessageId { get; set; }

    // Event timestamps
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }
    public DateTime? FirstOpenedAt { get; set; }
    public DateTime? LastOpenedAt { get; set; }
    public DateTime? FirstClickedAt { get; set; }
    public DateTime? LastClickedAt { get; set; }
    public DateTime? BouncedAt { get; set; }
    public DateTime? SpamReportedAt { get; set; }
    public DateTime? UnsubscribedAt { get; set; }

    // Engagement metrics
    public int OpenCount { get; set; }
    public int ClickCount { get; set; }
    public int UniqueClickCount { get; set; }

    // Bounce/Error info
    public string? BounceType { get; set; }
    public string? BounceReason { get; set; }

    // Links clicked (JSON array)
    public string? ClickedLinksJson { get; set; }

    // User agent info from opens
    public string? LastUserAgent { get; set; }
    public string? LastIpAddress { get; set; }
}
