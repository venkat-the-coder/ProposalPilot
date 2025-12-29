namespace ProposalPilot.Domain.Entities;

public class ProposalSection : BaseEntity
{
    public Guid ProposalId { get; set; }
    public Proposal Proposal { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}
