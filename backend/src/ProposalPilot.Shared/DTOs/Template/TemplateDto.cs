namespace ProposalPilot.Shared.DTOs.Template;

/// <summary>
/// Complete template details
/// </summary>
public class TemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public TemplateContentDto Content { get; set; } = new();
    public PricingTemplateDto? DefaultPricing { get; set; }
    public bool IsSystemTemplate { get; set; }
    public bool IsPublic { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public int UsageCount { get; set; }
    public decimal? WinRate { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? EstimatedTimeMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Template content structure
/// </summary>
public class TemplateContentDto
{
    public string Introduction { get; set; } = string.Empty;
    public string ProblemStatement { get; set; } = string.Empty;
    public string ProposedSolution { get; set; } = string.Empty;
    public string Methodology { get; set; } = string.Empty;
    public string Deliverables { get; set; } = string.Empty;
    public string Timeline { get; set; } = string.Empty;
    public string TeamAndExperience { get; set; } = string.Empty;
    public string TermsAndConditions { get; set; } = string.Empty;
    public string CallToAction { get; set; } = string.Empty;
}

/// <summary>
/// Pricing template structure (3-tier)
/// </summary>
public class PricingTemplateDto
{
    public PricingTierDto Basic { get; set; } = new();
    public PricingTierDto Standard { get; set; } = new();
    public PricingTierDto Premium { get; set; } = new();
}

/// <summary>
/// Individual pricing tier
/// </summary>
public class PricingTierDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PriceMin { get; set; }
    public decimal PriceMax { get; set; }
    public List<string> Features { get; set; } = new();
    public string Timeline { get; set; } = string.Empty;
}
