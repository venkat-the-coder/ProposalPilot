namespace ProposalPilot.Infrastructure.Services;

/// <summary>
/// Service for aggregating and calculating analytics data
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Get dashboard overview statistics for a user
    /// </summary>
    Task<DashboardOverview> GetDashboardOverviewAsync(Guid userId);

    /// <summary>
    /// Get proposal statistics over time
    /// </summary>
    Task<ProposalTrends> GetProposalTrendsAsync(Guid userId, int days = 30);

    /// <summary>
    /// Get engagement analytics across all proposals
    /// </summary>
    Task<EngagementAnalytics> GetEngagementAnalyticsAsync(Guid userId);

    /// <summary>
    /// Get AI usage statistics
    /// </summary>
    Task<AIUsageAnalytics> GetAIUsageAnalyticsAsync(Guid userId, int days = 30);

    /// <summary>
    /// Get client/industry breakdown
    /// </summary>
    Task<ClientAnalytics> GetClientAnalyticsAsync(Guid userId);

    /// <summary>
    /// Get complete analytics report
    /// </summary>
    Task<FullAnalyticsReport> GetFullReportAsync(Guid userId, int days = 30);
}

#region DTOs

/// <summary>
/// Dashboard overview with key metrics
/// </summary>
public record DashboardOverview(
    int TotalProposals,
    int ProposalsSent,
    int ProposalsAccepted,
    int ProposalsRejected,
    int ProposalsDraft,
    decimal WinRate,
    decimal TotalValue,
    decimal AverageProposalValue,
    int ProposalsThisMonth,
    int ProposalsLastMonth,
    decimal MonthOverMonthGrowth,
    int ActiveFollowUps,
    int PendingResponses
);

/// <summary>
/// Proposal trends over time
/// </summary>
public record ProposalTrends(
    List<DailyProposalStats> DailyStats,
    List<MonthlyProposalStats> MonthlyStats,
    List<StatusBreakdown> StatusBreakdown,
    int AverageDaysToResponse,
    int FastestResponse,
    int SlowestResponse
);

public record DailyProposalStats(
    DateTime Date,
    int Created,
    int Sent,
    int Accepted,
    int Rejected,
    int Views
);

public record MonthlyProposalStats(
    int Year,
    int Month,
    string MonthName,
    int Total,
    int Accepted,
    decimal WinRate,
    decimal TotalValue
);

public record StatusBreakdown(
    string Status,
    int Count,
    decimal Percentage
);

/// <summary>
/// Engagement analytics across proposals
/// </summary>
public record EngagementAnalytics(
    int TotalViews,
    int TotalUniqueViewers,
    int TotalEmailOpens,
    int TotalEmailClicks,
    decimal AverageEngagementScore,
    int HotProposals,
    int WarmProposals,
    int ColdProposals,
    List<TopEngagedProposal> TopEngagedProposals,
    List<EngagementByDay> EngagementByDay
);

public record TopEngagedProposal(
    Guid ProposalId,
    string Title,
    string ClientName,
    int EngagementScore,
    string EngagementLevel,
    int Views,
    int EmailOpens,
    DateTime? LastActivity
);

public record EngagementByDay(
    DateTime Date,
    int Views,
    int EmailOpens,
    int EmailClicks
);

/// <summary>
/// AI usage and cost analytics
/// </summary>
public record AIUsageAnalytics(
    int TotalRequests,
    int TotalInputTokens,
    int TotalOutputTokens,
    decimal TotalCost,
    decimal AverageCostPerProposal,
    int AverageResponseTimeMs,
    decimal SuccessRate,
    List<AIUsageByOperation> UsageByOperation,
    List<AIUsageByDay> UsageByDay,
    List<AIModelUsage> ModelUsage
);

public record AIUsageByOperation(
    string Operation,
    int RequestCount,
    int TotalTokens,
    decimal TotalCost,
    decimal AverageResponseTimeMs
);

public record AIUsageByDay(
    DateTime Date,
    int Requests,
    int Tokens,
    decimal Cost
);

public record AIModelUsage(
    string Model,
    int RequestCount,
    decimal TotalCost,
    decimal Percentage
);

/// <summary>
/// Client and industry analytics
/// </summary>
public record ClientAnalytics(
    int TotalClients,
    int RepeatClients,
    decimal RepeatClientRate,
    List<TopClient> TopClients,
    List<IndustryBreakdown> IndustryBreakdown
);

public record TopClient(
    Guid ClientId,
    string ClientName,
    string? CompanyName,
    int ProposalCount,
    int AcceptedCount,
    decimal TotalValue,
    decimal WinRate
);

public record IndustryBreakdown(
    string Industry,
    int ProposalCount,
    int AcceptedCount,
    decimal WinRate,
    decimal AverageValue
);

/// <summary>
/// Complete analytics report combining all metrics
/// </summary>
public record FullAnalyticsReport(
    DashboardOverview Overview,
    ProposalTrends Trends,
    EngagementAnalytics Engagement,
    AIUsageAnalytics AIUsage,
    ClientAnalytics Clients,
    DateTime GeneratedAt
);

#endregion
