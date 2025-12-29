using ProposalPilot.Shared.DTOs.Claude;

namespace ProposalPilot.Application.Interfaces;

public interface IClaudeApiService
{
    /// <summary>
    /// Send a message to Claude API and get a response
    /// </summary>
    Task<ClaudeResponse> SendMessageAsync(
        string message,
        string? systemPrompt = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send a conversation to Claude API
    /// </summary>
    Task<ClaudeResponse> SendConversationAsync(
        List<ClaudeMessage> messages,
        string? systemPrompt = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Estimate token count for text
    /// </summary>
    int EstimateTokenCount(string text);

    /// <summary>
    /// Calculate cost for tokens
    /// </summary>
    decimal CalculateCost(int inputTokens, int outputTokens, string model);
}
