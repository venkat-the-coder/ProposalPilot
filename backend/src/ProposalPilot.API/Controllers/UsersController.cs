using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Shared.DTOs.User;

namespace ProposalPilot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ICurrentUserService currentUserService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's profile
    /// </summary>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        try
        {
            if (!_currentUserService.UserId.HasValue)
            {
                _logger.LogWarning("Unauthorized access attempt to get profile");
                return Unauthorized(new { message = "User not authenticated" });
            }

            var profile = await _userService.GetUserProfileAsync(_currentUserService.UserId.Value);

            if (profile == null)
            {
                _logger.LogWarning("Profile not found for user: {UserId}", _currentUserService.UserId);
                return NotFound(new { message = "Profile not found" });
            }

            _logger.LogInformation("Profile retrieved for user: {UserId}", _currentUserService.UserId);
            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving profile for user: {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while retrieving the profile" });
        }
    }

    /// <summary>
    /// Update current user's profile
    /// </summary>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            if (!_currentUserService.UserId.HasValue)
            {
                _logger.LogWarning("Unauthorized access attempt to update profile");
                return Unauthorized(new { message = "User not authenticated" });
            }

            var updatedProfile = await _userService.UpdateProfileAsync(
                _currentUserService.UserId.Value,
                request);

            if (updatedProfile == null)
            {
                _logger.LogWarning("Profile not found for user: {UserId}", _currentUserService.UserId);
                return NotFound(new { message = "Profile not found" });
            }

            _logger.LogInformation("Profile updated for user: {UserId}", _currentUserService.UserId);
            return Ok(updatedProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user: {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while updating the profile" });
        }
    }

    /// <summary>
    /// Change current user's password
    /// </summary>
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            if (!_currentUserService.UserId.HasValue)
            {
                _logger.LogWarning("Unauthorized access attempt to change password");
                return Unauthorized(new { message = "User not authenticated" });
            }

            var success = await _userService.ChangePasswordAsync(
                _currentUserService.UserId.Value,
                request.CurrentPassword,
                request.NewPassword);

            if (!success)
            {
                _logger.LogWarning("Failed to change password for user: {UserId}", _currentUserService.UserId);
                return BadRequest(new { message = "Current password is incorrect" });
            }

            _logger.LogInformation("Password changed for user: {UserId}", _currentUserService.UserId);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while changing the password" });
        }
    }

    /// <summary>
    /// Update profile image URL
    /// </summary>
    [HttpPut("profile/image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfileImage([FromBody] UpdateProfileImageRequest request)
    {
        try
        {
            if (!_currentUserService.UserId.HasValue)
            {
                _logger.LogWarning("Unauthorized access attempt to update profile image");
                return Unauthorized(new { message = "User not authenticated" });
            }

            var success = await _userService.UpdateProfileImageAsync(
                _currentUserService.UserId.Value,
                request.ImageUrl);

            if (!success)
            {
                _logger.LogWarning("Failed to update profile image for user: {UserId}", _currentUserService.UserId);
                return BadRequest(new { message = "Failed to update profile image" });
            }

            _logger.LogInformation("Profile image updated for user: {UserId}", _currentUserService.UserId);
            return Ok(new { message = "Profile image updated successfully", imageUrl = request.ImageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile image for user: {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while updating the profile image" });
        }
    }
}

public record UpdateProfileImageRequest(string ImageUrl);
