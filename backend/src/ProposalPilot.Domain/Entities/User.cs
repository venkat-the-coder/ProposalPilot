namespace ProposalPilot.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? JobTitle { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime? EmailConfirmedAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Subscription
    public Guid? SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }

    // Navigation properties
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
