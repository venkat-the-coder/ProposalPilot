using MediatR;
using Microsoft.EntityFrameworkCore;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.DTOs.User;

namespace ProposalPilot.Application.Features.Users.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto?>
{
    private readonly ApplicationDbContext _context;

    public GetUserProfileQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileDto?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return null;

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
