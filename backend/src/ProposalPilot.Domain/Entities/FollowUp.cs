using ProposalPilot.Domain.Enums;

namespace ProposalPilot.Domain.Entities;

/// <summary>
/// Scheduled follow-up emails for proposals
/// </summary>
public class FollowUp : BaseEntity
{
    public Guid ProposalId { get; set; }
    public Proposal Proposal { get; set; } = null!;

    // Sequence in the follow-up chain (1, 2, 3...)
    public int SequenceNumber { get; set; } = 1;

    // Scheduling
    public DateTime ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }
    public FollowUpStatus Status { get; set; } = FollowUpStatus.Scheduled;

    // Email content (AI-generated)
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? PersonalizedMessage { get; set; }

    // Trigger conditions
    public string TriggerReason { get; set; } = string.Empty; // no_open, opened_no_response, reminder
    public int DaysSinceLastContact { get; set; }

    // Link to the email log when sent
    public Guid? EmailLogId { get; set; }
    public EmailLog? EmailLog { get; set; }

    // Cancellation info
    public string? CancelledReason { get; set; }
    public DateTime? CancelledAt { get; set; }
}
