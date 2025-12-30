namespace ProposalPilot.Application.Interfaces;

using ProposalPilot.Domain.Entities;
using ProposalPilot.Shared.DTOs.Brief;
using ProposalPilot.Shared.DTOs.Proposal;

public interface IProposalGeneratorService
{
    Task<ProposalGenerationResult> GenerateProposalAsync(
        BriefAnalysisResult briefAnalysis,
        string userName,
        string? companyName = null,
        decimal? hourlyRate = null,
        string? preferredTone = null,
        string? proposalLength = null,
        ProposalTemplate? template = null,
        CancellationToken cancellationToken = default);
}
