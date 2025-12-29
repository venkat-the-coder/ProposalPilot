# SendGrid Email Setup Guide

This guide walks you through setting up SendGrid for ProposalPilot's email functionality.

## Prerequisites

- SendGrid account (free tier available at [sendgrid.com](https://sendgrid.com))
- Verified sender email or domain

## Step 1: Create a SendGrid Account

1. Go to [sendgrid.com/free](https://sendgrid.com/free)
2. Sign up for a free account
3. Complete email verification

## Step 2: Create an API Key

1. Navigate to **Settings** > **API Keys**
2. Click **Create API Key**
3. Name it (e.g., "ProposalPilot Production")
4. Select **Full Access** or **Restricted Access** with:
   - Mail Send: Full Access
5. Click **Create & View**
6. **Copy the API key immediately** (you won't see it again)

## Step 3: Verify a Sender

### Option A: Single Sender Verification (Quick)
1. Go to **Settings** > **Sender Authentication**
2. Click **Verify a Single Sender**
3. Fill in:
   - **From Name**: Your name or company name
   - **From Email**: The email address you'll send from
   - **Reply To**: Same as From Email or different
4. Verify via the confirmation email

### Option B: Domain Authentication (Recommended for Production)
1. Go to **Settings** > **Sender Authentication**
2. Click **Authenticate Your Domain**
3. Follow the DNS setup instructions
4. Wait for verification (can take up to 48 hours)

## Step 4: Configure ProposalPilot

### Development (User Secrets)

Use .NET User Secrets to store sensitive configuration:

```bash
cd src/ProposalPilot.API
dotnet user-secrets set "SendGridSettings:ApiKey" "SG.your-api-key-here"
dotnet user-secrets set "SendGridSettings:FromEmail" "proposals@yourdomain.com"
dotnet user-secrets set "SendGridSettings:FromName" "ProposalPilot"
```

### Production (Environment Variables)

Set these environment variables in your hosting environment:

```bash
# Linux/Mac
export SendGridSettings__ApiKey="SG.your-api-key-here"
export SendGridSettings__FromEmail="proposals@yourdomain.com"
export SendGridSettings__FromName="ProposalPilot"

# Windows PowerShell
$env:SendGridSettings__ApiKey = "SG.your-api-key-here"
$env:SendGridSettings__FromEmail = "proposals@yourdomain.com"
$env:SendGridSettings__FromName = "ProposalPilot"
```

### appsettings.json Structure

The configuration structure in `appsettings.json` (values come from secrets/env):

```json
{
  "SendGridSettings": {
    "ApiKey": "YOUR_SENDGRID_API_KEY",
    "FromEmail": "proposals@yourdomain.com",
    "FromName": "ProposalPilot"
  }
}
```

## Step 5: Test the Integration

1. Start the API server
2. Create a proposal
3. Click "Send to Client"
4. Enter a test email address
5. Check for the email

## Email Features

### Proposal Email
- Sent when user clicks "Send to Client"
- Contains proposal title, summary, and view link
- Tracks opens and clicks via SendGrid

### Follow-up Email
- Sent for proposal follow-ups
- Dynamic subject based on days since proposal sent
- Includes personalized message

## Troubleshooting

### Email not sending

1. Check API key is correct
2. Verify sender email is verified in SendGrid
3. Check API logs in SendGrid dashboard
4. Review application logs for errors

### Email going to spam

1. Use domain authentication instead of single sender
2. Add SPF, DKIM, and DMARC records
3. Warm up your sending reputation gradually

### Rate limiting

SendGrid free tier: 100 emails/day
Consider upgrading for production use.

## Security Best Practices

1. **Never commit API keys** - Use user secrets or environment variables
2. **Rotate keys regularly** - Especially if compromised
3. **Use minimal permissions** - Only grant Mail Send access
4. **Monitor activity** - Check SendGrid dashboard for suspicious activity

## Related Files

- `src/ProposalPilot.Application/Interfaces/IEmailService.cs` - Email service interface
- `src/ProposalPilot.Infrastructure/Services/SendGridEmailService.cs` - SendGrid implementation
- `src/ProposalPilot.Shared/Configuration/SendGridSettings.cs` - Configuration class
- `src/ProposalPilot.API/Controllers/ProposalsController.cs` - Send endpoint

## Support

- SendGrid Documentation: [docs.sendgrid.com](https://docs.sendgrid.com)
- SendGrid Support: [support.sendgrid.com](https://support.sendgrid.com)
