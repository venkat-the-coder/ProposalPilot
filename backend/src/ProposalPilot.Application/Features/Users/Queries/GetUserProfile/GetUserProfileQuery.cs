using MediatR;
using ProposalPilot.Shared.DTOs.User;

namespace ProposalPilot.Application.Features.Users.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto?>;
