namespace ProposalPilot.Shared.DTOs.User;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
