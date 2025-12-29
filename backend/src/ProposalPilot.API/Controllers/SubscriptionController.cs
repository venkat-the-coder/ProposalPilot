using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Domain.Enums;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Infrastructure.Services;

namespace ProposalPilot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly IStripeService _stripeService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(
        IStripeService stripeService,
        ICurrentUserService currentUserService,
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<SubscriptionController> logger)
    {
        _stripeService = stripeService;
        _currentUserService = currentUserService;
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Create a Stripe checkout session for a subscription
    /// </summary>
    [HttpPost("checkout")]
    public async Task<ActionResult> CreateCheckoutSession([FromBody] CreateCheckoutRequest request)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId.Value);

            if (user == null)
                return NotFound(new { message = "User not found" });

            // Parse the subscription plan
            if (!Enum.TryParse<SubscriptionPlan>(request.Plan, true, out var plan))
            {
                return BadRequest(new { message = "Invalid subscription plan" });
            }

            if (plan == SubscriptionPlan.Free)
            {
                return BadRequest(new { message = "Cannot create checkout for Free plan" });
            }

            // Build success and cancel URLs
            var frontendUrl = _configuration["CorsSettings:AllowedOrigins:0"] ?? "http://localhost:4200";
            var successUrl = $"{frontendUrl}/subscription/success?session_id={{CHECKOUT_SESSION_ID}}";
            var cancelUrl = $"{frontendUrl}/pricing";

            var session = await _stripeService.CreateCheckoutSessionAsync(
                user.Id.ToString(),
                user.Email,
                plan,
                successUrl,
                cancelUrl
            );

            return Ok(new { sessionId = session.Id, url = session.Url });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating checkout session for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred while creating checkout session" });
        }
    }

    /// <summary>
    /// Create a Stripe customer portal session
    /// </summary>
    [HttpPost("portal")]
    public async Task<ActionResult> CreatePortalSession()
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId.Value);

            if (user == null)
                return NotFound(new { message = "User not found" });

            if (user.Subscription?.StripeCustomerId == null)
            {
                return BadRequest(new { message = "No active subscription found" });
            }

            var frontendUrl = _configuration["CorsSettings:AllowedOrigins:0"] ?? "http://localhost:4200";
            var returnUrl = $"{frontendUrl}/settings";

            var session = await _stripeService.CreateCustomerPortalSessionAsync(
                user.Subscription.StripeCustomerId,
                returnUrl
            );

            return Ok(new { url = session.Url });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating portal session for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred while creating portal session" });
        }
    }

    /// <summary>
    /// Get current user's subscription status
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult> GetSubscriptionStatus()
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        try
        {
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId.Value);

            if (user == null)
                return NotFound(new { message = "User not found" });

            if (user.Subscription == null)
            {
                return Ok(new
                {
                    plan = "Free",
                    isActive = true,
                    proposalsPerMonth = 3,
                    proposalsUsedThisMonth = 0,
                    hasAIAnalysis = false,
                    hasAdvancedTemplates = false,
                    hasPrioritySupport = false,
                    hasWhiteLabeling = false
                });
            }

            return Ok(new
            {
                plan = user.Subscription.Plan.ToString(),
                isActive = user.Subscription.IsActive,
                startDate = user.Subscription.StartDate,
                endDate = user.Subscription.EndDate,
                cancelledAt = user.Subscription.CancelledAt,
                autoRenew = user.Subscription.AutoRenew,
                proposalsPerMonth = user.Subscription.ProposalsPerMonth,
                proposalsUsedThisMonth = user.Subscription.ProposalsUsedThisMonth,
                usageResetDate = user.Subscription.UsageResetDate,
                hasAIAnalysis = user.Subscription.HasAIAnalysis,
                hasAdvancedTemplates = user.Subscription.HasAdvancedTemplates,
                hasPrioritySupport = user.Subscription.HasPrioritySupport,
                hasWhiteLabeling = user.Subscription.HasWhiteLabeling
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching subscription status for user {UserId}", _currentUserService.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred while fetching subscription status" });
        }
    }
}

public record CreateCheckoutRequest(string Plan);
