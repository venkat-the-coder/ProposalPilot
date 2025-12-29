using MediatR;
using ProposalPilot.Shared.DTOs.User;

namespace ProposalPilot.Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? CompanyName,
    string? JobTitle,
    string? PhoneNumber
) : IRequest<UserProfileDto?>;
