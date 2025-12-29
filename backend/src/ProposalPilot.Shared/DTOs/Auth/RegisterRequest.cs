namespace ProposalPilot.Shared.DTOs.Auth;

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? CompanyName = null
);
