using MediatR;

namespace ProposalPilot.Application.Features.Users.Commands.SendEmailVerification;

public record SendEmailVerificationCommand(Guid UserId) : IRequest<bool>;
