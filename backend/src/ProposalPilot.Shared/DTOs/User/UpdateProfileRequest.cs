namespace ProposalPilot.Shared.DTOs.User;

public record UpdateProfileRequest(
    string FirstName,
    string LastName,
    string? CompanyName,
    string? JobTitle,
    string? PhoneNumber
);
