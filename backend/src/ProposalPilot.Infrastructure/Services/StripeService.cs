using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stripe.BillingPortal;
using ProposalPilot.Shared.Configuration;
using ProposalPilot.Domain.Enums;

namespace ProposalPilot.Infrastructure.Services;

public interface IStripeService
{
    Task<Stripe.Checkout.Session> CreateCheckoutSessionAsync(string userId, string userEmail, SubscriptionPlan plan, string successUrl, string cancelUrl);
    Task<Stripe.BillingPortal.Session> CreateCustomerPortalSessionAsync(string customerId, string returnUrl);
    Task<Customer> GetOrCreateCustomerAsync(string userId, string email, string? name = null);
    Task<Subscription> GetSubscriptionAsync(string subscriptionId);
    Task<Subscription> CancelSubscriptionAsync(string subscriptionId);
}

public class StripeService : IStripeService
{
    private readonly StripeSettings _stripeSettings;

    // Stripe Price IDs - These will be created in Stripe Dashboard
    // Users will need to replace these with their actual Price IDs from Stripe
    private readonly Dictionary<SubscriptionPlan, string> _priceIds = new()
    {
        { SubscriptionPlan.Starter, "price_starter_monthly" },       // Replace with actual Stripe Price ID
        { SubscriptionPlan.Professional, "price_professional_monthly" }, // Replace with actual Stripe Price ID
        { SubscriptionPlan.Enterprise, "price_enterprise_monthly" }   // Replace with actual Stripe Price ID
    };

    public StripeService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public async Task<Stripe.Checkout.Session> CreateCheckoutSessionAsync(
        string userId,
        string userEmail,
        SubscriptionPlan plan,
        string successUrl,
        string cancelUrl)
    {
        if (plan == SubscriptionPlan.Free)
        {
            throw new InvalidOperationException("Cannot create checkout session for Free plan");
        }

        if (!_priceIds.ContainsKey(plan))
        {
            throw new InvalidOperationException($"No price configured for plan: {plan}");
        }

        var options = new Stripe.Checkout.SessionCreateOptions
        {
            Mode = "subscription",
            CustomerEmail = userEmail,
            ClientReferenceId = userId,
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = _priceIds[plan],
                    Quantity = 1,
                },
            },
            SubscriptionData = new SessionSubscriptionDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "user_id", userId },
                    { "plan", plan.ToString() }
                }
            },
            Metadata = new Dictionary<string, string>
            {
                { "user_id", userId },
                { "plan", plan.ToString() }
            }
        };

        var service = new Stripe.Checkout.SessionService();
        return await service.CreateAsync(options);
    }

    public async Task<Stripe.BillingPortal.Session> CreateCustomerPortalSessionAsync(
        string customerId,
        string returnUrl)
    {
        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = customerId,
            ReturnUrl = returnUrl,
        };

        var service = new Stripe.BillingPortal.SessionService();
        return await service.CreateAsync(options);
    }

    public async Task<Customer> GetOrCreateCustomerAsync(
        string userId,
        string email,
        string? name = null)
    {
        // Try to find existing customer by metadata
        var customerService = new CustomerService();
        var searchOptions = new CustomerSearchOptions
        {
            Query = $"metadata['user_id']:'{userId}'",
        };

        var customers = await customerService.SearchAsync(searchOptions);
        if (customers.Data.Count > 0)
        {
            return customers.Data[0];
        }

        // Create new customer
        var createOptions = new CustomerCreateOptions
        {
            Email = email,
            Name = name,
            Metadata = new Dictionary<string, string>
            {
                { "user_id", userId }
            }
        };

        return await customerService.CreateAsync(createOptions);
    }

    public async Task<Subscription> GetSubscriptionAsync(string subscriptionId)
    {
        var service = new SubscriptionService();
        return await service.GetAsync(subscriptionId);
    }

    public async Task<Subscription> CancelSubscriptionAsync(string subscriptionId)
    {
        var service = new SubscriptionService();
        var options = new SubscriptionCancelOptions
        {
            // Cancel at period end to allow user to use remaining time
            InvoiceNow = false,
            Prorate = false
        };

        return await service.CancelAsync(subscriptionId, options);
    }
}
