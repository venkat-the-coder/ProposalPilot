using MediatR;

namespace ProposalPilot.Application.Features.Users.Commands.UpdateProfileImage;

public record UpdateProfileImageCommand(
    Guid UserId,
    string ImageUrl
) : IRequest<bool>;
