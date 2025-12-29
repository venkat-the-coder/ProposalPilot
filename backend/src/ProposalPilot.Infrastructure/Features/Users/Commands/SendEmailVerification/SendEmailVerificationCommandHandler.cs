using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Features.Users.Commands.SendEmailVerification;
using ProposalPilot.Infrastructure.Data;

namespace ProposalPilot.Infrastructure.Features.Users.Commands.SendEmailVerification;

public class SendEmailVerificationCommandHandler : IRequestHandler<SendEmailVerificationCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SendEmailVerificationCommandHandler> _logger;

    public SendEmailVerificationCommandHandler(
        ApplicationDbContext context,
        ILogger<SendEmailVerificationCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
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

        // Generate verification token (in a real app, this would be sent via email)
        var verificationToken = Guid.NewGuid().ToString("N");

        // Store token in RefreshToken field temporarily (in production, use a separate table)
        user.RefreshToken = verificationToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddHours(24);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email verification token generated for user: {UserId}", request.UserId);
        _logger.LogInformation("Verification link (dev): /verify-email?token={Token}&userId={UserId}", verificationToken, request.UserId);

        // TODO: Send email via SendGrid in production
        // await _emailService.SendVerificationEmail(user.Email, verificationToken);

        return true;
    }
}
