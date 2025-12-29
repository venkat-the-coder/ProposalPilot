namespace ProposalPilot.Shared.DTOs.Brief;

public record BriefDto(
    Guid Id,
    string Title,
    string RawContent,
    string? AnalyzedContent,
    string? ProjectType,
    string? Industry,
    decimal? EstimatedBudget,
    string? Timeline,
    string? KeyRequirements,
    string? TechnicalRequirements,
    string? TargetAudience,
    string Status,
    DateTime? AnalyzedAt,
    int? TokensUsed,
    decimal? AnalysisCost,
    DateTime CreatedAt
);

public record CreateBriefRequest(
    string Title,
    string RawContent
);

public record AnalyzeBriefRequest(
    Guid BriefId
);

public record BriefAnalysisResult(
    string ProjectType,
    string Industry,
    decimal? EstimatedBudget,
    string Timeline,
    List<string> KeyRequirements,
    List<string> TechnicalRequirements,
    string TargetAudience,
    string Summary
);
