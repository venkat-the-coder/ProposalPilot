using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Infrastructure.Data;
using System.Security.Claims;
using System.Text.Json;

namespace ProposalPilot.Infrastructure.Middleware;

public class SubscriptionEnforcementMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SubscriptionEnforcementMiddleware> _logger;

    public SubscriptionEnforcementMiddleware(
        RequestDelegate next,
        ILogger<SubscriptionEnforcementMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        // Only enforce on proposal generation endpoint
        if (!context.Request.Path.StartsWithSegments("/api/proposals/generate", StringComparison.OrdinalIgnoreCase)
            || context.Request.Method != "POST")
        {
            await _next(context);
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            // User not authenticated, let auth middleware handle it
            await _next(context);
            return;
        }

        try
        {
            var user = await dbContext.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "User not found" }));
                return;
            }

            // Check if user has a subscription
            if (user.Subscription == null)
            {
                // Free tier - check if they've used their free proposals
                var proposalsThisMonth = await dbContext.Proposals
                    .Where(p => p.UserId == userId && p.CreatedAt >= DateTime.UtcNow.AddMonths(-1))
                    .CountAsync();

                if (proposalsThisMonth >= 3) // Free tier limit
                {
                    context.Response.StatusCode = 402; // Payment Required
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        message = "You've reached your proposal limit for the free plan. Please upgrade to continue.",
                        limit = 3,
                        used = proposalsThisMonth,
                        upgradeRequired = true
                    }));
                    return;
                }
            }
            else
            {
                // Reset usage if month has passed
                if (DateTime.UtcNow >= user.Subscription.UsageResetDate)
                {
                    user.Subscription.ProposalsUsedThisMonth = 0;
                    user.Subscription.UsageResetDate = DateTime.UtcNow.AddMonths(1);
                    await dbContext.SaveChangesAsync();
                }

                // Check if subscription is active
                if (!user.Subscription.IsActive)
                {
                    context.Response.StatusCode = 402; // Payment Required
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        message = "Your subscription is inactive. Please reactivate to continue.",
                        upgradeRequired = true
                    }));
                    return;
                }

                // Check proposal limits (-1 means unlimited)
                if (user.Subscription.ProposalsPerMonth != -1 &&
                    user.Subscription.ProposalsUsedThisMonth >= user.Subscription.ProposalsPerMonth)
                {
                    context.Response.StatusCode = 402; // Payment Required
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        message = $"You've reached your proposal limit of {user.Subscription.ProposalsPerMonth} for this month. Please upgrade or wait until next month.",
                        limit = user.Subscription.ProposalsPerMonth,
                        used = user.Subscription.ProposalsUsedThisMonth,
                        resetDate = user.Subscription.UsageResetDate,
                        upgradeRequired = false
                    }));
                    return;
                }
            }

            // User has quota, proceed with the request
            await _next(context);

            // If the request was successful (proposal created), increment usage
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                if (user.Subscription != null)
                {
                    user.Subscription.ProposalsUsedThisMonth++;
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in subscription enforcement middleware");
            // Don't block the request on middleware errors
            await _next(context);
        }
    }
}

public static class SubscriptionEnforcementMiddlewareExtensions
{
    public static IApplicationBuilder UseSubscriptionEnforcement(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SubscriptionEnforcementMiddleware>();
    }
}
