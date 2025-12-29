using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.Configuration;
using ProposalPilot.Domain.Enums;

namespace ProposalPilot.API.Controllers;

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly StripeSettings _stripeSettings;
    private readonly ILogger<StripeWebhookController> _logger;

    public StripeWebhookController(
        ApplicationDbContext context,
        IOptions<StripeSettings> stripeSettings,
        ILogger<StripeWebhookController> logger)
    {
        _context = context;
        _stripeSettings = stripeSettings.Value;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _stripeSettings.WebhookSecret,
                throwOnApiVersionMismatch: false
            );

            _logger.LogInformation("Received Stripe webhook: {EventType}", stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    await HandleCheckoutSessionCompleted(stripeEvent);
                    break;

                case "customer.subscription.created":
                    await HandleSubscriptionCreated(stripeEvent);
                    break;

                case "customer.subscription.updated":
                    await HandleSubscriptionUpdated(stripeEvent);
                    break;

                case "customer.subscription.deleted":
                    await HandleSubscriptionDeleted(stripeEvent);
                    break;

                case "invoice.payment_succeeded":
                    await HandleInvoicePaymentSucceeded(stripeEvent);
                    break;

                case "invoice.payment_failed":
                    await HandleInvoicePaymentFailed(stripeEvent);
                    break;

                default:
                    _logger.LogInformation("Unhandled event type: {EventType}", stripeEvent.Type);
                    break;
            }

            return Ok();
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe webhook error");
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing webhook");
            return StatusCode(500);
        }
    }

    private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
    {
        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
        if (session == null) return;

        _logger.LogInformation("Checkout session completed: {SessionId}", session.Id);

        var userId = session.Metadata.GetValueOrDefault("user_id");
        var planString = session.Metadata.GetValueOrDefault("plan");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(planString))
        {
            _logger.LogWarning("Missing metadata in checkout session");
            return;
        }

        if (!Guid.TryParse(userId, out var userGuid))
        {
            _logger.LogWarning("Invalid user ID in checkout session");
            return;
        }

        if (!Enum.TryParse<SubscriptionPlan>(planString, out var plan))
        {
            _logger.LogWarning("Invalid plan in checkout session");
            return;
        }

        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userGuid);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return;
        }

        // Create or update subscription
        if (user.Subscription == null)
        {
            user.Subscription = new Domain.Entities.Subscription
            {
                UserId = user.Id,
                Plan = plan,
                StartDate = DateTime.UtcNow,
                IsActive = true,
                AutoRenew = true,
                StripeCustomerId = session.CustomerId,
                StripeSubscriptionId = session.SubscriptionId,
                MonthlyPrice = plan switch
                {
                    SubscriptionPlan.Starter => 29m,
                    SubscriptionPlan.Professional => 99m,
                    SubscriptionPlan.Enterprise => 299m,
                    _ => 0m
                },
                ProposalsPerMonth = plan switch
                {
                    SubscriptionPlan.Starter => 10,
                    SubscriptionPlan.Professional => 50,
                    SubscriptionPlan.Enterprise => -1, // Unlimited
                    _ => 3
                },
                ProposalsUsedThisMonth = 0,
                UsageResetDate = DateTime.UtcNow.AddMonths(1),
                HasAIAnalysis = plan != SubscriptionPlan.Free,
                HasAdvancedTemplates = plan == SubscriptionPlan.Professional || plan == SubscriptionPlan.Enterprise,
                HasPrioritySupport = plan == SubscriptionPlan.Enterprise,
                HasWhiteLabeling = plan == SubscriptionPlan.Enterprise
            };
            _context.Subscriptions.Add(user.Subscription);
        }
        else
        {
            user.Subscription.Plan = plan;
            user.Subscription.IsActive = true;
            user.Subscription.StripeCustomerId = session.CustomerId;
            user.Subscription.StripeSubscriptionId = session.SubscriptionId;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Subscription created/updated for user: {UserId}", userId);
    }

    private async Task HandleSubscriptionCreated(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Stripe.Subscription;
        if (subscription == null) return;

        _logger.LogInformation("Subscription created: {SubscriptionId}", subscription.Id);
        // Additional logic if needed
    }

    private async Task HandleSubscriptionUpdated(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Stripe.Subscription;
        if (subscription == null) return;

        _logger.LogInformation("Subscription updated: {SubscriptionId}", subscription.Id);

        var dbSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == subscription.Id);

        if (dbSubscription != null)
        {
            dbSubscription.IsActive = subscription.Status == "active";
            dbSubscription.AutoRenew = !subscription.CancelAtPeriodEnd;

            if (subscription.CanceledAt.HasValue)
            {
                dbSubscription.CancelledAt = subscription.CanceledAt.Value;
            }

            await _context.SaveChangesAsync();
        }
    }

    private async Task HandleSubscriptionDeleted(Event stripeEvent)
    {
        var subscription = stripeEvent.Data.Object as Stripe.Subscription;
        if (subscription == null) return;

        _logger.LogInformation("Subscription deleted: {SubscriptionId}", subscription.Id);

        var dbSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == subscription.Id);

        if (dbSubscription != null)
        {
            dbSubscription.IsActive = false;
            dbSubscription.EndDate = DateTime.UtcNow;
            dbSubscription.CancelledAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    private async Task HandleInvoicePaymentSucceeded(Event stripeEvent)
    {
        var invoice = stripeEvent.Data.Object as Invoice;
        if (invoice == null) return;

        _logger.LogInformation("Invoice payment succeeded: {InvoiceId}", invoice.Id);
        // Could create a Payment record here
    }

    private async Task HandleInvoicePaymentFailed(Event stripeEvent)
    {
        var invoice = stripeEvent.Data.Object as Invoice;
        if (invoice == null) return;

        _logger.LogError("Invoice payment failed: {InvoiceId}", invoice.Id);

        // Could send notification to user about failed payment
        // Could deactivate subscription after multiple failures
    }
}
