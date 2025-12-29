namespace ProposalPilot.Shared.DTOs.User;

public record UserProfileDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? CompanyName,
    string? JobTitle,
    string? PhoneNumber,
    string? ProfileImageUrl,
    bool EmailConfirmed,
    DateTime? LastLoginAt,
    DateTime CreatedAt
);
