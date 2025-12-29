using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Shared.DTOs.Claude;
using ProposalPilot.Shared.DTOs.Quality;

namespace ProposalPilot.Infrastructure.Services;

public interface IQualityScorerService
{
    Task<QualityScoreResult> ScoreProposalAsync(
        string briefText,
        string briefAnalysisJson,
        string proposalJson,
        CancellationToken cancellationToken = default);
}

public class QualityScorerService : IQualityScorerService
{
    private readonly IClaudeApiService _claudeApiService;
    private readonly ILogger<QualityScorerService> _logger;
    private const string SystemPrompt = @"You are a proposal quality assessor. Your job is to evaluate a proposal and provide a score with specific improvement suggestions.

SCORING CRITERIA (100 points total):

1. **Relevance (25 points)**
   - Does it address the client's specific needs?
   - Is content tailored or generic?
   - Does it reference their pain points?

2. **Persuasiveness (25 points)**
   - Is the value proposition clear?
   - Are benefits emphasized over features?
   - Is there social proof?

3. **Clarity (20 points)**
   - Is it easy to understand?
   - Is the structure logical?
   - Is pricing clear?

4. **Professionalism (15 points)**
   - Grammar and spelling
   - Formatting consistency
   - Appropriate tone

5. **Actionability (15 points)**
   - Clear next steps?
   - Easy to say yes?
   - Creates appropriate urgency?

OUTPUT FORMAT (JSON):
{
  ""overall_score"": 0,
  ""grade"": ""A+ | A | B | C | D | F"",
  ""win_probability"": ""low | medium | high | very_high"",
  ""scores"": {
    ""relevance"": {""score"": 0, ""max"": 25, ""feedback"": ""...""},
    ""persuasiveness"": {""score"": 0, ""max"": 25, ""feedback"": ""...""},
    ""clarity"": {""score"": 0, ""max"": 20, ""feedback"": ""...""},
    ""professionalism"": {""score"": 0, ""max"": 15, ""feedback"": ""...""},
    ""actionability"": {""score"": 0, ""max"": 15, ""feedback"": ""...""}
  },
  ""strengths"": [""What's working well""],
  ""improvements"": [
    {
      ""priority"": ""critical | high | medium | low"",
      ""section"": ""Which section"",
      ""issue"": ""What's wrong"",
      ""suggestion"": ""How to fix it""
    }
  ],
  ""quick_wins"": [""Easy improvements with high impact""],
  ""rewrite_suggestions"": {
    ""section_name"": ""Suggested rewrite for specific sections""
  }
}

GRADING SCALE:
- A+ (95-100): Exceptional, ready to send
- A (85-94): Strong, minor tweaks only
- B (75-84): Good, needs some improvement
- C (65-74): Average, significant improvements needed
- D (50-64): Below average, major revision required
- F (0-49): Poor, consider starting over";

    public QualityScorerService(
        IClaudeApiService claudeApiService,
        ILogger<QualityScorerService> logger)
    {
        _claudeApiService = claudeApiService;
        _logger = logger;
    }

    public async Task<QualityScoreResult> ScoreProposalAsync(
        string briefText,
        string briefAnalysisJson,
        string proposalJson,
        CancellationToken cancellationToken = default)
    {
        var userMessage = $@"Score this proposal:

ORIGINAL BRIEF:
{briefText}

BRIEF ANALYSIS:
{briefAnalysisJson}

PROPOSAL CONTENT:
{proposalJson}";

        var response = await _claudeApiService.SendMessageAsync(userMessage, SystemPrompt, cancellationToken);

        if (response.Content == null || response.Content.Count == 0)
        {
            throw new InvalidOperationException("Failed to get quality score from Claude API");
        }

        var jsonResponse = response.Content[0].Text;

        _logger.LogInformation("Received quality score response from Claude API");

        // Parse JSON response
        var scoreData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

        var scores = new Dictionary<string, CategoryScore>();
        if (scoreData.TryGetProperty("scores", out var scoresElement))
        {
            foreach (var prop in scoresElement.EnumerateObject())
            {
                var scoreObj = prop.Value;
                scores[prop.Name] = new CategoryScore(
                    scoreObj.GetProperty("score").GetInt32(),
                    scoreObj.GetProperty("max").GetInt32(),
                    scoreObj.GetProperty("feedback").GetString() ?? ""
                );
            }
        }

        var improvements = new List<Improvement>();
        if (scoreData.TryGetProperty("improvements", out var improvementsElement))
        {
            foreach (var improvement in improvementsElement.EnumerateArray())
            {
                improvements.Add(new Improvement(
                    improvement.GetProperty("priority").GetString() ?? "medium",
                    improvement.GetProperty("section").GetString() ?? "",
                    improvement.GetProperty("issue").GetString() ?? "",
                    improvement.GetProperty("suggestion").GetString() ?? ""
                ));
            }
        }

        var strengths = new List<string>();
        if (scoreData.TryGetProperty("strengths", out var strengthsElement))
        {
            foreach (var strength in strengthsElement.EnumerateArray())
            {
                strengths.Add(strength.GetString() ?? "");
            }
        }

        var quickWins = new List<string>();
        if (scoreData.TryGetProperty("quick_wins", out var quickWinsElement))
        {
            foreach (var quickWin in quickWinsElement.EnumerateArray())
            {
                quickWins.Add(quickWin.GetString() ?? "");
            }
        }

        Dictionary<string, string>? rewriteSuggestions = null;
        if (scoreData.TryGetProperty("rewrite_suggestions", out var rewriteElement))
        {
            rewriteSuggestions = new Dictionary<string, string>();
            foreach (var prop in rewriteElement.EnumerateObject())
            {
                rewriteSuggestions[prop.Name] = prop.Value.GetString() ?? "";
            }
        }

        return new QualityScoreResult(
            scoreData.GetProperty("overall_score").GetInt32(),
            scoreData.GetProperty("grade").GetString() ?? "C",
            scoreData.GetProperty("win_probability").GetString() ?? "medium",
            scores,
            strengths,
            improvements,
            quickWins,
            rewriteSuggestions
        );
    }
}
