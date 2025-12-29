using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.Configuration;

namespace ProposalPilot.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        ApplicationDbContext context,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<(User user, string accessToken, string refreshToken)> RegisterAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        string? companyName = null)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        // Create new user
        var user = new User
        {
            Email = email.ToLower(),
            PasswordHash = HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            CompanyName = companyName,
            EmailConfirmed = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Generate tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (user, accessToken, refreshToken);
    }

    public async Task<(User user, string accessToken, string refreshToken)?> LoginAsync(
        string email,
        string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("User account is deactivated.");
        }

        // Generate new tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        // Update refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (user, accessToken, refreshToken);
    }

    public async Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDeleted);

        if (user == null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("User account is deactivated.");
        }

        // Generate new tokens
        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        // Update refresh token
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }

    public async Task<bool> RevokeTokenAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return false;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string HashPassword(string password)
    {
        // Using BCrypt-like hashing with PBKDF2
        using var deriveBytes = new Rfc2898DeriveBytes(
            password,
            saltSize: 16,
            iterations: 10000,
            HashAlgorithmName.SHA256);

        var salt = deriveBytes.Salt;
        var hash = deriveBytes.GetBytes(32);

        var hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var hashBytes = Convert.FromBase64String(hash);

        var salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        using var deriveBytes = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations: 10000,
            HashAlgorithmName.SHA256);

        var newHash = deriveBytes.GetBytes(32);

        for (int i = 0; i < 32; i++)
        {
            if (hashBytes[i + 16] != newHash[i])
            {
                return false;
            }
        }

        return true;
    }
}
