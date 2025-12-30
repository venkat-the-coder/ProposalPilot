namespace ProposalPilot.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Infrastructure.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IAnalyticsService analyticsService,
        ICurrentUserService currentUserService,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard overview with key metrics
    /// </summary>
    [HttpGet("overview")]
    public async Task<ActionResult<DashboardOverview>> GetOverview()
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var overview = await _analyticsService.GetDashboardOverviewAsync(_currentUserService.UserId.Value);
            return Ok(overview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting analytics overview for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while getting analytics" });
        }
    }

    /// <summary>
    /// Get proposal trends over time
    /// </summary>
    [HttpGet("trends")]
    public async Task<ActionResult<ProposalTrends>> GetTrends([FromQuery] int days = 30)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        // Limit days range
        days = Math.Clamp(days, 7, 365);

        try
        {
            var trends = await _analyticsService.GetProposalTrendsAsync(_currentUserService.UserId.Value, days);
            return Ok(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting proposal trends for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while getting trends" });
        }
    }

    /// <summary>
    /// Get engagement analytics
    /// </summary>
    [HttpGet("engagement")]
    public async Task<ActionResult<EngagementAnalytics>> GetEngagement()
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var engagement = await _analyticsService.GetEngagementAnalyticsAsync(_currentUserService.UserId.Value);
            return Ok(engagement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting engagement analytics for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while getting engagement analytics" });
        }
    }

    /// <summary>
    /// Get AI usage analytics
    /// </summary>
    [HttpGet("ai-usage")]
    public async Task<ActionResult<AIUsageAnalytics>> GetAIUsage([FromQuery] int days = 30)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        // Limit days range
        days = Math.Clamp(days, 7, 365);

        try
        {
            var aiUsage = await _analyticsService.GetAIUsageAnalyticsAsync(_currentUserService.UserId.Value, days);
            return Ok(aiUsage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AI usage analytics for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while getting AI usage analytics" });
        }
    }

    /// <summary>
    /// Get client analytics
    /// </summary>
    [HttpGet("clients")]
    public async Task<ActionResult<ClientAnalytics>> GetClients()
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var clients = await _analyticsService.GetClientAnalyticsAsync(_currentUserService.UserId.Value);
            return Ok(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client analytics for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while getting client analytics" });
        }
    }

    /// <summary>
    /// Get full analytics report
    /// </summary>
    [HttpGet("report")]
    public async Task<ActionResult<FullAnalyticsReport>> GetFullReport([FromQuery] int days = 30)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        // Limit days range
        days = Math.Clamp(days, 7, 365);

        try
        {
            var report = await _analyticsService.GetFullReportAsync(_currentUserService.UserId.Value, days);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting full analytics report for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An error occurred while getting the report" });
        }
    }
}
