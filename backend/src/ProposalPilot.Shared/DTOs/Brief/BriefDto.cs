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

// Comprehensive Brief Analysis Result from Claude API
public record BriefAnalysisResult(
    ProjectOverview ProjectOverview,
    Requirements Requirements,
    ClientInsights ClientInsights,
    ProjectSignals ProjectSignals,
    RiskAssessment RiskAssessment,
    RecommendedApproach RecommendedApproach
);

public record ProjectOverview(
    string Type,
    string Industry,
    string Complexity,
    int ConfidenceScore
);

public record Requirements(
    List<string> Explicit,
    List<string> Implicit,
    List<string> Technical,
    List<string> Deliverables
);

public record ClientInsights(
    List<string> PainPoints,
    List<string> SuccessCriteria,
    List<string> DecisionFactors
);

public record ProjectSignals(
    TimelineInfo Timeline,
    BudgetInfo Budget
);

public record TimelineInfo(
    string Urgency,
    string DurationEstimate,
    List<string> KeyDates
);

public record BudgetInfo(
    List<string> Signals,
    string RangeEstimate,
    string PricingSensitivity
);

public record RiskAssessment(
    List<string> RedFlags,
    List<string> ClarificationNeeded,
    List<string> ScopeCreepRisks
);

public record RecommendedApproach(
    string ProposalTone,
    List<string> KeyThemes,
    List<string> Differentiators,
    string PricingStrategy
);
