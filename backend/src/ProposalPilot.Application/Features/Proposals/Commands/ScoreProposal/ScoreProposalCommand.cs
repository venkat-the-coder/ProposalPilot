using MediatR;
using ProposalPilot.Shared.DTOs.Quality;

namespace ProposalPilot.Application.Features.Proposals.Commands.ScoreProposal;

public record ScoreProposalCommand(
    Guid ProposalId,
    Guid UserId
) : IRequest<QualityScoreResult>;
