namespace ProposalPilot.Domain.Entities;

public class Client : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public string? Notes { get; set; }

    // User who owns this client
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation properties
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
}
