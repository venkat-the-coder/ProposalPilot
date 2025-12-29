namespace ProposalPilot.Application.Features.Briefs.Queries.GetBrief;

using MediatR;
using ProposalPilot.Shared.DTOs.Brief;

public record GetBriefQuery(
    Guid BriefId,
    Guid UserId
) : IRequest<BriefDto?>;
