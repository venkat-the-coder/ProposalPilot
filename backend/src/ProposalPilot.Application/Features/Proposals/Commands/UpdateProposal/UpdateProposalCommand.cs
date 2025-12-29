namespace ProposalPilot.Application.Features.Proposals.Commands.UpdateProposal;

using MediatR;

public record UpdateProposalCommand(
    Guid ProposalId,
    Guid UserId,
    string? Title = null,
    string? Description = null,
    Dictionary<string, string>? Sections = null
) : IRequest<bool>;
