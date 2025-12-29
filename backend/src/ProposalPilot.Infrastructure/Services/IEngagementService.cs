namespace ProposalPilot.Infrastructure.Services;

/// <summary>
/// Service for calculating and managing proposal engagement metrics
/// </summary>
public interface IEngagementService
{
    /// <summary>
    /// Calculate the engagement score for a proposal (0-100)
    /// </summary>
    Task<EngagementScoreResult> CalculateEngagementScoreAsync(Guid proposalId);

    /// <summary>
    /// Get detailed engagement metrics for a proposal
    /// </summary>
    Task<ProposalEngagementMetrics> GetEngagementMetricsAsync(Guid proposalId);

    /// <summary>
    /// Check if a follow-up should be triggered based on engagement
    /// </summary>
    Task<FollowUpRecommendation> GetFollowUpRecommendationAsync(Guid proposalId);
}

public record EngagementScoreResult(
    int Score,
    string Level, // Hot, Warm, Cold
    string Description,
    List<EngagementFactor> Factors
);

public record EngagementFactor(
    string Name,
    int Points,
    string Description
);

public record ProposalEngagementMetrics(
    Guid ProposalId,
    int TotalViews,
    int UniqueViews,
    int EmailOpens,
    int EmailClicks,
    int EngagementScore,
    string EngagementLevel,
    DateTime? FirstViewedAt,
    DateTime? LastViewedAt,
    DateTime? LastEmailOpenedAt,
    TimeSpan? TimeSinceLastActivity,
    int DaysSinceSent,
    List<EmailEngagement> EmailHistory
);

public record EmailEngagement(
    Guid EmailLogId,
    string EmailType,
    string Status,
    DateTime SentAt,
    DateTime? OpenedAt,
    DateTime? ClickedAt,
    int OpenCount,
    int ClickCount
);

public record FollowUpRecommendation(
    bool ShouldFollowUp,
    string Reason,
    string SuggestedTone, // gentle, friendly, urgent
    int RecommendedDelayDays,
    string SuggestedMessage
);
