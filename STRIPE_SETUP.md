# Stripe Integration Setup Guide

This guide will walk you through setting up Stripe for ProposalPilot's subscription payments.

## Prerequisites

- A Stripe account (sign up at https://stripe.com)
- Access to your Stripe Dashboard
- ProposalPilot backend and frontend code deployed

---

## Step 1: Create Stripe Account & Get API Keys

1. **Sign up for Stripe** at https://stripe.com if you don't have an account

2. **Get your API keys**:
   - Go to https://dashboard.stripe.com/test/apikeys
   - Copy your **Publishable key** (starts with `pk_test_`)
   - Copy your **Secret key** (starts with `sk_test_`)
   - **IMPORTANT**: Use test keys for development, live keys for production

3. **Add keys to your backend configuration**:

   Update `appsettings.json`:
   ```json
   {
     "StripeSettings": {
       "SecretKey": "sk_test_YOUR_SECRET_KEY_HERE",
       "PublishableKey": "pk_test_YOUR_PUBLISHABLE_KEY_HERE",
       "WebhookSecret": "" // Will add this in Step 4
     }
   }
   ```

   Or use environment variables:
   ```bash
   STRIPE_SECRET_KEY=sk_test_YOUR_SECRET_KEY_HERE
   STRIPE_PUBLISHABLE_KEY=pk_test_YOUR_PUBLISHABLE_KEY_HERE
   ```

---

## Step 2: Create Products and Prices

You need to create 3 products in Stripe Dashboard corresponding to your subscription plans.

### 2.1 Create Starter Plan

1. Go to https://dashboard.stripe.com/test/products
2. Click **"+ Add Product"**
3. Fill in the details:
   - **Name**: ProposalPilot Starter
   - **Description**: For freelancers and small teams - 10 proposals per month
   - **Pricing model**: Standard pricing
   - **Price**: $29
   - **Billing period**: Monthly
   - **Currency**: USD
4. Click **"Save product"**
5. **Copy the Price ID** (starts with `price_`) - you'll need this!

### 2.2 Create Professional Plan

1. Click **"+ Add Product"** again
2. Fill in:
   - **Name**: ProposalPilot Professional
   - **Description**: For growing businesses - 50 proposals per month
   - **Price**: $99 per month
3. Click **"Save product"**
4. **Copy the Price ID**

### 2.3 Create Enterprise Plan

1. Click **"+ Add Product"** again
2. Fill in:
   - **Name**: ProposalPilot Enterprise
   - **Description**: For large teams and agencies - Unlimited proposals
   - **Price**: $299 per month
3. Click **"Save product"**
4. **Copy the Price ID**

---

## Step 3: Update Price IDs in Code

Open `backend/src/ProposalPilot.Infrastructure/Services/StripeService.cs` and update the Price IDs:

```csharp
private readonly Dictionary<SubscriptionPlan, string> _priceIds = new()
{
    { SubscriptionPlan.Starter, "price_XXXXX" },       // Replace with your Starter Price ID
    { SubscriptionPlan.Professional, "price_XXXXX" },   // Replace with your Professional Price ID
    { SubscriptionPlan.Enterprise, "price_XXXXX" }      // Replace with your Enterprise Price ID
};
```

**Find your Price IDs**: Go to https://dashboard.stripe.com/test/products and click on each product to see its Price ID.

---

## Step 4: Set Up Webhooks

Webhooks allow Stripe to notify your app about payment events (successful payments, subscription cancellations, etc.).

### 4.1 Create Webhook Endpoint

1. Go to https://dashboard.stripe.com/test/webhooks
2. Click **"+ Add endpoint"**
3. Enter your webhook URL:
   - **For local development**: Use ngrok or similar tool
     - Install ngrok: https://ngrok.com/download
     - Run: `ngrok http 5000` (or your API port)
     - Use the ngrok URL: `https://YOUR_NGROK_URL/api/webhooks/stripe`

   - **For production**: `https://your-domain.com/api/webhooks/stripe`

4. **Select events to listen to**:
   - `checkout.session.completed`
   - `customer.subscription.created`
   - `customer.subscription.updated`
   - `customer.subscription.deleted`
   - `invoice.payment_succeeded`
   - `invoice.payment_failed`

5. Click **"Add endpoint"**

### 4.2 Get Webhook Secret

1. After creating the endpoint, click on it
2. Click **"Reveal" under "Signing secret"**
3. Copy the webhook secret (starts with `whsec_`)
4. Add it to your `appsettings.json`:
   ```json
   {
     "StripeSettings": {
       "WebhookSecret": "whsec_YOUR_WEBHOOK_SECRET_HERE"
     }
   }
   ```

---

## Step 5: Configure Customer Portal

The Customer Portal allows users to manage their subscriptions, update payment methods, and view billing history.

1. Go to https://dashboard.stripe.com/test/settings/billing/portal
2. **Enable customer portal**
3. Configure settings:
   - **Allow customers to**:
     - ✅ Update payment methods
     - ✅ Update billing information
     - ✅ Cancel subscriptions
     - ✅ Switch plans
   - **Cancellation behavior**: Cancel at period end (recommended)
   - **Proration**: Prorate subscription changes
4. Click **"Save changes"**

---

## Step 6: Test the Integration

### 6.1 Test Checkout Flow

1. Start your backend: `dotnet run`
2. Start your frontend: `ng serve`
3. Navigate to `/pricing`
4. Click on a plan (e.g., Starter)
5. You'll be redirected to Stripe Checkout
6. Use test card: `4242 4242 4242 4242`
   - Expiry: Any future date (e.g., 12/34)
   - CVC: Any 3 digits (e.g., 123)
   - ZIP: Any 5 digits (e.g., 12345)
7. Complete the checkout
8. You should be redirected back to your app
9. Check Stripe Dashboard to see the subscription

### 6.2 Test Webhooks Locally

Using ngrok for local webhook testing:

```bash
# Terminal 1: Run your API
cd backend/src/ProposalPilot.API
dotnet run

# Terminal 2: Run ngrok
ngrok http 5000

# Terminal 3: Forward Stripe webhooks
stripe listen --forward-to localhost:5000/api/webhooks/stripe
```

Then complete a test checkout and watch the webhook events arrive.

### 6.3 Stripe Test Cards

Use these test cards for different scenarios:

| Card Number         | Scenario               |
|---------------------|------------------------|
| 4242 4242 4242 4242 | Successful payment     |
| 4000 0000 0000 0341 | Requires authentication|
| 4000 0000 0000 0002 | Card declined          |
| 4000 0000 0000 9995 | Payment fails          |

Full list: https://stripe.com/docs/testing

---

## Step 7: Go Live

When ready to go live:

### 7.1 Switch to Live Mode

1. Toggle from **"Test mode"** to **"Live mode"** in Stripe Dashboard
2. Repeat Steps 2-4 with live mode products and webhooks
3. Get live API keys from https://dashboard.stripe.com/apikeys
4. Update your production environment variables

### 7.2 Production Checklist

- [ ] Created live products and prices
- [ ] Updated Price IDs in production code
- [ ] Set up live webhook endpoint
- [ ] Added live API keys to production environment
- [ ] Configured Customer Portal for live mode
- [ ] Tested complete checkout flow in live mode with real card
- [ ] Verified webhooks are being received
- [ ] Set up Stripe billing email notifications
- [ ] Configured dispute management
- [ ] Set up Stripe Radar for fraud prevention (recommended)

---

## Common Issues & Troubleshooting

### Issue: Webhooks not being received

**Solution**:
- Verify webhook URL is correct and publicly accessible
- Check webhook secret is correct in `appsettings.json`
- Look at webhook delivery attempts in Stripe Dashboard
- Check your API logs for webhook processing errors

### Issue: Checkout session fails to create

**Solution**:
- Verify Price IDs are correct in `StripeService.cs`
- Check Stripe Secret Key is valid
- Ensure products are active in Stripe Dashboard
- Check API logs for detailed error messages

### Issue: "Cannot read properties of null" errors

**Solution**:
- Ensure user is authenticated before accessing subscription endpoints
- Verify JWT token is valid and not expired
- Check CORS settings allow requests from your frontend

---

## Additional Resources

- **Stripe Documentation**: https://stripe.com/docs
- **Stripe Testing Guide**: https://stripe.com/docs/testing
- **Webhooks Guide**: https://stripe.com/docs/webhooks
- **Customer Portal Guide**: https://stripe.com/docs/billing/subscriptions/customer-portal
- **Subscription Lifecycle**: https://stripe.com/docs/billing/subscriptions/overview

---

## Support

If you encounter issues:

1. Check Stripe Dashboard Logs: https://dashboard.stripe.com/test/logs
2. Check your API application logs
3. Review Stripe API errors: https://stripe.com/docs/error-codes
4. Contact Stripe Support: https://support.stripe.com

---

## Security Best Practices

1. **Never commit API keys** to version control
2. **Use environment variables** for all sensitive data
3. **Validate webhook signatures** (already implemented)
4. **Use HTTPS** in production for all endpoints
5. **Enable Stripe Radar** for fraud prevention
6. **Regularly rotate** API keys
7. **Monitor** webhook delivery and payment failures
8. **Set up alerts** for failed payments and disputes

---

That's it! Your Stripe integration is now ready. Happy selling! 🎉
