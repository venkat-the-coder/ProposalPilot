using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.DTOs.User;

namespace ProposalPilot.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        ApplicationDbContext context,
        IAuthService authService,
        ILogger<UserService> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return null;
        }

        return new UserProfileDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.CompanyName,
            user.JobTitle,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.EmailConfirmed,
            user.LastLoginAt,
            user.CreatedAt
        );
    }

    public async Task<UserProfileDto?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return null;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.CompanyName = request.CompanyName;
        user.JobTitle = request.JobTitle;
        user.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Profile updated for user: {UserId}", userId);

        return new UserProfileDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.CompanyName,
            user.JobTitle,
            user.PhoneNumber,
            user.ProfileImageUrl,
            user.EmailConfirmed,
            user.LastLoginAt,
            user.CreatedAt
        );
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        // Verify current password
        if (!_authService.VerifyPassword(currentPassword, user.PasswordHash))
        {
            _logger.LogWarning("Invalid current password for user: {UserId}", userId);
            return false;
        }

        // Update password
        user.PasswordHash = _authService.HashPassword(newPassword);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Password changed for user: {UserId}", userId);

        return true;
    }

    public async Task<bool> UpdateProfileImageAsync(Guid userId, string imageUrl)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        user.ProfileImageUrl = imageUrl;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Profile image updated for user: {UserId}", userId);

        return true;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Email == email.ToLower());
    }
}
