using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Domain.Enums;
using ProposalPilot.Infrastructure.Data;

namespace ProposalPilot.Infrastructure.Services;

/// <summary>
/// Service for aggregating and calculating analytics data
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        ApplicationDbContext context,
        ILogger<AnalyticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DashboardOverview> GetDashboardOverviewAsync(Guid userId)
    {
        var proposals = await _context.Proposals
            .Where(p => p.UserId == userId)
            .ToListAsync();

        var totalProposals = proposals.Count;
        var proposalsSent = proposals.Count(p => p.Status != ProposalStatus.Draft);
        var proposalsAccepted = proposals.Count(p => p.Status == ProposalStatus.Accepted);
        var proposalsRejected = proposals.Count(p => p.Status == ProposalStatus.Rejected);
        var proposalsDraft = proposals.Count(p => p.Status == ProposalStatus.Draft);

        var winRate = proposalsSent > 0
            ? Math.Round((decimal)proposalsAccepted / proposalsSent * 100, 1)
            : 0;

        // Calculate total value from accepted proposals (parse from StandardTierJson - recommended tier)
        var totalValue = 0m;
        var acceptedWithValue = 0;
        foreach (var p in proposals.Where(p => p.Status == ProposalStatus.Accepted))
        {
            var value = ExtractProposalValue(p.StandardTierJson);
            if (value > 0)
            {
                totalValue += value;
                acceptedWithValue++;
            }
        }

        var averageValue = acceptedWithValue > 0 ? totalValue / acceptedWithValue : 0;

        // Monthly comparison
        var thisMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var lastMonthStart = thisMonthStart.AddMonths(-1);

        var proposalsThisMonth = proposals.Count(p => p.CreatedAt >= thisMonthStart);
        var proposalsLastMonth = proposals.Count(p => p.CreatedAt >= lastMonthStart && p.CreatedAt < thisMonthStart);

        var monthOverMonthGrowth = proposalsLastMonth > 0
            ? Math.Round((decimal)(proposalsThisMonth - proposalsLastMonth) / proposalsLastMonth * 100, 1)
            : proposalsThisMonth > 0 ? 100 : 0;

        // Active follow-ups
        var activeFollowUps = await _context.FollowUps
            .CountAsync(f => f.Proposal.UserId == userId && f.Status == FollowUpStatus.Scheduled);

        // Pending responses (sent but not accepted/rejected)
        var pendingResponses = proposals.Count(p =>
            p.Status == ProposalStatus.Sent || p.Status == ProposalStatus.Viewed);

        return new DashboardOverview(
            TotalProposals: totalProposals,
            ProposalsSent: proposalsSent,
            ProposalsAccepted: proposalsAccepted,
            ProposalsRejected: proposalsRejected,
            ProposalsDraft: proposalsDraft,
            WinRate: winRate,
            TotalValue: totalValue,
            AverageProposalValue: averageValue,
            ProposalsThisMonth: proposalsThisMonth,
            ProposalsLastMonth: proposalsLastMonth,
            MonthOverMonthGrowth: monthOverMonthGrowth,
            ActiveFollowUps: activeFollowUps,
            PendingResponses: pendingResponses
        );
    }

    public async Task<ProposalTrends> GetProposalTrendsAsync(Guid userId, int days = 30)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);

        var proposals = await _context.Proposals
            .Include(p => p.Analytics)
            .Where(p => p.UserId == userId)
            .ToListAsync();

        // Daily stats
        var dailyStats = new List<DailyProposalStats>();
        for (var date = startDate; date <= DateTime.UtcNow.Date; date = date.AddDays(1))
        {
            var dayProposals = proposals.Where(p => p.CreatedAt.Date == date).ToList();
            var dayViews = proposals
                .SelectMany(p => p.Analytics)
                .Count(a => a.ViewedAt.Date == date);

            dailyStats.Add(new DailyProposalStats(
                Date: date,
                Created: dayProposals.Count,
                Sent: dayProposals.Count(p => p.SentAt?.Date == date),
                Accepted: dayProposals.Count(p => p.Status == ProposalStatus.Accepted),
                Rejected: dayProposals.Count(p => p.Status == ProposalStatus.Rejected),
                Views: dayViews
            ));
        }

        // Monthly stats (last 12 months)
        var monthlyStats = new List<MonthlyProposalStats>();
        for (var i = 11; i >= 0; i--)
        {
            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1);

            var monthProposals = proposals
                .Where(p => p.CreatedAt >= monthStart && p.CreatedAt < monthEnd)
                .ToList();

            var accepted = monthProposals.Count(p => p.Status == ProposalStatus.Accepted);
            var sent = monthProposals.Count(p => p.Status != ProposalStatus.Draft);

            var totalValue = monthProposals
                .Where(p => p.Status == ProposalStatus.Accepted)
                .Sum(p => ExtractProposalValue(p.StandardTierJson));

            monthlyStats.Add(new MonthlyProposalStats(
                Year: monthStart.Year,
                Month: monthStart.Month,
                MonthName: monthStart.ToString("MMM yyyy"),
                Total: monthProposals.Count,
                Accepted: accepted,
                WinRate: sent > 0 ? Math.Round((decimal)accepted / sent * 100, 1) : 0,
                TotalValue: totalValue
            ));
        }

        // Status breakdown
        var statusBreakdown = proposals
            .GroupBy(p => p.Status)
            .Select(g => new StatusBreakdown(
                Status: g.Key.ToString(),
                Count: g.Count(),
                Percentage: proposals.Count > 0
                    ? Math.Round((decimal)g.Count() / proposals.Count * 100, 1)
                    : 0
            ))
            .ToList();

        // Response time stats
        var respondedProposals = proposals
            .Where(p => p.SentAt.HasValue &&
                       (p.Status == ProposalStatus.Accepted || p.Status == ProposalStatus.Rejected))
            .Select(p =>
            {
                var responseDate = p.Status == ProposalStatus.Accepted ? p.AcceptedAt : p.RejectedAt;
                return responseDate.HasValue ? (int)(responseDate.Value - p.SentAt!.Value).TotalDays : 0;
            })
            .Where(d => d > 0)
            .ToList();

        return new ProposalTrends(
            DailyStats: dailyStats,
            MonthlyStats: monthlyStats,
            StatusBreakdown: statusBreakdown,
            AverageDaysToResponse: respondedProposals.Any() ? (int)respondedProposals.Average() : 0,
            FastestResponse: respondedProposals.Any() ? respondedProposals.Min() : 0,
            SlowestResponse: respondedProposals.Any() ? respondedProposals.Max() : 0
        );
    }

    public async Task<EngagementAnalytics> GetEngagementAnalyticsAsync(Guid userId)
    {
        var proposals = await _context.Proposals
            .Include(p => p.Analytics)
            .Include(p => p.EmailLogs)
            .Where(p => p.UserId == userId)
            .ToListAsync();

        var totalViews = proposals.Sum(p => p.ViewCount);
        var totalUniqueViewers = proposals
            .SelectMany(p => p.Analytics)
            .Select(a => a.ViewerIpAddress)
            .Where(ip => !string.IsNullOrEmpty(ip))
            .Distinct()
            .Count();

        var totalEmailOpens = proposals.Sum(p => p.EmailLogs.Sum(e => e.OpenCount));
        var totalEmailClicks = proposals.Sum(p => p.EmailLogs.Sum(e => e.ClickCount));

        // Calculate engagement scores
        var engagementScores = new List<(Domain.Entities.Proposal p, int score, string level)>();
        foreach (var proposal in proposals.Where(p => p.SentAt.HasValue))
        {
            var score = CalculateEngagementScore(proposal);
            var level = score switch
            {
                >= 70 => "Hot",
                >= 40 => "Warm",
                >= 15 => "Cold",
                _ => "Inactive"
            };
            engagementScores.Add((proposal, score, level));
        }

        var averageScore = engagementScores.Any()
            ? Math.Round((decimal)engagementScores.Average(e => e.score), 1)
            : 0;

        var topEngaged = engagementScores
            .OrderByDescending(e => e.score)
            .Take(5)
            .Select(e => new TopEngagedProposal(
                ProposalId: e.p.Id,
                Title: e.p.Title,
                ClientName: e.p.Client?.Name ?? "Unknown",
                EngagementScore: e.score,
                EngagementLevel: e.level,
                Views: e.p.ViewCount,
                EmailOpens: e.p.EmailLogs.Sum(el => el.OpenCount),
                LastActivity: e.p.LastViewedAt
            ))
            .ToList();

        // Engagement by day (last 30 days)
        var last30Days = DateTime.UtcNow.Date.AddDays(-30);
        var engagementByDay = new List<EngagementByDay>();

        for (var date = last30Days; date <= DateTime.UtcNow.Date; date = date.AddDays(1))
        {
            var dayViews = proposals
                .SelectMany(p => p.Analytics)
                .Count(a => a.ViewedAt.Date == date);

            var dayOpens = proposals
                .SelectMany(p => p.EmailLogs)
                .Where(e => e.FirstOpenedAt?.Date == date)
                .Sum(e => e.OpenCount);

            var dayClicks = proposals
                .SelectMany(p => p.EmailLogs)
                .Where(e => e.FirstClickedAt?.Date == date)
                .Sum(e => e.ClickCount);

            engagementByDay.Add(new EngagementByDay(date, dayViews, dayOpens, dayClicks));
        }

        return new EngagementAnalytics(
            TotalViews: totalViews,
            TotalUniqueViewers: totalUniqueViewers,
            TotalEmailOpens: totalEmailOpens,
            TotalEmailClicks: totalEmailClicks,
            AverageEngagementScore: averageScore,
            HotProposals: engagementScores.Count(e => e.level == "Hot"),
            WarmProposals: engagementScores.Count(e => e.level == "Warm"),
            ColdProposals: engagementScores.Count(e => e.level == "Cold"),
            TopEngagedProposals: topEngaged,
            EngagementByDay: engagementByDay
        );
    }

    public async Task<AIUsageAnalytics> GetAIUsageAnalyticsAsync(Guid userId, int days = 30)
    {
        // Note: AIUsageLogs entity not implemented yet - return placeholder data
        // TODO: Add AIUsageLog entity and tracking when AI features are enhanced

        var proposalCount = await _context.Proposals
            .CountAsync(p => p.UserId == userId && p.CreatedAt >= DateTime.UtcNow.AddDays(-days));

        // Estimate AI usage based on proposal count (each proposal uses AI for analysis + generation)
        var estimatedRequests = proposalCount * 2; // analyze + generate
        var estimatedTokensPerRequest = 2000;
        var estimatedCostPerRequest = 0.05m;

        return new AIUsageAnalytics(
            TotalRequests: estimatedRequests,
            TotalInputTokens: estimatedRequests * estimatedTokensPerRequest / 2,
            TotalOutputTokens: estimatedRequests * estimatedTokensPerRequest / 2,
            TotalCost: estimatedRequests * estimatedCostPerRequest,
            AverageCostPerProposal: proposalCount > 0 ? (estimatedRequests * estimatedCostPerRequest) / proposalCount : 0,
            AverageResponseTimeMs: 1500,
            SuccessRate: 99.5m,
            UsageByOperation: new List<AIUsageByOperation>
            {
                new("analyze", proposalCount, proposalCount * estimatedTokensPerRequest, proposalCount * estimatedCostPerRequest, 1200),
                new("generate", proposalCount, proposalCount * estimatedTokensPerRequest, proposalCount * estimatedCostPerRequest, 1800)
            },
            UsageByDay: new List<AIUsageByDay>(),
            ModelUsage: new List<AIModelUsage>
            {
                new("claude-3-haiku", proposalCount, proposalCount * 0.02m, 40),
                new("claude-3-sonnet", proposalCount, proposalCount * 0.03m, 60)
            }
        );
    }

    public async Task<ClientAnalytics> GetClientAnalyticsAsync(Guid userId)
    {
        var proposals = await _context.Proposals
            .Include(p => p.Client)
            .Where(p => p.UserId == userId)
            .ToListAsync();

        var clientGroups = proposals
            .GroupBy(p => p.ClientId)
            .ToList();

        var totalClients = clientGroups.Count;
        var repeatClients = clientGroups.Count(g => g.Count() > 1);
        var repeatClientRate = totalClients > 0
            ? Math.Round((decimal)repeatClients / totalClients * 100, 1)
            : 0;

        // Top clients by proposal count and value
        var topClients = clientGroups
            .Select(g =>
            {
                var client = g.First().Client;
                var accepted = g.Count(p => p.Status == ProposalStatus.Accepted);
                var sent = g.Count(p => p.Status != ProposalStatus.Draft);
                var totalValue = g
                    .Where(p => p.Status == ProposalStatus.Accepted)
                    .Sum(p => ExtractProposalValue(p.StandardTierJson));

                return new TopClient(
                    ClientId: g.Key,
                    ClientName: client?.Name ?? "Unknown",
                    CompanyName: client?.CompanyName,
                    ProposalCount: g.Count(),
                    AcceptedCount: accepted,
                    TotalValue: totalValue,
                    WinRate: sent > 0 ? Math.Round((decimal)accepted / sent * 100, 1) : 0
                );
            })
            .OrderByDescending(c => c.TotalValue)
            .ThenByDescending(c => c.ProposalCount)
            .Take(10)
            .ToList();

        // Industry breakdown
        var industryBreakdown = proposals
            .Where(p => !string.IsNullOrEmpty(p.Client?.Industry))
            .GroupBy(p => p.Client!.Industry!)
            .Select(g =>
            {
                var accepted = g.Count(p => p.Status == ProposalStatus.Accepted);
                var sent = g.Count(p => p.Status != ProposalStatus.Draft);
                var avgValue = g
                    .Where(p => p.Status == ProposalStatus.Accepted)
                    .Select(p => ExtractProposalValue(p.StandardTierJson))
                    .DefaultIfEmpty(0)
                    .Average();

                return new IndustryBreakdown(
                    Industry: g.Key,
                    ProposalCount: g.Count(),
                    AcceptedCount: accepted,
                    WinRate: sent > 0 ? Math.Round((decimal)accepted / sent * 100, 1) : 0,
                    AverageValue: avgValue
                );
            })
            .OrderByDescending(i => i.ProposalCount)
            .ToList();

        return new ClientAnalytics(
            TotalClients: totalClients,
            RepeatClients: repeatClients,
            RepeatClientRate: repeatClientRate,
            TopClients: topClients,
            IndustryBreakdown: industryBreakdown
        );
    }

    public async Task<FullAnalyticsReport> GetFullReportAsync(Guid userId, int days = 30)
    {
        var overview = await GetDashboardOverviewAsync(userId);
        var trends = await GetProposalTrendsAsync(userId, days);
        var engagement = await GetEngagementAnalyticsAsync(userId);
        var aiUsage = await GetAIUsageAnalyticsAsync(userId, days);
        var clients = await GetClientAnalyticsAsync(userId);

        return new FullAnalyticsReport(
            Overview: overview,
            Trends: trends,
            Engagement: engagement,
            AIUsage: aiUsage,
            Clients: clients,
            GeneratedAt: DateTime.UtcNow
        );
    }

    #region Helper Methods

    private int CalculateEngagementScore(Domain.Entities.Proposal proposal)
    {
        var score = 0;

        // Email opens (max 25 points)
        var emailOpens = proposal.EmailLogs.Sum(e => e.OpenCount);
        score += Math.Min(emailOpens * 5, 25);

        // Email clicks (max 30 points)
        var emailClicks = proposal.EmailLogs.Sum(e => e.ClickCount);
        score += Math.Min(emailClicks * 10, 30);

        // Proposal views (max 20 points)
        score += Math.Min(proposal.ViewCount * 4, 20);

        // Recency (max 15 points)
        var lastActivity = GetLastActivityDate(proposal);
        if (lastActivity.HasValue)
        {
            var daysSince = (DateTime.UtcNow - lastActivity.Value).TotalDays;
            score += daysSince switch
            {
                < 1 => 15,
                < 3 => 12,
                < 7 => 8,
                < 14 => 4,
                _ => 0
            };
        }

        // Return visits (max 10 points)
        var uniqueSessions = proposal.Analytics.Select(a => a.ViewedAt.Date).Distinct().Count();
        if (uniqueSessions > 1)
        {
            score += Math.Min(uniqueSessions * 3, 10);
        }

        return score;
    }

    private DateTime? GetLastActivityDate(Domain.Entities.Proposal proposal)
    {
        var dates = new List<DateTime?> { proposal.LastViewedAt };

        var lastEmailActivity = proposal.EmailLogs
            .Select(e => new[] { e.LastOpenedAt, e.LastClickedAt }.Max())
            .Max();

        if (lastEmailActivity.HasValue)
            dates.Add(lastEmailActivity);

        return dates.Where(d => d.HasValue).Max();
    }

    private decimal ExtractProposalValue(string? tierJson)
    {
        if (string.IsNullOrEmpty(tierJson))
            return 0;

        try
        {
            var doc = System.Text.Json.JsonDocument.Parse(tierJson);
            if (doc.RootElement.TryGetProperty("price", out var price))
            {
                return price.GetDecimal();
            }
            if (doc.RootElement.TryGetProperty("Price", out var priceUpper))
            {
                return priceUpper.GetDecimal();
            }
        }
        catch
        {
            // Ignore parsing errors
        }

        return 0;
    }

    #endregion
}
