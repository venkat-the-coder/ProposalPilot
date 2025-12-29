# ProposalPilot AI - Technical Specifications

> Complete technical reference for database schema, API endpoints, and configurations.

---

## Table of Contents

1. [Database Schema](#database-schema)
2. [API Endpoints](#api-endpoints)
3. [Configuration Files](#configuration-files)
4. [External Services](#external-services)
5. [Security Specifications](#security-specifications)

---

## Database Schema

### Entity Relationship Overview

```
Users (1) ──────── (M) Proposals
  │                      │
  │                      ├── (M) ProposalViews
  │                      └── (M) FollowUps
  │
  ├── (1) UserProfile
  ├── (1) Subscription
  └── (M) AIUsageLogs
  
ProposalTemplates (standalone, linked to Users for custom templates)
EmailLogs (linked to Proposals)
```

---

### Core Entities

#### 1. User Entity

```csharp
public class User : BaseEntity
{
    public string Email { get; set; }                    // Required, Unique, Max 255
    public string PasswordHash { get; set; }             // Required
    public string FullName { get; set; }                 // Required, Max 100
    public string? CompanyName { get; set; }             // Optional, Max 100
    public decimal HourlyRate { get; set; }              // Default 0, Precision(10,2)
    public PlanType PlanType { get; set; }               // Enum: Free, Starter, Pro, Agency
    public string? StripeCustomerId { get; set; }        // Optional, Max 100
    public string? RefreshToken { get; set; }            // Optional
    public DateTime? RefreshTokenExpiry { get; set; }    // Optional
    public bool EmailVerified { get; set; }              // Default false
    public DateTime? LastLoginAt { get; set; }           // Optional
    
    // Navigation
    public UserProfile Profile { get; set; }
    public Subscription? Subscription { get; set; }
    public ICollection<Proposal> Proposals { get; set; }
    public ICollection<AIUsageLog> AIUsageLogs { get; set; }
}
```

#### 2. UserProfile Entity

```csharp
public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }                     // FK to Users
    public string? Bio { get; set; }                     // Optional, Max 1000
    public string? Website { get; set; }                 // Optional, Max 255
    public string? LinkedInUrl { get; set; }             // Optional, Max 255
    public string? ProfileImageUrl { get; set; }         // Optional, Max 500
    public string? LogoUrl { get; set; }                 // Optional, Max 500
    public string? PrimaryColor { get; set; }            // Optional, Max 7 (#XXXXXX)
    public string? SecondaryColor { get; set; }          // Optional, Max 7
    public string? SignatureHtml { get; set; }           // Optional, Max 2000
    public string? PortfolioJson { get; set; }           // JSON array of portfolio items
    public string? UspsJson { get; set; }                // JSON array of USPs
    public string? SkillsJson { get; set; }              // JSON array of skills
    public int YearsExperience { get; set; }             // Default 0
    
    // Navigation
    public User User { get; set; }
}
```

#### 3. Proposal Entity

```csharp
public class Proposal : BaseEntity
{
    public Guid UserId { get; set; }                     // FK to Users
    public string Title { get; set; }                    // Required, Max 200
    public string? ClientName { get; set; }              // Optional, Max 100
    public string? ClientEmail { get; set; }             // Optional, Max 255
    public string? ClientCompany { get; set; }           // Optional, Max 100
    public ProposalStatus Status { get; set; }           // Enum
    public string OriginalBrief { get; set; }            // Required, Max
    public string? BriefAnalysisJson { get; set; }       // JSON, AI analysis result
    public string? GeneratedContentJson { get; set; }    // JSON, proposal sections
    public string? PricingOptionsJson { get; set; }      // JSON, pricing tiers
    public string? CustomContentJson { get; set; }       // JSON, user edits
    public int QualityScore { get; set; }                // 0-100
    public string? QualityFeedbackJson { get; set; }     // JSON, scoring details
    public int ViewCount { get; set; }                   // Default 0
    public int UniqueViewCount { get; set; }             // Default 0
    public decimal EngagementScore { get; set; }         // 0-100
    public decimal? SelectedPrice { get; set; }          // Precision(12,2)
    public string? SelectedTier { get; set; }            // Basic/Recommended/Premium
    public DateTime? SentAt { get; set; }
    public DateTime? FirstViewedAt { get; set; }
    public DateTime? LastViewedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectionReason { get; set; }         // Optional, Max 500
    
    // Navigation
    public User User { get; set; }
    public ICollection<ProposalView> Views { get; set; }
    public ICollection<FollowUp> FollowUps { get; set; }
    public ICollection<EmailLog> EmailLogs { get; set; }
}
```

#### 4. ProposalView Entity

```csharp
public class ProposalView : BaseEntity
{
    public Guid ProposalId { get; set; }                 // FK to Proposals
    public string? ViewerIp { get; set; }                // Optional, Max 45
    public string? ViewerUserAgent { get; set; }         // Optional, Max 500
    public string? ViewerLocation { get; set; }          // Optional, Max 100
    public DateTime ViewedAt { get; set; }               // Required
    public int DurationSeconds { get; set; }             // Default 0
    public string? SectionsViewedJson { get; set; }      // JSON array
    public string? EventType { get; set; }               // open, scroll, click, etc.
    public string? EventDataJson { get; set; }           // JSON, event details
    
    // Navigation
    public Proposal Proposal { get; set; }
}
```

#### 5. ProposalTemplate Entity

```csharp
public class ProposalTemplate : BaseEntity
{
    public string Name { get; set; }                     // Required, Max 100
    public string? Description { get; set; }             // Optional, Max 500
    public string Industry { get; set; }                 // Required, Max 50
    public string ContentJson { get; set; }              // JSON, template content
    public bool IsSystem { get; set; }                   // Default false
    public Guid? UserId { get; set; }                    // FK, null for system templates
    public int UsageCount { get; set; }                  // Default 0
    public string? ThumbnailUrl { get; set; }            // Optional, Max 500
    public string? Tags { get; set; }                    // Comma-separated, Max 255
    
    // Navigation
    public User? User { get; set; }
}
```

#### 6. Subscription Entity

```csharp
public class Subscription : BaseEntity
{
    public Guid UserId { get; set; }                     // FK to Users
    public string StripeSubscriptionId { get; set; }     // Required, Max 100
    public string StripePriceId { get; set; }            // Required, Max 100
    public PlanType PlanType { get; set; }               // Enum
    public SubscriptionStatus Status { get; set; }       // Enum
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; }          // Default false
    public DateTime? CanceledAt { get; set; }
    public int ProposalsUsedThisPeriod { get; set; }     // Default 0
    public int ProposalsLimit { get; set; }              // Based on plan
    
    // Navigation
    public User User { get; set; }
}
```

#### 7. AIUsageLog Entity

```csharp
public class AIUsageLog : BaseEntity
{
    public Guid UserId { get; set; }                     // FK to Users
    public Guid? ProposalId { get; set; }                // FK, optional
    public string Model { get; set; }                    // Required, Max 50
    public string Operation { get; set; }                // Required, Max 50
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public decimal Cost { get; set; }                    // Precision(10,6)
    public int DurationMs { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }            // Optional, Max 1000
    
    // Navigation
    public User User { get; set; }
    public Proposal? Proposal { get; set; }
}
```

#### 8. FollowUp Entity

```csharp
public class FollowUp : BaseEntity
{
    public Guid ProposalId { get; set; }                 // FK to Proposals
    public int SequenceNumber { get; set; }              // 1, 2, 3, etc.
    public string TriggerType { get; set; }              // Required, Max 50
    public FollowUpStatus Status { get; set; }           // Enum
    public DateTime ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Subject { get; set; }                 // Optional, Max 200
    public string? BodyHtml { get; set; }                // Optional
    public string? SendGridMessageId { get; set; }       // Optional, Max 100
    
    // Navigation
    public Proposal Proposal { get; set; }
}
```

#### 9. EmailLog Entity

```csharp
public class EmailLog : BaseEntity
{
    public Guid ProposalId { get; set; }                 // FK to Proposals
    public string EmailType { get; set; }                // proposal_delivery, followup, etc.
    public string ToEmail { get; set; }                  // Required, Max 255
    public string Subject { get; set; }                  // Required, Max 200
    public string? SendGridMessageId { get; set; }       // Optional, Max 100
    public EmailStatus Status { get; set; }              // Enum
    public DateTime? DeliveredAt { get; set; }
    public DateTime? OpenedAt { get; set; }
    public int OpenCount { get; set; }                   // Default 0
    public DateTime? ClickedAt { get; set; }
    public int ClickCount { get; set; }                  // Default 0
    public DateTime? BouncedAt { get; set; }
    public string? BounceReason { get; set; }            // Optional, Max 500
    
    // Navigation
    public Proposal Proposal { get; set; }
}
```

---

### Base Entity

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }                         // Primary Key
    public DateTime CreatedAt { get; set; }              // Auto-set
    public DateTime? UpdatedAt { get; set; }             // Auto-set on update
    public string? CreatedBy { get; set; }               // User ID who created
    public string? UpdatedBy { get; set; }               // User ID who updated
    public bool IsDeleted { get; set; }                  // Soft delete flag
    public DateTime? DeletedAt { get; set; }             // When deleted
}
```

---

### Enums

```csharp
public enum PlanType
{
    Free = 0,
    Starter = 1,
    Pro = 2,
    Agency = 3,
    Enterprise = 4
}

public enum ProposalStatus
{
    Draft = 0,
    Sent = 1,
    Viewed = 2,
    Accepted = 3,
    Rejected = 4,
    Expired = 5
}

public enum SubscriptionStatus
{
    Active = 0,
    PastDue = 1,
    Canceled = 2,
    Trialing = 3,
    Incomplete = 4
}

public enum FollowUpStatus
{
    Scheduled = 0,
    Sent = 1,
    Skipped = 2,
    Failed = 3
}

public enum EmailStatus
{
    Pending = 0,
    Sent = 1,
    Delivered = 2,
    Opened = 3,
    Clicked = 4,
    Bounced = 5,
    Failed = 6
}
```

---

### Database Indexes

```sql
-- Users
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email) WHERE IsDeleted = 0;
CREATE INDEX IX_Users_StripeCustomerId ON Users(StripeCustomerId);

-- Proposals
CREATE INDEX IX_Proposals_UserId ON Proposals(UserId);
CREATE INDEX IX_Proposals_Status ON Proposals(Status);
CREATE INDEX IX_Proposals_CreatedAt ON Proposals(CreatedAt DESC);
CREATE INDEX IX_Proposals_UserId_Status ON Proposals(UserId, Status);

-- ProposalViews
CREATE INDEX IX_ProposalViews_ProposalId ON ProposalViews(ProposalId);
CREATE INDEX IX_ProposalViews_ViewedAt ON ProposalViews(ViewedAt DESC);

-- AIUsageLogs
CREATE INDEX IX_AIUsageLogs_UserId ON AIUsageLogs(UserId);
CREATE INDEX IX_AIUsageLogs_CreatedAt ON AIUsageLogs(CreatedAt DESC);

-- FollowUps
CREATE INDEX IX_FollowUps_ProposalId ON FollowUps(ProposalId);
CREATE INDEX IX_FollowUps_ScheduledFor ON FollowUps(ScheduledFor) WHERE Status = 0;
```

---

## API Endpoints

### Authentication

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login, get tokens | No |
| POST | `/api/auth/refresh-token` | Refresh access token | No |
| POST | `/api/auth/forgot-password` | Request password reset | No |
| POST | `/api/auth/reset-password` | Reset password with token | No |
| GET | `/api/auth/me` | Get current user | Yes |
| POST | `/api/auth/logout` | Logout (invalidate refresh) | Yes |

### Users & Profiles

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/users/profile` | Get user profile | Yes |
| PUT | `/api/users/profile` | Update profile | Yes |
| PUT | `/api/users/settings` | Update settings | Yes |
| GET | `/api/users/stats` | Get user statistics | Yes |
| DELETE | `/api/users` | Delete account (soft) | Yes |

### Brief Analysis

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/analysis/analyze-brief` | Analyze client brief | Yes |
| GET | `/api/analysis/{id}` | Get existing analysis | Yes |
| POST | `/api/analysis/{id}/regenerate` | Re-analyze brief | Yes |

### Proposals

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/proposals` | List proposals (paginated) | Yes |
| POST | `/api/proposals` | Create proposal draft | Yes |
| GET | `/api/proposals/{id}` | Get single proposal | Yes |
| PUT | `/api/proposals/{id}` | Update proposal | Yes |
| DELETE | `/api/proposals/{id}` | Delete proposal (soft) | Yes |
| POST | `/api/proposals/{id}/generate` | Generate with AI | Yes |
| POST | `/api/proposals/{id}/pricing` | Get AI pricing | Yes |
| POST | `/api/proposals/{id}/score` | Get quality score | Yes |
| POST | `/api/proposals/{id}/send` | Send to client | Yes |
| GET | `/api/proposals/{id}/pdf` | Download PDF | Yes |
| GET | `/api/proposals/{id}/docx` | Download DOCX | Yes |
| POST | `/api/proposals/{id}/duplicate` | Duplicate proposal | Yes |
| GET | `/api/proposals/{id}/analytics` | Engagement data | Yes |

### Templates

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/templates` | List templates | Yes |
| GET | `/api/templates/{id}` | Get template | Yes |
| POST | `/api/templates` | Create user template | Yes |
| PUT | `/api/templates/{id}` | Update template | Yes |
| DELETE | `/api/templates/{id}` | Delete template | Yes |

### Subscriptions

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/subscriptions/current` | Get subscription | Yes |
| POST | `/api/subscriptions/checkout` | Create checkout | Yes |
| POST | `/api/subscriptions/portal` | Create portal session | Yes |
| POST | `/api/subscriptions/cancel` | Cancel subscription | Yes |

### Analytics

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/analytics/summary` | Summary stats | Yes |
| GET | `/api/analytics/proposals` | Proposal performance | Yes |
| GET | `/api/analytics/trends` | Trends over time | Yes |

### Webhooks

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/webhooks/stripe` | Stripe events | Signature |
| POST | `/api/webhooks/sendgrid` | SendGrid events | Signature |

### Public (Proposal Viewing)

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/public/proposals/{token}` | View proposal | No |
| POST | `/api/public/proposals/{token}/view` | Record view event | No |
| POST | `/api/public/proposals/{token}/accept` | Accept proposal | No |
| POST | `/api/public/proposals/{token}/reject` | Reject proposal | No |

---

## Configuration Files

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProposalPilotDB;..."
  },
  "Jwt": {
    "Secret": "your-256-bit-secret",
    "Issuer": "ProposalPilot",
    "Audience": "ProposalPilotUsers",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Claude": {
    "ApiKey": "sk-ant-xxxxx",
    "BaseUrl": "https://api.anthropic.com/v1/messages",
    "DefaultModel": "claude-haiku-4-5-20251001",
    "ProposalModel": "claude-sonnet-4-20250514",
    "TimeoutSeconds": 60,
    "MaxRetries": 3
  },
  "SendGrid": {
    "ApiKey": "SG.xxxxx",
    "FromEmail": "noreply@proposalpilot.ai",
    "FromName": "ProposalPilot"
  },
  "Stripe": {
    "SecretKey": "sk_test_xxxxx",
    "PublishableKey": "pk_test_xxxxx",
    "WebhookSecret": "whsec_xxxxx",
    "Prices": {
      "Starter": "price_xxxxx",
      "Pro": "price_xxxxx",
      "Agency": "price_xxxxx"
    }
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Storage": {
    "Provider": "Azure",
    "ConnectionString": "DefaultEndpointsProtocol=https;..."
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": ["Console", "File"]
  }
}
```

---

## External Services

### Stripe Products & Prices

| Plan | Monthly Price | Proposal Limit | Stripe Price ID |
|------|---------------|----------------|-----------------|
| Free | $0 | 3/month | (no subscription) |
| Starter | $19 | 20/month | price_starter_monthly |
| Pro | $49 | Unlimited | price_pro_monthly |
| Agency | $99 | Unlimited + 5 seats | price_agency_monthly |

### Stripe Webhook Events to Handle

- `checkout.session.completed`
- `customer.subscription.created`
- `customer.subscription.updated`
- `customer.subscription.deleted`
- `invoice.paid`
- `invoice.payment_failed`

### SendGrid Webhook Events to Handle

- `delivered`
- `open`
- `click`
- `bounce`
- `dropped`
- `deferred`

---

## Security Specifications

### Password Requirements

- Minimum 8 characters
- At least 1 uppercase letter
- At least 1 lowercase letter
- At least 1 number
- At least 1 special character

### JWT Token Structure

```json
{
  "sub": "user-guid",
  "email": "user@example.com",
  "name": "User Name",
  "plan": "Pro",
  "iat": 1234567890,
  "exp": 1234568790
}
```

### Rate Limiting

| Endpoint Type | Limit |
|---------------|-------|
| Auth endpoints | 5/minute per IP |
| AI generation | 10/minute per user |
| General API | 100/minute per user |
| Webhooks | 1000/minute per IP |

### CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "https://app.proposalpilot.ai"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```
