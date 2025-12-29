namespace ProposalPilot.Infrastructure.Services;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Shared.DTOs.Brief;
using ProposalPilot.Shared.DTOs.Claude;

public class BriefAnalyzerService : IBriefAnalyzerService
{
    private readonly IClaudeApiService _claudeApiService;
    private readonly ILogger<BriefAnalyzerService> _logger;

    private const string BRIEF_ANALYZER_SYSTEM_PROMPT = @"You are an expert proposal consultant with 20 years of experience analyzing client briefs and RFPs. Your task is to analyze the provided client brief and extract structured information that will help create a winning proposal.

Analyze the brief carefully and extract:

1. **Project Overview**: What type of project is this? What industry? How complex?
2. **Requirements**: Both explicit (stated) and implicit (implied) requirements
3. **Client Insights**: What are their pain points? What does success look like to them?
4. **Project Signals**: Any timeline urgency? Budget indicators?
5. **Risk Assessment**: Red flags or concerns? Questions that need clarification?
6. **Recommended Approach**: How should we position the proposal?

OUTPUT FORMAT (JSON):
{
  ""project_overview"": {
    ""type"": ""string - project type (e.g., 'Web Application', 'Mobile App', 'Consulting')"",
    ""industry"": ""string - client's industry"",
    ""complexity"": ""low | medium | high | enterprise"",
    ""confidence_score"": ""number 0-100 - how confident are you in this analysis""
  },
  ""requirements"": {
    ""explicit"": [""array of clearly stated requirements""],
    ""implicit"": [""array of implied/assumed requirements""],
    ""technical"": [""array of technical specifications mentioned""],
    ""deliverables"": [""array of expected outputs/deliverables""]
  },
  ""client_insights"": {
    ""pain_points"": [""array of problems they're trying to solve""],
    ""success_criteria"": [""array of what success looks like to them""],
    ""decision_factors"": [""array of what will influence their decision""]
  },
  ""project_signals"": {
    ""timeline"": {
      ""urgency"": ""low | medium | high"",
      ""duration_estimate"": ""string - estimated project duration"",
      ""key_dates"": [""array of any mentioned deadlines""]
    },
    ""budget"": {
      ""signals"": [""array of budget indicators found""],
      ""range_estimate"": ""string - estimated budget range if possible"",
      ""pricing_sensitivity"": ""low | medium | high""
    }
  },
  ""risk_assessment"": {
    ""red_flags"": [""array of concerns or warning signs""],
    ""clarification_needed"": [""array of questions to ask before proceeding""],
    ""scope_creep_risks"": [""array of areas where scope might expand""]
  },
  ""recommended_approach"": {
    ""proposal_tone"": ""formal | professional | friendly | consultative"",
    ""key_themes"": [""array of themes to emphasize in proposal""],
    ""differentiators"": [""array of how to stand out""],
    ""pricing_strategy"": ""value_based | competitive | premium""
  }
}

IMPORTANT:
- Be thorough but concise
- If information is not available, make reasonable assumptions and note them
- Flag anything that seems unusual or risky
- Focus on actionable insights that will help win this project";

    public BriefAnalyzerService(
        IClaudeApiService claudeApiService,
        ILogger<BriefAnalyzerService> logger)
    {
        _claudeApiService = claudeApiService;
        _logger = logger;
    }

    public async Task<BriefAnalysisResult> AnalyzeBriefAsync(
        string briefContent,
        string? clientName = null,
        string? industry = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Analyzing brief with {Length} characters", briefContent.Length);

            // Build user message
            var userMessage = $@"Please analyze this client brief:

---
{briefContent}
---";

            if (!string.IsNullOrEmpty(clientName) || !string.IsNullOrEmpty(industry))
            {
                userMessage += "\n\nAdditional context:";
                if (!string.IsNullOrEmpty(clientName))
                    userMessage += $"\n- Client Name: {clientName}";
                if (!string.IsNullOrEmpty(industry))
                    userMessage += $"\n- Client Industry: {industry}";
            }

            // Call Claude API
            var response = await _claudeApiService.SendMessageAsync(
                message: userMessage,
                systemPrompt: BRIEF_ANALYZER_SYSTEM_PROMPT,
                cancellationToken: cancellationToken
            );

            // Extract JSON from response
            var analysisJson = response.Content.FirstOrDefault()?.Text ?? string.Empty;

            _logger.LogInformation("Received analysis response with {TokensUsed} tokens",
                (response.Usage?.InputTokens ?? 0) + (response.Usage?.OutputTokens ?? 0));

            // Parse the JSON response with custom naming policy
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var result = JsonSerializer.Deserialize<BriefAnalysisResult>(analysisJson, options);

            if (result == null)
            {
                _logger.LogError("Failed to deserialize brief analysis response");
                throw new InvalidOperationException("Failed to parse AI response");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing brief");
            throw;
        }
    }
}
