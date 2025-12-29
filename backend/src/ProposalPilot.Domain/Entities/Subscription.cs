using ProposalPilot.Domain.Enums;

namespace ProposalPilot.Domain.Entities;

public class Subscription : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Free;
    public decimal MonthlyPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CancelledAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool AutoRenew { get; set; } = true;

    // Stripe integration
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCustomerId { get; set; }

    // Usage limits
    public int ProposalsPerMonth { get; set; }
    public int ProposalsUsedThisMonth { get; set; }
    public DateTime UsageResetDate { get; set; }

    // Features
    public bool HasAIAnalysis { get; set; }
    public bool HasAdvancedTemplates { get; set; }
    public bool HasPrioritySupport { get; set; }
    public bool HasWhiteLabeling { get; set; }

    // Navigation properties
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
