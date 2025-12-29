using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Infrastructure.Data;

namespace ProposalPilot.Application.Features.Users.Commands.UpdateProfileImage;

public class UpdateProfileImageCommandHandler : IRequestHandler<UpdateProfileImageCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UpdateProfileImageCommandHandler> _logger;

    public UpdateProfileImageCommandHandler(
        ApplicationDbContext context,
        ILogger<UpdateProfileImageCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return false;
        }

        user.ProfileImageUrl = request.ImageUrl;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile image updated for user: {UserId}", request.UserId);

        return true;
    }
}
