namespace ProposalPilot.Domain.Entities;

using ProposalPilot.Domain.Enums;

public class Brief : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string RawContent { get; set; } = string.Empty; // Original brief text from client

    // AI Analysis Results
    public string? AnalyzedContent { get; set; } // Structured analysis from Claude
    public string? ProjectType { get; set; } // e.g., "Web Development", "Mobile App"
    public string? Industry { get; set; } // e.g., "Healthcare", "E-commerce"
    public decimal? EstimatedBudget { get; set; }
    public string? Timeline { get; set; } // e.g., "2-3 months"
    public string? KeyRequirements { get; set; } // JSON array of requirements
    public string? TechnicalRequirements { get; set; } // JSON array
    public string? TargetAudience { get; set; }

    // Status
    public BriefStatus Status { get; set; } = BriefStatus.Draft;
    public DateTime? AnalyzedAt { get; set; }

    // AI Metadata
    public int? TokensUsed { get; set; }
    public decimal? AnalysisCost { get; set; }

    // Navigation properties
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
}
