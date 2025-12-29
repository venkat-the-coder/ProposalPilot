using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Infrastructure.Data;

namespace ProposalPilot.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        ApplicationDbContext context,
        IAuthService authService,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return false;
        }

        // Verify current password
        if (!_authService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            _logger.LogWarning("Invalid current password for user: {UserId}", request.UserId);
            return false;
        }

        // Update password
        user.PasswordHash = _authService.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password changed for user: {UserId}", request.UserId);

        return true;
    }
}
