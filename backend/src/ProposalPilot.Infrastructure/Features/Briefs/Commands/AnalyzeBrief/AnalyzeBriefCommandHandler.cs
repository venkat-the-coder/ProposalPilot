namespace ProposalPilot.Infrastructure.Features.Briefs.Commands.AnalyzeBrief;

using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Features.Briefs.Commands.AnalyzeBrief;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Domain.Enums;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.DTOs.Brief;

public class AnalyzeBriefCommandHandler : IRequestHandler<AnalyzeBriefCommand, BriefDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IBriefAnalyzerService _briefAnalyzerService;
    private readonly IClaudeApiService _claudeApiService;
    private readonly ILogger<AnalyzeBriefCommandHandler> _logger;

    public AnalyzeBriefCommandHandler(
        ApplicationDbContext context,
        IBriefAnalyzerService briefAnalyzerService,
        IClaudeApiService claudeApiService,
        ILogger<AnalyzeBriefCommandHandler> logger)
    {
        _context = context;
        _briefAnalyzerService = briefAnalyzerService;
        _claudeApiService = claudeApiService;
        _logger = logger;
    }

    public async Task<BriefDto> Handle(AnalyzeBriefCommand request, CancellationToken cancellationToken)
    {
        // Find the brief
        var brief = await _context.Briefs
            .FirstOrDefaultAsync(b => b.Id == request.BriefId && b.UserId == request.UserId, cancellationToken);

        if (brief == null)
        {
            throw new InvalidOperationException($"Brief with ID {request.BriefId} not found");
        }

        try
        {
            // Update status to Analyzing
            brief.Status = BriefStatus.Analyzing;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Starting analysis for brief {BriefId}", brief.Id);

            // Analyze the brief
            var analysis = await _briefAnalyzerService.AnalyzeBriefAsync(
                brief.RawContent,
                clientName: null,
                industry: brief.Industry,
                cancellationToken: cancellationToken
            );

            // Serialize the full analysis result
            var analyzedContentJson = JsonSerializer.Serialize(analysis, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Update the brief with analysis results
            brief.AnalyzedContent = analyzedContentJson;
            brief.ProjectType = analysis.ProjectOverview.Type;
            brief.Industry = analysis.ProjectOverview.Industry;
            brief.Timeline = analysis.ProjectSignals.Timeline.DurationEstimate;
            brief.Status = BriefStatus.Analyzed;
            brief.AnalyzedAt = DateTime.UtcNow;

            // Extract key requirements and technical requirements
            if (analysis.Requirements.Explicit.Any() || analysis.Requirements.Implicit.Any())
            {
                var allRequirements = analysis.Requirements.Explicit
                    .Concat(analysis.Requirements.Implicit)
                    .ToList();
                brief.KeyRequirements = JsonSerializer.Serialize(allRequirements);
            }

            if (analysis.Requirements.Technical.Any())
            {
                brief.TechnicalRequirements = JsonSerializer.Serialize(analysis.Requirements.Technical);
            }

            // Extract target audience from client insights
            if (analysis.ClientInsights.SuccessCriteria.Any())
            {
                brief.TargetAudience = JsonSerializer.Serialize(analysis.ClientInsights.SuccessCriteria);
            }

            // Estimate token usage and cost (this would come from the actual API call)
            // For now, we'll estimate based on content length
            var estimatedTokens = _claudeApiService.EstimateTokenCount(brief.RawContent);
            brief.TokensUsed = estimatedTokens + 1000; // Input + output estimate
            brief.AnalysisCost = _claudeApiService.CalculateCost(
                estimatedTokens,
                1000,
                "claude-haiku-4-5-20251001"
            );

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully analyzed brief {BriefId} with {TokensUsed} tokens",
                brief.Id, brief.TokensUsed);

            return new BriefDto(
                brief.Id,
                brief.Title,
                brief.RawContent,
                brief.AnalyzedContent,
                brief.ProjectType,
                brief.Industry,
                brief.EstimatedBudget,
                brief.Timeline,
                brief.KeyRequirements,
                brief.TechnicalRequirements,
                brief.TargetAudience,
                brief.Status.ToString(),
                brief.AnalyzedAt,
                brief.TokensUsed,
                brief.AnalysisCost,
                brief.CreatedAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing brief {BriefId}", brief.Id);

            // Update status to Failed
            brief.Status = BriefStatus.Failed;
            await _context.SaveChangesAsync(cancellationToken);

            throw;
        }
    }
}
