using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProposalPilot.Application.Interfaces;

namespace ProposalPilot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClaudeTestController : ControllerBase
{
    private readonly IClaudeApiService _claudeApiService;
    private readonly ILogger<ClaudeTestController> _logger;

    public ClaudeTestController(
        IClaudeApiService claudeApiService,
        ILogger<ClaudeTestController> logger)
    {
        _claudeApiService = claudeApiService;
        _logger = logger;
    }

    /// <summary>
    /// Test Claude API with a simple message
    /// </summary>
    [HttpPost("simple")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> TestSimpleMessage([FromBody] TestMessageRequest request)
    {
        try
        {
            _logger.LogInformation("Testing Claude API with message: {Message}", request.Message);

            var response = await _claudeApiService.SendMessageAsync(
                request.Message,
                request.SystemPrompt);

            var responseText = response.Content.FirstOrDefault()?.Text ?? string.Empty;
            var inputTokens = response.Usage?.InputTokens ?? 0;
            var outputTokens = response.Usage?.OutputTokens ?? 0;
            var cost = _claudeApiService.CalculateCost(inputTokens, outputTokens, response.Model);

            _logger.LogInformation("Claude API response received. Tokens: {InputTokens} in, {OutputTokens} out. Cost: ${Cost:F6}",
                inputTokens, outputTokens, cost);

            return Ok(new
            {
                response = responseText,
                model = response.Model,
                usage = new
                {
                    inputTokens,
                    outputTokens,
                    totalTokens = inputTokens + outputTokens
                },
                cost = new
                {
                    amount = cost,
                    currency = "USD",
                    formatted = $"${cost:F6}"
                },
                metadata = new
                {
                    id = response.Id,
                    stopReason = response.StopReason
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing Claude API");
            return StatusCode(500, new { message = "An error occurred while calling Claude API", error = ex.Message });
        }
    }

    /// <summary>
    /// Estimate token count for text
    /// </summary>
    [HttpPost("estimate-tokens")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult EstimateTokens([FromBody] EstimateTokensRequest request)
    {
        try
        {
            var estimatedTokens = _claudeApiService.EstimateTokenCount(request.Text);

            return Ok(new
            {
                text = request.Text,
                characterCount = request.Text.Length,
                estimatedTokens,
                note = "This is a rough estimation (~4 chars per token)"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating tokens");
            return StatusCode(500, new { message = "An error occurred while estimating tokens", error = ex.Message });
        }
    }

    /// <summary>
    /// Calculate cost for given token counts
    /// </summary>
    [HttpPost("calculate-cost")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CalculateCost([FromBody] CalculateCostRequest request)
    {
        try
        {
            var cost = _claudeApiService.CalculateCost(
                request.InputTokens,
                request.OutputTokens,
                request.Model);

            return Ok(new
            {
                model = request.Model,
                inputTokens = request.InputTokens,
                outputTokens = request.OutputTokens,
                totalTokens = request.InputTokens + request.OutputTokens,
                cost = new
                {
                    amount = cost,
                    currency = "USD",
                    formatted = $"${cost:F6}"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating cost");
            return StatusCode(500, new { message = "An error occurred while calculating cost", error = ex.Message });
        }
    }
}

public record TestMessageRequest(string Message, string? SystemPrompt = null);
public record EstimateTokensRequest(string Text);
public record CalculateCostRequest(int InputTokens, int OutputTokens, string Model = "claude-3-5-sonnet-20241022");
