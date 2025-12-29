namespace ProposalPilot.Application.Interfaces;

using ProposalPilot.Shared.DTOs.Brief;

public interface IBriefAnalyzerService
{
    Task<BriefAnalysisResult> AnalyzeBriefAsync(
        string briefContent,
        string? clientName = null,
        string? industry = null,
        CancellationToken cancellationToken = default);
}
