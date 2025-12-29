using MediatR;

namespace ProposalPilot.Application.Features.Users.Commands.VerifyEmail;

public record VerifyEmailCommand(Guid UserId, string Token) : IRequest<bool>;
