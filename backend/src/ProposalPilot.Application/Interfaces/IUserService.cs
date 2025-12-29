using ProposalPilot.Domain.Entities;
using ProposalPilot.Shared.DTOs.User;

namespace ProposalPilot.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
    Task<UserProfileDto?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<bool> UpdateProfileImageAsync(Guid userId, string imageUrl);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByEmailAsync(string email);
}
