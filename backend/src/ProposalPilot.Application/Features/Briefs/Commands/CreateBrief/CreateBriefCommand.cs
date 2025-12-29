namespace ProposalPilot.Application.Features.Briefs.Commands.CreateBrief;

using MediatR;
using ProposalPilot.Shared.DTOs.Brief;

public record CreateBriefCommand(
    Guid UserId,
    string Title,
    string RawContent
) : IRequest<BriefDto>;
