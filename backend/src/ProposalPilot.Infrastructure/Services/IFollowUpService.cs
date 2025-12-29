namespace ProposalPilot.Infrastructure.Services;

/// <summary>
/// Service for managing proposal follow-up emails
/// </summary>
public interface IFollowUpService
{
    /// <summary>
    /// Schedule a follow-up email for a proposal
    /// </summary>
    Task<ScheduleFollowUpResult> ScheduleFollowUpAsync(ScheduleFollowUpRequest request);

    /// <summary>
    /// Cancel a scheduled follow-up
    /// </summary>
    Task<bool> CancelFollowUpAsync(Guid followUpId, Guid userId);

    /// <summary>
    /// Get all follow-ups for a proposal
    /// </summary>
    Task<List<FollowUpDto>> GetFollowUpsAsync(Guid proposalId, Guid userId);

    /// <summary>
    /// Process a scheduled follow-up (called by Hangfire)
    /// </summary>
    Task ProcessScheduledFollowUpAsync(Guid followUpId);

    /// <summary>
    /// Check all proposals and schedule automatic follow-ups based on engagement
    /// </summary>
    Task ProcessAutomaticFollowUpsAsync();
}

public record ScheduleFollowUpRequest(
    Guid ProposalId,
    Guid UserId,
    DateTime ScheduledFor,
    string? CustomMessage = null,
    bool IsAutomatic = false
);

public record ScheduleFollowUpResult(
    bool Success,
    Guid? FollowUpId,
    string Message,
    DateTime? ScheduledFor = null
);

public record FollowUpDto(
    Guid Id,
    Guid ProposalId,
    int SequenceNumber,
    DateTime ScheduledFor,
    DateTime? SentAt,
    string Status,
    string Subject,
    string TriggerReason,
    bool CanCancel
);
