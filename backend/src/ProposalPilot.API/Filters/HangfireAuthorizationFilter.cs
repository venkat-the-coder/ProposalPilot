using Hangfire.Dashboard;

namespace ProposalPilot.API.Filters;

/// <summary>
/// Authorization filter for Hangfire Dashboard in production
/// Only allows authenticated admin users to access the dashboard
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // In production, require authentication
        if (!httpContext.User.Identity?.IsAuthenticated ?? true)
        {
            return false;
        }

        // Check for admin role (adjust as needed for your authorization scheme)
        return httpContext.User.IsInRole("Admin");
    }
}
