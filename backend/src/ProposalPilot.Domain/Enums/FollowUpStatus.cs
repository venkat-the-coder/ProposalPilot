namespace ProposalPilot.Domain.Enums;

public enum FollowUpStatus
{
    Scheduled = 0,
    Sent = 1,
    Cancelled = 2,
    Skipped = 3,  // Skipped because client responded
    Failed = 4
}
