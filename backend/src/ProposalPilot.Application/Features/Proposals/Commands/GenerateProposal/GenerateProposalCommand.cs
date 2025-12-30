namespace ProposalPilot.Application.Features.Proposals.Commands.GenerateProposal;

using MediatR;
using ProposalPilot.Shared.DTOs.Proposal;

public record GenerateProposalCommand(
    Guid UserId,
    Guid BriefId,
    Guid ClientId,
    string? PreferredTone = null,
    string? ProposalLength = null,
    Guid? TemplateId = null
) : IRequest<Guid>; // Returns the new Proposal ID
