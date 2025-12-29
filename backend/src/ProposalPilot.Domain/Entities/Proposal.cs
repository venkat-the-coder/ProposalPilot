using ProposalPilot.Domain.Enums;

namespace ProposalPilot.Domain.Entities;

public class Proposal : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProposalStatus Status { get; set; } = ProposalStatus.Draft;

    // Client information
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    // User who created the proposal
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Brief analysis
    public string? OriginalBrief { get; set; }
    public string? BriefAnalysis { get; set; }

    // Pricing tiers (JSON columns in database)
    public string BasicTierJson { get; set; } = string.Empty;
    public string StandardTierJson { get; set; } = string.Empty;
    public string PremiumTierJson { get; set; } = string.Empty;

    // Deliverables and timeline
    public string DeliverablesJson { get; set; } = string.Empty;
    public string TimelineJson { get; set; } = string.Empty;

    // Terms and conditions
    public string? TermsAndConditions { get; set; }
    public string? PaymentTerms { get; set; }

    // Tracking
    public int ViewCount { get; set; }
    public DateTime? FirstViewedAt { get; set; }
    public DateTime? LastViewedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    // Sharing
    public string ShareToken { get; set; } = Guid.NewGuid().ToString("N");
    public bool IsPublic { get; set; }

    // Navigation properties
    public ICollection<ProposalSection> Sections { get; set; } = new List<ProposalSection>();
    public ICollection<ProposalAnalytics> Analytics { get; set; } = new List<ProposalAnalytics>();
}
