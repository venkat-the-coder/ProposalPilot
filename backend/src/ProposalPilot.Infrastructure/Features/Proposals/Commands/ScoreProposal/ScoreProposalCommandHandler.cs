using MediatR;
using Microsoft.EntityFrameworkCore;
using ProposalPilot.Application.Features.Proposals.Commands.ScoreProposal;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Infrastructure.Services;
using ProposalPilot.Shared.DTOs.Quality;

namespace ProposalPilot.Infrastructure.Features.Proposals.Commands.ScoreProposal;

public class ScoreProposalCommandHandler : IRequestHandler<ScoreProposalCommand, QualityScoreResult>
{
    private readonly ApplicationDbContext _context;
    private readonly IQualityScorerService _qualityScorerService;

    public ScoreProposalCommandHandler(
        ApplicationDbContext context,
        IQualityScorerService qualityScorerService)
    {
        _context = context;
        _qualityScorerService = qualityScorerService;
    }

    public async Task<QualityScoreResult> Handle(ScoreProposalCommand request, CancellationToken cancellationToken)
    {
        // Get proposal with related brief
        var proposal = await _context.Proposals
            .Include(p => p.Brief)
            .FirstOrDefaultAsync(p => p.Id == request.ProposalId && p.UserId == request.UserId, cancellationToken);

        if (proposal == null)
        {
            throw new InvalidOperationException("Proposal not found");
        }

        if (proposal.Brief == null || string.IsNullOrEmpty(proposal.Brief.RawContent))
        {
            throw new InvalidOperationException("Brief content not found");
        }

        if (string.IsNullOrEmpty(proposal.Brief.AnalyzedContent))
        {
            throw new InvalidOperationException("Brief must be analyzed before scoring proposal");
        }

        if (string.IsNullOrEmpty(proposal.DeliverablesJson))
        {
            throw new InvalidOperationException("Proposal content not found");
        }

        // Call Quality Scorer Service
        var scoreResult = await _qualityScorerService.ScoreProposalAsync(
            proposal.Brief.RawContent,
            proposal.Brief.AnalyzedContent,
            proposal.DeliverablesJson,
            cancellationToken
        );

        return scoreResult;
    }
}
