using ProposalPilot.Domain.Entities;

namespace ProposalPilot.Application.Interfaces;

public interface IAuthService
{
    Task<(User user, string accessToken, string refreshToken)> RegisterAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        string? companyName = null);

    Task<(User user, string accessToken, string refreshToken)?> LoginAsync(
        string email,
        string password);

    Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken);

    Task<bool> RevokeTokenAsync(Guid userId);

    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    string HashPassword(string password);

    bool VerifyPassword(string password, string hash);
}
