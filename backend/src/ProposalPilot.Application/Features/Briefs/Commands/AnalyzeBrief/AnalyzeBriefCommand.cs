namespace ProposalPilot.Application.Features.Briefs.Commands.AnalyzeBrief;

using MediatR;
using ProposalPilot.Shared.DTOs.Brief;

public record AnalyzeBriefCommand(
    Guid BriefId,
    Guid UserId
) : IRequest<BriefDto>;
