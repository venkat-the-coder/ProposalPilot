using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Shared.Configuration;
using ProposalPilot.Shared.DTOs.Claude;

namespace ProposalPilot.Infrastructure.Services;

public class ClaudeApiService : IClaudeApiService
{
    private readonly HttpClient _httpClient;
    private readonly AnthropicSettings _settings;
    private readonly ILogger<ClaudeApiService> _logger;
    private const string ApiUrl = "https://api.anthropic.com/v1/messages";

    public ClaudeApiService(
        HttpClient httpClient,
        IOptions<AnthropicSettings> settings,
        ILogger<ClaudeApiService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        // Configure HttpClient
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<ClaudeResponse> SendMessageAsync(
        string message,
        string? systemPrompt = null,
        CancellationToken cancellationToken = default)
    {
        var messages = new List<ClaudeMessage>
        {
            new() { Role = "user", Content = message }
        };

        return await SendConversationAsync(messages, systemPrompt, cancellationToken);
    }

    public async Task<ClaudeResponse> SendConversationAsync(
        List<ClaudeMessage> messages,
        string? systemPrompt = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ClaudeRequest
            {
                Model = _settings.Model,
                MaxTokens = _settings.MaxTokens,
                Messages = messages,
                System = systemPrompt,
                Temperature = 1.0
            };

            _logger.LogInformation("Sending request to Claude API with {MessageCount} messages", messages.Count);

            var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ApiUrl, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Claude API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Claude API error: {response.StatusCode} - {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var claudeResponse = JsonSerializer.Deserialize<ClaudeResponse>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            if (claudeResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize Claude API response");
            }

            _logger.LogInformation("Claude API response received: {InputTokens} input, {OutputTokens} output tokens",
                claudeResponse.Usage?.InputTokens ?? 0,
                claudeResponse.Usage?.OutputTokens ?? 0);

            return claudeResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Claude API");
            throw;
        }
    }

    public int EstimateTokenCount(string text)
    {
        // Rough estimation: ~4 characters per token for English text
        // This is a simplified version - in production, use a proper tokenizer
        if (string.IsNullOrEmpty(text))
            return 0;

        var charCount = text.Length;
        var estimatedTokens = (int)Math.Ceiling(charCount / 4.0);

        _logger.LogDebug("Estimated {Tokens} tokens for {CharCount} characters", estimatedTokens, charCount);

        return estimatedTokens;
    }

    public decimal CalculateCost(int inputTokens, int outputTokens, string model)
    {
        // Claude 3.5 Sonnet pricing (as of 2024)
        // Input: $3 per million tokens
        // Output: $15 per million tokens

        decimal inputCost = 0;
        decimal outputCost = 0;

        if (model.Contains("claude-3-5-sonnet") || model.Contains("claude-3-sonnet"))
        {
            inputCost = (inputTokens / 1_000_000m) * 3m;
            outputCost = (outputTokens / 1_000_000m) * 15m;
        }
        else if (model.Contains("claude-3-haiku"))
        {
            inputCost = (inputTokens / 1_000_000m) * 0.25m;
            outputCost = (outputTokens / 1_000_000m) * 1.25m;
        }
        else if (model.Contains("claude-3-opus"))
        {
            inputCost = (inputTokens / 1_000_000m) * 15m;
            outputCost = (outputTokens / 1_000_000m) * 75m;
        }
        else
        {
            // Default to Sonnet pricing
            inputCost = (inputTokens / 1_000_000m) * 3m;
            outputCost = (outputTokens / 1_000_000m) * 15m;
        }

        var totalCost = inputCost + outputCost;

        _logger.LogDebug("Cost calculation: {InputTokens} input (${InputCost:F6}), {OutputTokens} output (${OutputCost:F6}), Total: ${TotalCost:F6}",
            inputTokens, inputCost, outputTokens, outputCost, totalCost);

        return totalCost;
    }
}
