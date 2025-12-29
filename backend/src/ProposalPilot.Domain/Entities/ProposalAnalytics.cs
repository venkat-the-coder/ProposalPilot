namespace ProposalPilot.Domain.Entities;

public class ProposalAnalytics : BaseEntity
{
    public Guid ProposalId { get; set; }
    public Proposal Proposal { get; set; } = null!;

    public DateTime ViewedAt { get; set; }
    public string? ViewerIpAddress { get; set; }
    public string? ViewerUserAgent { get; set; }
    public string? ViewerCountry { get; set; }
    public string? ViewerCity { get; set; }

    // Section-level analytics
    public string? SectionId { get; set; }
    public int? TimeSpentSeconds { get; set; }

    // Scroll depth percentage (0-100)
    public int? ScrollDepth { get; set; }
}
