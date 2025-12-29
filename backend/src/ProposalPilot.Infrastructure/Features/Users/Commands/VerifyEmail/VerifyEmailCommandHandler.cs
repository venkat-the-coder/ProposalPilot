using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Features.Users.Commands.VerifyEmail;
using ProposalPilot.Infrastructure.Data;

namespace ProposalPilot.Infrastructure.Features.Users.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VerifyEmailCommandHandler> _logger;

    public VerifyEmailCommandHandler(
        ApplicationDbContext context,
        ILogger<VerifyEmailCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return false;
        }

        if (user.EmailConfirmed)
        {
            _logger.LogInformation("Email already verified for user: {UserId}", request.UserId);
            return true;
        }

        // Verify token
        if (user.RefreshToken != request.Token)
        {
            _logger.LogWarning("Invalid verification token for user: {UserId}", request.UserId);
            return false;
        }

        // Check if token expired
        if (user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Verification token expired for user: {UserId}", request.UserId);
            return false;
        }

        // Mark email as verified
        user.EmailConfirmed = true;
        user.EmailConfirmedAt = DateTime.UtcNow;
        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email verified successfully for user: {UserId}", request.UserId);

        return true;
    }
}
