namespace ProposalPilot.Infrastructure.Services;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Shared.DTOs.Brief;
using ProposalPilot.Shared.DTOs.Proposal;

public class ProposalGeneratorService : IProposalGeneratorService
{
    private readonly IClaudeApiService _claudeApiService;
    private readonly ILogger<ProposalGeneratorService> _logger;

    private const string PROPOSAL_GENERATOR_SYSTEM_PROMPT = @"You are a world-class proposal writer who has helped win over $50M in contracts. Your proposals are known for being persuasive, personalized, and professional. You write in a way that makes clients feel deeply understood.

Your task is to generate a complete proposal based on the brief analysis and user profile provided.

PROPOSAL STRUCTURE:
Generate each section with compelling, personalized content:

1. **OPENING HOOK** (2-3 sentences)
   - Mirror the client's language and pain points
   - Show you understand their world
   - Create immediate connection

2. **PROBLEM STATEMENT** (1-2 paragraphs)
   - Articulate their problem better than they can
   - Show the cost of not solving it
   - Demonstrate deep understanding

3. **PROPOSED SOLUTION** (2-3 paragraphs)
   - Tailored approach to THEIR specific needs
   - Connect each element to their pain points
   - Focus on outcomes, not just activities

4. **METHODOLOGY** (structured phases)
   - Phase-by-phase breakdown
   - Clear milestones and deliverables
   - Show your proven process

5. **TIMELINE** (realistic schedule)
   - Week-by-week or phase-by-phase
   - Key milestones with dates
   - Buffer for reviews and revisions

6. **INVESTMENT OPTIONS** (3 tiers)
   - Basic: Core deliverables, essential scope
   - Recommended: Full scope as described (highlight this)
   - Premium: Enhanced scope with extras
   - Each tier with clear price and what's included

7. **WHY CHOOSE ME/US** (proof section)
   - Relevant experience and portfolio
   - Social proof (testimonials, case studies)
   - Unique selling points

8. **NEXT STEPS** (clear CTA)
   - Specific action to take
   - Create urgency without being pushy
   - Make it easy to say yes

OUTPUT FORMAT (JSON):
{
  ""title"": ""Proposal title"",
  ""sections"": {
    ""opening_hook"": ""<p>HTML formatted content</p>"",
    ""problem_statement"": ""<p>HTML formatted content</p>"",
    ""proposed_solution"": ""<p>HTML formatted content</p>"",
    ""methodology"": ""<p>HTML formatted with phases</p>"",
    ""timeline"": ""<p>HTML formatted timeline</p>"",
    ""investment"": {
      ""intro"": ""<p>Introduction to pricing</p>"",
      ""tiers"": [
        {
          ""name"": ""Basic"",
          ""price"": 0,
          ""description"": ""Core deliverables"",
          ""features"": [""feature 1"", ""feature 2""],
          ""timeline"": ""X weeks""
        },
        {
          ""name"": ""Recommended"",
          ""price"": 0,
          ""description"": ""Full scope"",
          ""features"": [""feature 1"", ""feature 2"", ""feature 3""],
          ""timeline"": ""X weeks"",
          ""highlighted"": true
        },
        {
          ""name"": ""Premium"",
          ""price"": 0,
          ""description"": ""Enhanced scope"",
          ""features"": [""feature 1"", ""feature 2"", ""feature 3"", ""feature 4""],
          ""timeline"": ""X weeks""
        }
      ]
    },
    ""why_choose_us"": ""<p>HTML formatted content</p>"",
    ""next_steps"": ""<p>HTML formatted content</p>""
  },
  ""metadata"": {
    ""word_count"": 0,
    ""estimated_read_time"": ""X minutes"",
    ""tone"": ""formal | professional | friendly""
  }
}

TONE GUIDELINES:
- Formal: Corporate, structured, third-person where appropriate
- Professional: Expert but approachable, first-person
- Friendly: Warm, conversational, collaborative
- Consultative: Advisory, thought leadership, strategic

IMPORTANT:
- Personalize EVERYTHING to this specific client
- Use their language and terminology
- Focus on outcomes and value, not features
- Be specific, not generic
- Sound human, not robotic
- Prices should be placeholders (0) - they'll be calculated separately";

    public ProposalGeneratorService(
        IClaudeApiService claudeApiService,
        ILogger<ProposalGeneratorService> logger)
    {
        _claudeApiService = claudeApiService;
        _logger = logger;
    }

    public async Task<ProposalGenerationResult> GenerateProposalAsync(
        BriefAnalysisResult briefAnalysis,
        string userName,
        string? companyName = null,
        decimal? hourlyRate = null,
        string? preferredTone = null,
        string? proposalLength = null,
        ProposalTemplate? template = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating proposal for user {UserName}", userName);

            // Serialize brief analysis
            var briefAnalysisJson = JsonSerializer.Serialize(briefAnalysis, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            // Build template section if provided
            var templateSection = template != null ? $@"

TEMPLATE TO FOLLOW:
Use this template as a structural guide for the proposal. Adapt the content to match the specific brief while maintaining the template's structure and tone:
- Name: {template.Name}
- Description: {template.Description}
- Category: {template.Category}
- Tags: {string.Join(", ", template.Tags ?? "General")}

TEMPLATE CONTENT:
{template.Content}

Note: Use the template's structure and style as inspiration, but customize all content to address the specific brief requirements and client needs." : "";

            // Build user message
            var userMessage = $@"Generate a proposal based on:

BRIEF ANALYSIS:
{briefAnalysisJson}

USER PROFILE:
- Name: {userName}
- Company: {companyName ?? "Independent Consultant"}
- Hourly Rate: ${hourlyRate ?? 100}
- Experience: Professional with proven track record
- Specialties: {briefAnalysis.ProjectOverview.Type}

CUSTOMIZATION:
- Preferred Tone: {preferredTone ?? "professional"}
- Proposal Length: {proposalLength ?? "medium"}
- Emphasis: Focus on {string.Join(", ", briefAnalysis.RecommendedApproach.KeyThemes.Take(3))}{templateSection}";

            // Call Claude API with Sonnet for better creative writing
            var response = await _claudeApiService.SendMessageAsync(
                message: userMessage,
                systemPrompt: PROPOSAL_GENERATOR_SYSTEM_PROMPT,
                cancellationToken: cancellationToken
            );

            // Extract JSON from response
            var proposalJson = response.Content.FirstOrDefault()?.Text ?? string.Empty;

            _logger.LogInformation("Received proposal response with {TokensUsed} tokens",
                (response.Usage?.InputTokens ?? 0) + (response.Usage?.OutputTokens ?? 0));

            // Parse the JSON response
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var result = JsonSerializer.Deserialize<ProposalGenerationResult>(proposalJson, options);

            if (result == null)
            {
                _logger.LogError("Failed to deserialize proposal generation response");
                throw new InvalidOperationException("Failed to parse AI response");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating proposal");
            throw;
        }
    }
}
