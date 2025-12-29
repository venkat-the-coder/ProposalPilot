using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.DTOs.User;

namespace ProposalPilot.Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserProfileDto?>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UpdateProfileCommandHandler> _logger;

    public UpdateProfileCommandHandler(
        ApplicationDbContext context,
        ILogger<UpdateProfileCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserProfileDto?> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return null;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.CompanyName = request.CompanyName;
        user.JobTitle = request.JobTitle;
        user.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile updated for user: {UserId}", request.UserId);

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
}
