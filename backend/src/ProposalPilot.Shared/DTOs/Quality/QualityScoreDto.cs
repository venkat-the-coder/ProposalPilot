namespace ProposalPilot.Shared.DTOs.Quality;

public record QualityScoreResult(
    int OverallScore,
    string Grade,
    string WinProbability,
    Dictionary<string, CategoryScore> Scores,
    List<string> Strengths,
    List<Improvement> Improvements,
    List<string> QuickWins,
    Dictionary<string, string>? RewriteSuggestions
);

public record CategoryScore(
    int Score,
    int Max,
    string Feedback
);

public record Improvement(
    string Priority,
    string Section,
    string Issue,
    string Suggestion
);

public record ScoreProposalRequest(
    Guid ProposalId
);
