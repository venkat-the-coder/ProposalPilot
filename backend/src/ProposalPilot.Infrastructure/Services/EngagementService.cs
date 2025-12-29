using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Domain.Enums;
using ProposalPilot.Infrastructure.Data;

namespace ProposalPilot.Infrastructure.Services;

/// <summary>
/// Service for calculating and managing proposal engagement metrics
/// </summary>
public class EngagementService : IEngagementService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EngagementService> _logger;

    public EngagementService(
        ApplicationDbContext context,
        ILogger<EngagementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EngagementScoreResult> CalculateEngagementScoreAsync(Guid proposalId)
    {
        var proposal = await _context.Proposals
            .Include(p => p.EmailLogs)
            .Include(p => p.Analytics)
            .FirstOrDefaultAsync(p => p.Id == proposalId);

        if (proposal == null)
        {
            return new EngagementScoreResult(0, "Unknown", "Proposal not found", new List<EngagementFactor>());
        }

        var factors = new List<EngagementFactor>();
        var totalScore = 0;

        // Factor 1: Email Opens (max 25 points)
        var emailOpens = proposal.EmailLogs.Sum(e => e.OpenCount);
        if (emailOpens > 0)
        {
            var openPoints = Math.Min(emailOpens * 5, 25);
            totalScore += openPoints;
            factors.Add(new EngagementFactor("Email Opens", openPoints, $"{emailOpens} email open(s)"));
        }

        // Factor 2: Email Clicks (max 30 points)
        var emailClicks = proposal.EmailLogs.Sum(e => e.ClickCount);
        if (emailClicks > 0)
        {
            var clickPoints = Math.Min(emailClicks * 10, 30);
            totalScore += clickPoints;
            factors.Add(new EngagementFactor("Email Clicks", clickPoints, $"{emailClicks} link click(s)"));
        }

        // Factor 3: Proposal Views (max 20 points)
        var viewCount = proposal.ViewCount;
        if (viewCount > 0)
        {
            var viewPoints = Math.Min(viewCount * 4, 20);
            totalScore += viewPoints;
            factors.Add(new EngagementFactor("Proposal Views", viewPoints, $"{viewCount} view(s)"));
        }

        // Factor 4: Recency of Activity (max 15 points)
        var lastActivity = GetLastActivityDate(proposal);
        if (lastActivity.HasValue)
        {
            var daysSinceActivity = (DateTime.UtcNow - lastActivity.Value).TotalDays;
            var recencyPoints = daysSinceActivity switch
            {
                < 1 => 15,   // Activity today
                < 3 => 12,   // Activity in last 3 days
                < 7 => 8,    // Activity in last week
                < 14 => 4,   // Activity in last 2 weeks
                _ => 0       // No recent activity
            };
            totalScore += recencyPoints;
            factors.Add(new EngagementFactor("Recent Activity", recencyPoints,
                recencyPoints > 0 ? $"Last activity {daysSinceActivity:F0} days ago" : "No recent activity"));
        }

        // Factor 5: Multiple Sessions (max 10 points)
        var uniqueSessions = proposal.Analytics.Select(a => a.ViewedAt.Date).Distinct().Count();
        if (uniqueSessions > 1)
        {
            var sessionPoints = Math.Min(uniqueSessions * 3, 10);
            totalScore += sessionPoints;
            factors.Add(new EngagementFactor("Return Visits", sessionPoints, $"{uniqueSessions} unique session(s)"));
        }

        // Determine engagement level
        var (level, description) = totalScore switch
        {
            >= 70 => ("Hot", "Highly engaged - likely to convert"),
            >= 40 => ("Warm", "Showing interest - follow up recommended"),
            >= 15 => ("Cold", "Low engagement - needs attention"),
            _ => ("Inactive", "No engagement yet")
        };

        return new EngagementScoreResult(totalScore, level, description, factors);
    }

    public async Task<ProposalEngagementMetrics> GetEngagementMetricsAsync(Guid proposalId)
    {
        var proposal = await _context.Proposals
            .Include(p => p.EmailLogs)
            .Include(p => p.Analytics)
            .FirstOrDefaultAsync(p => p.Id == proposalId);

        if (proposal == null)
        {
            throw new InvalidOperationException("Proposal not found");
        }

        var scoreResult = await CalculateEngagementScoreAsync(proposalId);
        var lastActivity = GetLastActivityDate(proposal);

        var emailHistory = proposal.EmailLogs
            .OrderByDescending(e => e.SentAt)
            .Select(e => new EmailEngagement(
                e.Id,
                e.EmailType.ToString(),
                e.Status.ToString(),
                e.SentAt,
                e.FirstOpenedAt,
                e.FirstClickedAt,
                e.OpenCount,
                e.ClickCount
            ))
            .ToList();

        var daysSinceSent = proposal.SentAt.HasValue
            ? (int)(DateTime.UtcNow - proposal.SentAt.Value).TotalDays
            : 0;

        return new ProposalEngagementMetrics(
            ProposalId: proposalId,
            TotalViews: proposal.ViewCount,
            UniqueViews: proposal.Analytics.Select(a => a.ViewerIpAddress).Distinct().Count(),
            EmailOpens: proposal.EmailLogs.Sum(e => e.OpenCount),
            EmailClicks: proposal.EmailLogs.Sum(e => e.ClickCount),
            EngagementScore: scoreResult.Score,
            EngagementLevel: scoreResult.Level,
            FirstViewedAt: proposal.FirstViewedAt,
            LastViewedAt: proposal.LastViewedAt,
            LastEmailOpenedAt: proposal.EmailLogs
                .Where(e => e.LastOpenedAt.HasValue)
                .OrderByDescending(e => e.LastOpenedAt)
                .Select(e => e.LastOpenedAt)
                .FirstOrDefault(),
            TimeSinceLastActivity: lastActivity.HasValue ? DateTime.UtcNow - lastActivity.Value : null,
            DaysSinceSent: daysSinceSent,
            EmailHistory: emailHistory
        );
    }

    public async Task<FollowUpRecommendation> GetFollowUpRecommendationAsync(Guid proposalId)
    {
        var proposal = await _context.Proposals
            .Include(p => p.EmailLogs)
            .Include(p => p.FollowUps)
            .FirstOrDefaultAsync(p => p.Id == proposalId);

        if (proposal == null)
        {
            return new FollowUpRecommendation(false, "Proposal not found", "", 0, "");
        }

        // Check if proposal is already accepted or rejected
        if (proposal.Status == ProposalStatus.Accepted)
        {
            return new FollowUpRecommendation(false, "Proposal already accepted", "", 0, "");
        }

        if (proposal.Status == ProposalStatus.Rejected)
        {
            return new FollowUpRecommendation(false, "Proposal was rejected", "", 0, "");
        }

        // Check if proposal has been sent
        if (!proposal.SentAt.HasValue)
        {
            return new FollowUpRecommendation(false, "Proposal hasn't been sent yet", "", 0, "");
        }

        var daysSinceSent = (int)(DateTime.UtcNow - proposal.SentAt.Value).TotalDays;
        var pendingFollowUps = proposal.FollowUps.Where(f => f.Status == FollowUpStatus.Scheduled).ToList();
        var sentFollowUps = proposal.FollowUps.Where(f => f.Status == FollowUpStatus.Sent).Count();

        // Don't recommend if there's already a pending follow-up
        if (pendingFollowUps.Any())
        {
            return new FollowUpRecommendation(
                false,
                "A follow-up is already scheduled",
                "",
                0,
                ""
            );
        }

        // Limit total follow-ups
        if (sentFollowUps >= 3)
        {
            return new FollowUpRecommendation(
                false,
                "Maximum follow-ups reached (3)",
                "",
                0,
                ""
            );
        }

        var scoreResult = await CalculateEngagementScoreAsync(proposalId);
        var hasOpened = proposal.EmailLogs.Any(e => e.OpenCount > 0);
        var hasClicked = proposal.EmailLogs.Any(e => e.ClickCount > 0);

        // Determine follow-up strategy based on engagement
        if (!hasOpened && daysSinceSent >= 2)
        {
            // Never opened - send a gentle reminder
            return new FollowUpRecommendation(
                true,
                "No email opens detected",
                "friendly",
                0,
                "Just wanted to make sure my proposal didn't get lost in your inbox. I'd love to hear your thoughts!"
            );
        }

        if (hasOpened && !hasClicked && daysSinceSent >= 3)
        {
            // Opened but no clicks - they saw it but didn't engage
            return new FollowUpRecommendation(
                true,
                "Email opened but no proposal views",
                "curious",
                1,
                "I noticed you had a chance to see my email. I'd be happy to answer any questions or provide additional details about the proposal."
            );
        }

        if (hasClicked && daysSinceSent >= 5)
        {
            // Viewed the proposal - genuine interest
            return new FollowUpRecommendation(
                true,
                "Viewed proposal but no response",
                "helpful",
                2,
                "I hope you had a chance to review the proposal. Is there anything I can clarify or adjust to better meet your needs?"
            );
        }

        // Default: wait a bit longer
        var waitDays = daysSinceSent < 3 ? 3 - daysSinceSent : 0;
        return new FollowUpRecommendation(
            waitDays == 0,
            waitDays > 0 ? $"Wait {waitDays} more day(s)" : "Time for a follow-up",
            "gentle",
            waitDays,
            "I wanted to check in and see if you had any questions about the proposal I sent."
        );
    }

    private DateTime? GetLastActivityDate(Domain.Entities.Proposal proposal)
    {
        var dates = new List<DateTime?>();

        if (proposal.LastViewedAt.HasValue)
            dates.Add(proposal.LastViewedAt);

        var lastEmailActivity = proposal.EmailLogs
            .Select(e => new[] { e.LastOpenedAt, e.LastClickedAt }.Max())
            .Max();

        if (lastEmailActivity.HasValue)
            dates.Add(lastEmailActivity);

        return dates.Where(d => d.HasValue).Max();
    }
}
