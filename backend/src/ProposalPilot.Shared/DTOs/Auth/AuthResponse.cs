namespace ProposalPilot.Shared.DTOs.Auth;

public record AuthResponse(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string? CompanyName,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
