# ProposalPilot AI - Development Project Plan

> **Version:** 1.0  
> **Created:** December 2025  
> **Author:** Venkat  
> **Total Duration:** 10 Weeks (50 Working Days)  
> **Stack:** .NET 8 + Angular 18 + SQL Server + Claude AI

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Progress Summary](#progress-summary)
3. [Pre-Development Setup](#pre-development-setup)
4. [Phase 1: Foundation (Week 1-2)](#phase-1-foundation-week-1-2)
5. [Phase 2: Core AI Features (Week 3-4)](#phase-2-core-ai-features-week-3-4)
6. [Phase 3: Essential Features (Week 5-6)](#phase-3-essential-features-week-5-6)
7. [Phase 4: Automation & Tracking (Week 7-8)](#phase-4-automation--tracking-week-7-8)
8. [Phase 5: Polish & Launch (Week 9-10)](#phase-5-polish--launch-week-9-10)
9. [Post-Launch Tasks](#post-launch-tasks)
10. [Technical Specifications](#technical-specifications)
11. [File Structure Reference](#file-structure-reference)

---

## Project Overview

### What We're Building

ProposalPilot AI is an AI-powered SaaS application that helps freelancers and agencies create winning proposals in minutes instead of hours. The system analyzes client briefs using Claude AI, generates personalized proposals, suggests optimal pricing, and automates follow-up sequences.

### Core Features

| Feature | Description | Priority |
|---------|-------------|----------|
| Brief Analyzer | AI extracts requirements from client briefs | P0 |
| Proposal Generator | AI creates complete proposals | P0 |
| Smart Pricing | AI suggests 3-tier pricing | P0 |
| Quality Scorer | AI validates proposal quality | P1 |
| Rich Editor | WYSIWYG proposal editing | P0 |
| PDF/DOCX Export | Download formatted proposals | P0 |
| Email Delivery | Send proposals directly | P1 |
| Engagement Tracking | Track opens, clicks, time spent | P1 |
| Smart Follow-ups | AI-generated follow-up sequences | P2 |
| Analytics Dashboard | Win rates, trends, insights | P2 |
| Stripe Payments | Subscription billing | P0 |

### Technology Stack

| Layer | Technology |
|-------|------------|
| Backend API | .NET 8 Web API |
| Frontend | Angular 18 + Tailwind CSS |
| Database | SQL Server 2022 |
| Cache | Redis 7 |
| AI Services | Claude API (Anthropic) |
| Email | SendGrid |
| Payments | Stripe |
| Automation | n8n |
| Storage | Azure Blob Storage |
| Hosting | Azure App Service |

---

## Progress Summary

> **Last Updated:** [DATE]  
> **Current Phase:** Pre-Development  
> **Overall Progress:** 0%

| Phase | Status | Progress | Start Date | End Date |
|-------|--------|----------|------------|----------|
| Pre-Development | 🔄 In Progress | 0/12 | - | - |
| Phase 1: Foundation | ⏳ Not Started | 0/45 | - | - |
| Phase 2: Core AI | ⏳ Not Started | 0/38 | - | - |
| Phase 3: Features | ⏳ Not Started | 0/42 | - | - |
| Phase 4: Automation | ⏳ Not Started | 0/35 | - | - |
| Phase 5: Launch | ⏳ Not Started | 0/32 | - | - |

**Legend:** ✅ Complete | 🔄 In Progress | ⏳ Not Started | ❌ Blocked

---

## Pre-Development Setup

> **Estimated Time:** 1-2 Days  
> **Status:** ⏳ Not Started  
> **Progress:** 0/12

### Accounts & API Keys

- [ ] **GitHub Account** - Create repository `ProposalPilot`
  - URL: https://github.com
  - Action: Create private repository
  
- [ ] **Anthropic Account** - Get Claude API key
  - URL: https://console.anthropic.com
  - Action: Create API key, note usage limits
  - Store as: `ANTHROPIC_API_KEY`
  
- [ ] **SendGrid Account** - Email service
  - URL: https://sendgrid.com
  - Action: Create account, verify domain, get API key
  - Store as: `SENDGRID_API_KEY`
  
- [ ] **Stripe Account** - Payment processing
  - URL: https://dashboard.stripe.com
  - Action: Create account (test mode), get API keys
  - Store as: `STRIPE_SECRET_KEY`, `STRIPE_PUBLISHABLE_KEY`
  
- [ ] **Azure Account** - Hosting & storage
  - URL: https://portal.azure.com
  - Action: Create subscription, note for later deployment

### Development Environment

- [ ] **Install .NET 8 SDK**
  ```bash
  # Verify installation
  dotnet --version  # Should show 8.x.x
  ```

- [ ] **Install Node.js 20 LTS**
  ```bash
  # Verify installation
  node --version  # Should show 20.x.x
  npm --version   # Should show 10.x.x
  ```

- [ ] **Install Angular CLI**
  ```bash
  npm install -g @angular/cli
  ng version  # Should show 18.x.x
  ```

- [ ] **Install Docker Desktop**
  ```bash
  docker --version  # Should show 24.x.x or higher
  ```

- [ ] **Install IDE**
  - Option A: Visual Studio 2022 (recommended for .NET)
  - Option B: VS Code with C# Dev Kit extension

- [ ] **Install Database Tools**
  - Azure Data Studio or SQL Server Management Studio

- [ ] **Install API Testing Tool**
  - Postman or Insomnia

### Environment Variables Template

Create `.env.template` file:
```env
# Database
DB_CONNECTION_STRING=Server=localhost;Database=ProposalPilotDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True

# AI Services
ANTHROPIC_API_KEY=sk-ant-xxxxx

# Email
SENDGRID_API_KEY=SG.xxxxx
SENDGRID_FROM_EMAIL=noreply@proposalpilot.ai

# Payments
STRIPE_SECRET_KEY=sk_test_xxxxx
STRIPE_PUBLISHABLE_KEY=pk_test_xxxxx
STRIPE_WEBHOOK_SECRET=whsec_xxxxx

# JWT
JWT_SECRET=your-256-bit-secret-key-here
JWT_ISSUER=ProposalPilot
JWT_AUDIENCE=ProposalPilotUsers

# Redis
REDIS_CONNECTION_STRING=localhost:6379

# Azure Storage
AZURE_STORAGE_CONNECTION_STRING=DefaultEndpointsProtocol=https;AccountName=xxx
```

### Pre-Development Checkpoint

- [ ] All accounts created and API keys obtained
- [ ] All development tools installed and verified
- [ ] `.env.template` created with all required variables

---

## Phase 1: Foundation (Week 1-2)

> **Estimated Time:** 10 Days  
> **Status:** ⏳ Not Started  
> **Progress:** 0/45

---

### Day 1-2: Project Setup

#### Day 1 Morning: Create Solution Structure

**Status:** ⏳ Not Started

- [ ] Create project directory and navigate to it
  ```bash
  mkdir ProposalPilot && cd ProposalPilot
  ```

- [ ] Create .NET solution
  ```bash
  dotnet new sln -n ProposalPilot
  ```

- [ ] Create API project
  ```bash
  dotnet new webapi -n ProposalPilot.API -o src/ProposalPilot.API
  dotnet sln add src/ProposalPilot.API
  ```

- [ ] Create Application project (business logic)
  ```bash
  dotnet new classlib -n ProposalPilot.Application -o src/ProposalPilot.Application
  dotnet sln add src/ProposalPilot.Application
  ```

- [ ] Create Domain project (entities)
  ```bash
  dotnet new classlib -n ProposalPilot.Domain -o src/ProposalPilot.Domain
  dotnet sln add src/ProposalPilot.Domain
  ```

- [ ] Create Infrastructure project (data access, external services)
  ```bash
  dotnet new classlib -n ProposalPilot.Infrastructure -o src/ProposalPilot.Infrastructure
  dotnet sln add src/ProposalPilot.Infrastructure
  ```

- [ ] Create Shared project (DTOs, common utilities)
  ```bash
  dotnet new classlib -n ProposalPilot.Shared -o src/ProposalPilot.Shared
  dotnet sln add src/ProposalPilot.Shared
  ```

- [ ] Set up project references
  ```bash
  # API references Application and Infrastructure
  cd src/ProposalPilot.API
  dotnet add reference ../ProposalPilot.Application
  dotnet add reference ../ProposalPilot.Infrastructure
  
  # Application references Domain and Shared
  cd ../ProposalPilot.Application
  dotnet add reference ../ProposalPilot.Domain
  dotnet add reference ../ProposalPilot.Shared
  
  # Infrastructure references Application and Domain
  cd ../ProposalPilot.Infrastructure
  dotnet add reference ../ProposalPilot.Application
  dotnet add reference ../ProposalPilot.Domain
  
  cd ../../..
  ```

- [ ] Verify solution builds
  ```bash
  dotnet build
  ```

**Acceptance Criteria:**
- Solution builds without errors
- All 5 projects created with correct references
- Clean Architecture structure in place

#### Day 1 Afternoon: Install NuGet Packages

**Status:** ⏳ Not Started

- [ ] Install API project packages
  ```bash
  cd src/ProposalPilot.API
  dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
  dotnet add package Swashbuckle.AspNetCore
  dotnet add package Serilog.AspNetCore
  dotnet add package Serilog.Sinks.Console
  dotnet add package Serilog.Sinks.File
  dotnet add package FluentValidation.AspNetCore
  dotnet add package AspNetCoreRateLimit
  dotnet add package Hangfire.AspNetCore
  dotnet add package Hangfire.SqlServer
  ```

- [ ] Install Application project packages
  ```bash
  cd ../ProposalPilot.Application
  dotnet add package MediatR
  dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
  dotnet add package FluentValidation
  ```

- [ ] Install Infrastructure project packages
  ```bash
  cd ../ProposalPilot.Infrastructure
  dotnet add package Microsoft.EntityFrameworkCore.SqlServer
  dotnet add package Microsoft.EntityFrameworkCore.Tools
  dotnet add package Microsoft.EntityFrameworkCore.Design
  dotnet add package StackExchange.Redis
  dotnet add package SendGrid
  dotnet add package Stripe.net
  dotnet add package BCrypt.Net-Next
  dotnet add package Polly
  dotnet add package QuestPDF
  ```

- [ ] Install Shared project packages
  ```bash
  cd ../ProposalPilot.Shared
  dotnet add package System.Text.Json
  ```

- [ ] Return to root and verify build
  ```bash
  cd ../../..
  dotnet build
  ```

**Acceptance Criteria:**
- All packages installed successfully
- Solution builds without package errors

#### Day 2 Morning: Create Angular Frontend

**Status:** ⏳ Not Started

- [ ] Create Angular project
  ```bash
  ng new ProposalPilot.Web --routing --style=scss --standalone --skip-git
  cd ProposalPilot.Web
  ```

- [ ] Install Angular Material
  ```bash
  ng add @angular/material
  ```

- [ ] Install Tailwind CSS
  ```bash
  npm install -D tailwindcss postcss autoprefixer
  npx tailwindcss init
  ```

- [ ] Configure Tailwind (update `tailwind.config.js`)
  ```javascript
  module.exports = {
    content: ["./src/**/*.{html,ts}"],
    theme: { extend: {} },
    plugins: [],
  }
  ```

- [ ] Add Tailwind to `src/styles.scss`
  ```scss
  @tailwind base;
  @tailwind components;
  @tailwind utilities;
  ```

- [ ] Install NgRx for state management
  ```bash
  npm install @ngrx/store @ngrx/effects @ngrx/store-devtools @ngrx/entity
  ```

- [ ] Install additional dependencies
  ```bash
  npm install ngx-quill quill
  npm install chart.js ng2-charts
  npm install @auth0/angular-jwt
  npm install date-fns
  ```

- [ ] Create folder structure
  ```bash
  mkdir -p src/app/core/services
  mkdir -p src/app/core/guards
  mkdir -p src/app/core/interceptors
  mkdir -p src/app/core/models
  mkdir -p src/app/shared/components
  mkdir -p src/app/shared/pipes
  mkdir -p src/app/shared/directives
  mkdir -p src/app/features/auth/components
  mkdir -p src/app/features/auth/services
  mkdir -p src/app/features/auth/state
  mkdir -p src/app/features/dashboard
  mkdir -p src/app/features/proposals/components
  mkdir -p src/app/features/proposals/services
  mkdir -p src/app/features/proposals/state
  mkdir -p src/app/features/templates
  mkdir -p src/app/features/analytics
  mkdir -p src/app/features/settings
  mkdir -p src/app/layouts
  mkdir -p src/app/state
  mkdir -p src/environments
  ```

- [ ] Verify Angular serves
  ```bash
  ng serve
  # Should be accessible at http://localhost:4200
  ```

**Acceptance Criteria:**
- Angular app runs at localhost:4200
- Tailwind CSS working
- Folder structure created

#### Day 2 Afternoon: Docker & Git Setup

**Status:** ⏳ Not Started

- [ ] Create `docker-compose.yml` in root
  ```yaml
  version: '3.8'
  
  services:
    sqlserver:
      image: mcr.microsoft.com/mssql/server:2022-latest
      container_name: proposalpilot-sql
      environment:
        - ACCEPT_EULA=Y
        - SA_PASSWORD=YourStrong@Passw0rd
        - MSSQL_PID=Developer
      ports:
        - "1433:1433"
      volumes:
        - sqlserver_data:/var/opt/mssql
      healthcheck:
        test: /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT 1"
        interval: 10s
        timeout: 3s
        retries: 10
  
    redis:
      image: redis:7-alpine
      container_name: proposalpilot-redis
      ports:
        - "6379:6379"
      volumes:
        - redis_data:/data
      command: redis-server --appendonly yes
  
  volumes:
    sqlserver_data:
    redis_data:
  ```

- [ ] Create `.gitignore`
  ```gitignore
  # .NET
  bin/
  obj/
  *.user
  *.suo
  .vs/
  
  # Angular
  node_modules/
  dist/
  .angular/
  
  # Environment
  .env
  appsettings.Development.json
  appsettings.Local.json
  
  # IDE
  .idea/
  .vscode/
  *.swp
  
  # OS
  .DS_Store
  Thumbs.db
  
  # Docker
  docker-compose.override.yml
  ```

- [ ] Start Docker containers
  ```bash
  docker-compose up -d
  ```

- [ ] Verify containers running
  ```bash
  docker ps
  # Should see proposalpilot-sql and proposalpilot-redis
  ```

- [ ] Initialize Git repository
  ```bash
  git init
  git add .
  git commit -m "Initial project setup with Clean Architecture"
  ```

- [ ] Create GitHub repository and push
  ```bash
  gh repo create ProposalPilot --private --source=. --push
  # Or manually create on GitHub and:
  # git remote add origin https://github.com/YOUR_USERNAME/ProposalPilot.git
  # git push -u origin main
  ```

**Acceptance Criteria:**
- Docker containers running (SQL Server + Redis)
- Can connect to SQL Server on localhost:1433
- Git repository initialized and pushed to GitHub

---

### Day 3-4: Database Design & Entity Framework

#### Day 3: Create Domain Entities

**Status:** ⏳ Not Started

- [ ] Create base entity class
  
  **File:** `src/ProposalPilot.Domain/Common/BaseEntity.cs`
  ```csharp
  namespace ProposalPilot.Domain.Common;
  
  public abstract class BaseEntity
  {
      public Guid Id { get; set; } = Guid.NewGuid();
      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      public DateTime? UpdatedAt { get; set; }
      public string? CreatedBy { get; set; }
      public string? UpdatedBy { get; set; }
      public bool IsDeleted { get; set; } = false;
  }
  ```

- [ ] Create enums
  
  **File:** `src/ProposalPilot.Domain/Enums/ProposalStatus.cs`
  ```csharp
  namespace ProposalPilot.Domain.Enums;
  
  public enum ProposalStatus
  {
      Draft = 0,
      Sent = 1,
      Viewed = 2,
      Accepted = 3,
      Rejected = 4
  }
  ```
  
  **File:** `src/ProposalPilot.Domain/Enums/PlanType.cs`
  ```csharp
  namespace ProposalPilot.Domain.Enums;
  
  public enum PlanType
  {
      Free = 0,
      Starter = 1,
      Pro = 2,
      Agency = 3,
      Enterprise = 4
  }
  ```
  
  **File:** `src/ProposalPilot.Domain/Enums/SubscriptionStatus.cs`
  ```csharp
  namespace ProposalPilot.Domain.Enums;
  
  public enum SubscriptionStatus
  {
      Active = 0,
      PastDue = 1,
      Canceled = 2,
      Trialing = 3,
      Incomplete = 4
  }
  ```

- [ ] Create User entity
  
  **File:** `src/ProposalPilot.Domain/Entities/User.cs`
  ```csharp
  namespace ProposalPilot.Domain.Entities;
  
  using ProposalPilot.Domain.Common;
  using ProposalPilot.Domain.Enums;
  
  public class User : BaseEntity
  {
      public string Email { get; set; } = string.Empty;
      public string PasswordHash { get; set; } = string.Empty;
      public string FullName { get; set; } = string.Empty;
      public bool EmailVerified { get; set; } = false;
      public string? StripeCustomerId { get; set; }
      public PlanType PlanType { get; set; } = PlanType.Free;
      public string? RefreshToken { get; set; }
      public DateTime? RefreshTokenExpiryTime { get; set; }
      
      // Navigation properties
      public UserProfile? Profile { get; set; }
      public Subscription? Subscription { get; set; }
      public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
      public ICollection<AIUsageLog> AIUsageLogs { get; set; } = new List<AIUsageLog>();
  }
  ```

- [ ] Create UserProfile entity
  
  **File:** `src/ProposalPilot.Domain/Entities/UserProfile.cs`
  ```csharp
  namespace ProposalPilot.Domain.Entities;
  
  using ProposalPilot.Domain.Common;
  
  public class UserProfile : BaseEntity
  {
      public Guid UserId { get; set; }
      public string? CompanyName { get; set; }
      public string? JobTitle { get; set; }
      public string? Bio { get; set; }
      public string? Website { get; set; }
      public string? LogoUrl { get; set; }
      public decimal HourlyRate { get; set; } = 0;
      public string? Currency { get; set; } = "USD";
      public string? Timezone { get; set; }
      public string? BrandColor { get; set; } = "#1E3A5F";
      public string? Portfolio { get; set; } // JSON array
      public string? USPs { get; set; } // JSON array - Unique Selling Points
      
      // Navigation
      public User User { get; set; } = null!;
  }
  ```

- [ ] Create Proposal entity
  
  **File:** `src/ProposalPilot.Domain/Entities/Proposal.cs`
  ```csharp
  namespace ProposalPilot.Domain.Entities;
  
  using ProposalPilot.Domain.Common;
  using ProposalPilot.Domain.Enums;
  
  public class Proposal : BaseEntity
  {
      public Guid UserId { get; set; }
      public string Title { get; set; } = string.Empty;
      public string ClientName { get; set; } = string.Empty;
      public string? ClientEmail { get; set; }
      public string? ClientCompany { get; set; }
      public ProposalStatus Status { get; set; } = ProposalStatus.Draft;
      
      // Content
      public string? OriginalBrief { get; set; }
      public string? BriefAnalysis { get; set; } // JSON
      public string? GeneratedContent { get; set; } // JSON with all sections
      public string? PricingOptions { get; set; } // JSON with tiers
      public int? QualityScore { get; set; }
      
      // Engagement
      public int ViewCount { get; set; } = 0;
      public int UniqueViewers { get; set; } = 0;
      public int EngagementScore { get; set; } = 0;
      
      // Timestamps
      public DateTime? SentAt { get; set; }
      public DateTime? FirstViewedAt { get; set; }
      public DateTime? LastViewedAt { get; set; }
      public DateTime? RespondedAt { get; set; }
      
      // Template reference
      public Guid? TemplateId { get; set; }
      
      // Navigation
      public User User { get; set; } = null!;
      public ProposalTemplate? Template { get; set; }
      public ICollection<ProposalView> Views { get; set; } = new List<ProposalView>();
      public ICollection<FollowUp> FollowUps { get; set; } = new List<FollowUp>();
      public ICollection<EmailLog> EmailLogs { get; set; } = new List<EmailLog>();
  }
  ```

- [ ] Create ProposalTemplate entity
  
  **File:** `src/ProposalPilot.Domain/Entities/ProposalTemplate.cs`
  ```csharp
  namespace ProposalPilot.Domain.Entities;
  
  using ProposalPilot.Domain.Common;
  
  public class ProposalTemplate : BaseEntity
  {
      public string Name { get; set; } = string.Empty;
      public string? Description { get; set; }
      public string Industry { get; set; } = string.Empty;
      public string Content { get; set; } = string.Empty; // JSON
      public bool IsSystem { get; set; } = false;
      public Guid? UserId { get; set; } // Null for system templates
      public int UsageCount { get; set; } = 0;
      
      // Navigation
      public User? User { get; set; }
      public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
  }
  ```

- [ ] Create ProposalView entity (engagement tracking)
  
  **File:** `src/ProposalPilot.Domain/Entities/ProposalView.cs`
  ```csharp
  namespace ProposalPilot.Domain.Entities;
  
  using ProposalPilot.Domain.Common;
  
  public class ProposalView : BaseEntity
  {
      public Guid ProposalId { get; set; }
      public string? ViewerEmail { get; set; }
      public string? ViewerIP { get; set; }
      public string? UserAgent { get; set; }
      public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
      public int DurationSeconds { get; set; } = 0;
      public string? SectionsViewed { get; set; } // JSON array
      public string? EventType { get; set; } // open, click, etc.
      
      // Navigation
      public Proposal Proposal { get; set; } = null!;
  }
  ```

- [ ] Create Subscription entity
  
  **File:** `src/ProposalPilot.Domain/Entities/Subscription.cs`
  ```csharp
  namespace ProposalPilot.Domain.Entities;
  
  using ProposalPilot.Domain.Common;
  using ProposalPilot.Domain.Enums;
  
  public class Subscription : BaseEntity
  {
      public Guid UserId { get; set; }
      public string? StripeSubscriptionId { get; set; }
      public string? StripePriceId { get; set; }
      public PlanType PlanType { get; set; } = PlanType.Free;
      public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
      public DateTime? CurrentPeriodStart { get; set; }
      public DateTime? CurrentPeriodEnd { get; set; }
      public int ProposalsUsedThisMonth { get; set; } = 0;
      public int ProposalsLimit { get; set; } = 3; // Free plan default
      public DateTime LastResetDate { get; set; } = DateTime.UtcNow;
      
      // Navigation
      public User User { get; set; } = null!;
  }
  ```

- [ ] Create AIUsageLog entity
  
  **File:** `src/ProposalPilot.Domain/Entities/AIUsageLog.cs`
  ```csharp
  namespace ProposalPilot.Domain.Entities;
  
  using ProposalPilot.Domain.Common;
  
  public class AIUsageLog : BaseEntity
  {
      public Guid UserId { get; set; }
      public Guid? ProposalId { get; set; }
      public string Model { get; set; } = string.Empty;
      public string Operation { get; set; } = string.Empty; // analyze, generate, price, score
      public int InputTokens { get; set; }
      public int OutputTokens { get; set; }
      public decimal Cost { get; set; }
      public int DurationMs { get; set; }
      public bool Success { get; set; } = true;
      public string? ErrorMessage { get; set; }
      
      // Navigation
      public User User { get; set; } = null!;
      public Proposal? Proposal { get; set; }
  }
  ```

- [ ] Create FollowUp entity
  
  **File:** `src/ProposalPilot.Domain/Entities/FollowUp.cs`
  ```csharp
  namespace ProposalPilot.Domain.Entities;
  
  using ProposalPilot.Domain.Common;
  
  public class FollowUp : BaseEntity
  {
      public Guid ProposalId { get; set; }
      public int SequenceNumber { get; set; } // 1, 2, 3, etc.
      public DateTime ScheduledFor { get; set; }
      public DateTime? SentAt { get; set; }
      public string Status { get; set; } = "pending"; // pending, sent, skipped, cancelled
      public string? Subject { get; set; }
      public string? Content { get; set; }
      public string? TriggerReason { get; set; } // no_open, opened_no_response, etc.
      
      // Navigation
      public Proposal Proposal { get; set; } = null!;
  }
  ```

- [ ] Create EmailLog entity
  
  **File:** `src/ProposalPilot.Domain/Entities/EmailLog.cs`
  ```csharp
  namespace ProposalPilot.Domain.Entities;
  
  using ProposalPilot.Domain.Common;
  
  public class EmailLog : BaseEntity
  {
      public Guid ProposalId { get; set; }
      public string RecipientEmail { get; set; } = string.Empty;
      public string Subject { get; set; } = string.Empty;
      public string EmailType { get; set; } = string.Empty; // proposal, followup
      public string Status { get; set; } = "sent"; // sent, delivered, opened, clicked, bounced
      public string? SendGridMessageId { get; set; }
      public DateTime? DeliveredAt { get; set; }
      public DateTime? OpenedAt { get; set; }
      public int OpenCount { get; set; } = 0;
      public int ClickCount { get; set; } = 0;
      
      // Navigation
      public Proposal Proposal { get; set; } = null!;
  }
  ```

**Acceptance Criteria:**
- All entity files created
- Entities have proper navigation properties
- Enums defined for status fields

#### Day 4: Create DbContext & Run Migrations

**Status:** ⏳ Not Started

- [ ] Create DbContext
  
  **File:** `src/ProposalPilot.Infrastructure/Persistence/ProposalPilotDbContext.cs`
  ```csharp
  namespace ProposalPilot.Infrastructure.Persistence;
  
  using Microsoft.EntityFrameworkCore;
  using ProposalPilot.Domain.Entities;
  
  public class ProposalPilotDbContext : DbContext
  {
      public ProposalPilotDbContext(DbContextOptions<ProposalPilotDbContext> options) 
          : base(options) { }
      
      public DbSet<User> Users => Set<User>();
      public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
      public DbSet<Proposal> Proposals => Set<Proposal>();
      public DbSet<ProposalTemplate> ProposalTemplates => Set<ProposalTemplate>();
      public DbSet<ProposalView> ProposalViews => Set<ProposalView>();
      public DbSet<Subscription> Subscriptions => Set<Subscription>();
      public DbSet<AIUsageLog> AIUsageLogs => Set<AIUsageLog>();
      public DbSet<FollowUp> FollowUps => Set<FollowUp>();
      public DbSet<EmailLog> EmailLogs => Set<EmailLog>();
      
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          base.OnModelCreating(modelBuilder);
          
          // Apply configurations
          modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProposalPilotDbContext).Assembly);
          
          // Global query filter for soft delete
          modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
          modelBuilder.Entity<Proposal>().HasQueryFilter(e => !e.IsDeleted);
          modelBuilder.Entity<ProposalTemplate>().HasQueryFilter(e => !e.IsDeleted);
      }
      
      public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
      {
          foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
          {
              switch (entry.State)
              {
                  case EntityState.Added:
                      entry.Entity.CreatedAt = DateTime.UtcNow;
                      break;
                  case EntityState.Modified:
                      entry.Entity.UpdatedAt = DateTime.UtcNow;
                      break;
              }
          }
          return base.SaveChangesAsync(cancellationToken);
      }
  }
  ```

- [ ] Create entity configurations
  
  **File:** `src/ProposalPilot.Infrastructure/Persistence/Configurations/UserConfiguration.cs`
  ```csharp
  namespace ProposalPilot.Infrastructure.Persistence.Configurations;
  
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;
  using ProposalPilot.Domain.Entities;
  
  public class UserConfiguration : IEntityTypeConfiguration<User>
  {
      public void Configure(EntityTypeBuilder<User> builder)
      {
          builder.HasKey(e => e.Id);
          
          builder.Property(e => e.Email)
              .IsRequired()
              .HasMaxLength(256);
          
          builder.HasIndex(e => e.Email)
              .IsUnique();
          
          builder.Property(e => e.FullName)
              .IsRequired()
              .HasMaxLength(200);
          
          builder.HasOne(e => e.Profile)
              .WithOne(e => e.User)
              .HasForeignKey<UserProfile>(e => e.UserId);
          
          builder.HasOne(e => e.Subscription)
              .WithOne(e => e.User)
              .HasForeignKey<Subscription>(e => e.UserId);
      }
  }
  ```

  **File:** `src/ProposalPilot.Infrastructure/Persistence/Configurations/ProposalConfiguration.cs`
  ```csharp
  namespace ProposalPilot.Infrastructure.Persistence.Configurations;
  
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Metadata.Builders;
  using ProposalPilot.Domain.Entities;
  
  public class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
  {
      public void Configure(EntityTypeBuilder<Proposal> builder)
      {
          builder.HasKey(e => e.Id);
          
          builder.Property(e => e.Title)
              .IsRequired()
              .HasMaxLength(500);
          
          builder.Property(e => e.ClientName)
              .IsRequired()
              .HasMaxLength(200);
          
          builder.HasIndex(e => e.UserId);
          builder.HasIndex(e => e.Status);
          builder.HasIndex(e => e.CreatedAt);
          
          builder.HasOne(e => e.User)
              .WithMany(e => e.Proposals)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);
      }
  }
  ```

- [ ] Configure DbContext in API
  
  **Update:** `src/ProposalPilot.API/Program.cs`
  ```csharp
  // Add after builder.Services...
  builder.Services.AddDbContext<ProposalPilotDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
  ```

- [ ] Add connection string to appsettings
  
  **Update:** `src/ProposalPilot.API/appsettings.Development.json`
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=ProposalPilotDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
    }
  }
  ```

- [ ] Create and run migrations
  ```bash
  cd src/ProposalPilot.API
  dotnet ef migrations add InitialCreate -p ../ProposalPilot.Infrastructure -o Persistence/Migrations
  dotnet ef database update
  ```

- [ ] Verify database created
  - Connect to SQL Server using Azure Data Studio
  - Verify all tables exist
  - Check indexes are created

**Acceptance Criteria:**
- Database `ProposalPilotDB` created
- All tables exist with correct columns
- Indexes created on key columns
- Can query database from application

---

### Day 5-7: Authentication System

#### Day 5: JWT Configuration & Auth Service

**Status:** ⏳ Not Started

- [ ] Create JWT settings class
  
  **File:** `src/ProposalPilot.Shared/Configuration/JwtSettings.cs`
  ```csharp
  namespace ProposalPilot.Shared.Configuration;
  
  public class JwtSettings
  {
      public string Secret { get; set; } = string.Empty;
      public string Issuer { get; set; } = string.Empty;
      public string Audience { get; set; } = string.Empty;
      public int AccessTokenExpirationMinutes { get; set; } = 15;
      public int RefreshTokenExpirationDays { get; set; } = 7;
  }
  ```

- [ ] Create IAuthService interface
  
  **File:** `src/ProposalPilot.Application/Interfaces/IAuthService.cs`
  ```csharp
  namespace ProposalPilot.Application.Interfaces;
  
  using ProposalPilot.Domain.Entities;
  
  public interface IAuthService
  {
      Task<(User user, string accessToken, string refreshToken)> RegisterAsync(
          string email, string password, string fullName);
      Task<(User user, string accessToken, string refreshToken)?> LoginAsync(
          string email, string password);
      Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken);
      Task<bool> RevokeTokenAsync(Guid userId);
      string HashPassword(string password);
      bool VerifyPassword(string password, string hash);
  }
  ```

- [ ] Create ICurrentUserService interface
  
  **File:** `src/ProposalPilot.Application/Interfaces/ICurrentUserService.cs`
  ```csharp
  namespace ProposalPilot.Application.Interfaces;
  
  public interface ICurrentUserService
  {
      Guid? UserId { get; }
      string? Email { get; }
      bool IsAuthenticated { get; }
  }
  ```

- [ ] Implement AuthService
  
  **File:** `src/ProposalPilot.Infrastructure/Services/AuthService.cs`
  ```csharp
  namespace ProposalPilot.Infrastructure.Services;
  
  using System.IdentityModel.Tokens.Jwt;
  using System.Security.Claims;
  using System.Security.Cryptography;
  using System.Text;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Options;
  using Microsoft.IdentityModel.Tokens;
  using ProposalPilot.Application.Interfaces;
  using ProposalPilot.Domain.Entities;
  using ProposalPilot.Infrastructure.Persistence;
  using ProposalPilot.Shared.Configuration;
  
  public class AuthService : IAuthService
  {
      private readonly ProposalPilotDbContext _context;
      private readonly JwtSettings _jwtSettings;
      
      public AuthService(ProposalPilotDbContext context, IOptions<JwtSettings> jwtSettings)
      {
          _context = context;
          _jwtSettings = jwtSettings.Value;
      }
      
      public async Task<(User user, string accessToken, string refreshToken)> RegisterAsync(
          string email, string password, string fullName)
      {
          // Check if user exists
          if (await _context.Users.AnyAsync(u => u.Email == email.ToLower()))
              throw new InvalidOperationException("User with this email already exists");
          
          var user = new User
          {
              Email = email.ToLower(),
              PasswordHash = HashPassword(password),
              FullName = fullName
          };
          
          // Create default profile
          user.Profile = new UserProfile { UserId = user.Id };
          
          // Create default subscription (Free)
          user.Subscription = new Subscription 
          { 
              UserId = user.Id,
              ProposalsLimit = 3
          };
          
          _context.Users.Add(user);
          
          var (accessToken, refreshToken) = GenerateTokens(user);
          user.RefreshToken = refreshToken;
          user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
          
          await _context.SaveChangesAsync();
          
          return (user, accessToken, refreshToken);
      }
      
      public async Task<(User user, string accessToken, string refreshToken)?> LoginAsync(
          string email, string password)
      {
          var user = await _context.Users
              .Include(u => u.Profile)
              .Include(u => u.Subscription)
              .FirstOrDefaultAsync(u => u.Email == email.ToLower());
          
          if (user == null || !VerifyPassword(password, user.PasswordHash))
              return null;
          
          var (accessToken, refreshToken) = GenerateTokens(user);
          user.RefreshToken = refreshToken;
          user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
          
          await _context.SaveChangesAsync();
          
          return (user, accessToken, refreshToken);
      }
      
      public async Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken)
      {
          var user = await _context.Users
              .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
          
          if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
              return null;
          
          var (newAccessToken, newRefreshToken) = GenerateTokens(user);
          user.RefreshToken = newRefreshToken;
          user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
          
          await _context.SaveChangesAsync();
          
          return (newAccessToken, newRefreshToken);
      }
      
      public async Task<bool> RevokeTokenAsync(Guid userId)
      {
          var user = await _context.Users.FindAsync(userId);
          if (user == null) return false;
          
          user.RefreshToken = null;
          user.RefreshTokenExpiryTime = null;
          await _context.SaveChangesAsync();
          
          return true;
      }
      
      public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
      
      public bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
      
      private (string accessToken, string refreshToken) GenerateTokens(User user)
      {
          var accessToken = GenerateAccessToken(user);
          var refreshToken = GenerateRefreshToken();
          return (accessToken, refreshToken);
      }
      
      private string GenerateAccessToken(User user)
      {
          var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
          var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
          
          var claims = new[]
          {
              new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
              new Claim(ClaimTypes.Email, user.Email),
              new Claim(ClaimTypes.Name, user.FullName),
              new Claim("plan", user.PlanType.ToString())
          };
          
          var token = new JwtSecurityToken(
              issuer: _jwtSettings.Issuer,
              audience: _jwtSettings.Audience,
              claims: claims,
              expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
              signingCredentials: credentials
          );
          
          return new JwtSecurityTokenHandler().WriteToken(token);
      }
      
      private static string GenerateRefreshToken()
      {
          var randomNumber = new byte[64];
          using var rng = RandomNumberGenerator.Create();
          rng.GetBytes(randomNumber);
          return Convert.ToBase64String(randomNumber);
      }
  }
  ```

- [ ] Implement CurrentUserService
  
  **File:** `src/ProposalPilot.Infrastructure/Services/CurrentUserService.cs`
  ```csharp
  namespace ProposalPilot.Infrastructure.Services;
  
  using System.Security.Claims;
  using Microsoft.AspNetCore.Http;
  using ProposalPilot.Application.Interfaces;
  
  public class CurrentUserService : ICurrentUserService
  {
      private readonly IHttpContextAccessor _httpContextAccessor;
      
      public CurrentUserService(IHttpContextAccessor httpContextAccessor)
      {
          _httpContextAccessor = httpContextAccessor;
      }
      
      public Guid? UserId
      {
          get
          {
              var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
              return id != null ? Guid.Parse(id) : null;
          }
      }
      
      public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
      
      public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
  }
  ```

- [ ] Configure JWT in Program.cs
  
  **Update:** `src/ProposalPilot.API/Program.cs`
  ```csharp
  // Add JWT configuration
  builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
  
  var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
  
  builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
          options.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              ValidIssuer = jwtSettings.Issuer,
              ValidAudience = jwtSettings.Audience,
              IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(jwtSettings.Secret))
          };
      });
  
  builder.Services.AddAuthorization();
  
  // Register services
  builder.Services.AddScoped<IAuthService, AuthService>();
  builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
  builder.Services.AddHttpContextAccessor();
  ```

- [ ] Add JWT settings to appsettings
  
  **Update:** `src/ProposalPilot.API/appsettings.Development.json`
  ```json
  {
    "Jwt": {
      "Secret": "your-256-bit-secret-key-here-make-it-long-enough",
      "Issuer": "ProposalPilot",
      "Audience": "ProposalPilotUsers",
      "AccessTokenExpirationMinutes": 15,
      "RefreshTokenExpirationDays": 7
    }
  }
  ```

**Acceptance Criteria:**
- JWT settings configured
- AuthService can hash/verify passwords
- AuthService can generate access and refresh tokens

#### Day 6: Auth Controller & DTOs

**Status:** ⏳ Not Started

- [ ] Create Auth DTOs
  
  **File:** `src/ProposalPilot.Shared/DTOs/Auth/RegisterRequest.cs`
  ```csharp
  namespace ProposalPilot.Shared.DTOs.Auth;
  
  public record RegisterRequest(
      string Email,
      string Password,
      string FullName
  );
  ```
  
  **File:** `src/ProposalPilot.Shared/DTOs/Auth/LoginRequest.cs`
  ```csharp
  namespace ProposalPilot.Shared.DTOs.Auth;
  
  public record LoginRequest(
      string Email,
      string Password
  );
  ```
  
  **File:** `src/ProposalPilot.Shared/DTOs/Auth/AuthResponse.cs`
  ```csharp
  namespace ProposalPilot.Shared.DTOs.Auth;
  
  public record AuthResponse(
      Guid UserId,
      string Email,
      string FullName,
      string AccessToken,
      string RefreshToken,
      DateTime ExpiresAt
  );
  ```
  
  **File:** `src/ProposalPilot.Shared/DTOs/Auth/RefreshTokenRequest.cs`
  ```csharp
  namespace ProposalPilot.Shared.DTOs.Auth;
  
  public record RefreshTokenRequest(string RefreshToken);
  ```

- [ ] Create Auth Validators
  
  **File:** `src/ProposalPilot.Application/Validators/RegisterRequestValidator.cs`
  ```csharp
  namespace ProposalPilot.Application.Validators;
  
  using FluentValidation;
  using ProposalPilot.Shared.DTOs.Auth;
  
  public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
  {
      public RegisterRequestValidator()
      {
          RuleFor(x => x.Email)
              .NotEmpty().WithMessage("Email is required")
              .EmailAddress().WithMessage("Invalid email format");
          
          RuleFor(x => x.Password)
              .NotEmpty().WithMessage("Password is required")
              .MinimumLength(8).WithMessage("Password must be at least 8 characters")
              .Matches("[A-Z]").WithMessage("Password must contain uppercase letter")
              .Matches("[a-z]").WithMessage("Password must contain lowercase letter")
              .Matches("[0-9]").WithMessage("Password must contain a number");
          
          RuleFor(x => x.FullName)
              .NotEmpty().WithMessage("Full name is required")
              .MaximumLength(200).WithMessage("Full name too long");
      }
  }
  ```

- [ ] Create AuthController
  
  **File:** `src/ProposalPilot.API/Controllers/AuthController.cs`
  ```csharp
  namespace ProposalPilot.API.Controllers;
  
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using ProposalPilot.Application.Interfaces;
  using ProposalPilot.Shared.DTOs.Auth;
  
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
      private readonly IAuthService _authService;
      private readonly ICurrentUserService _currentUserService;
      
      public AuthController(IAuthService authService, ICurrentUserService currentUserService)
      {
          _authService = authService;
          _currentUserService = currentUserService;
      }
      
      [HttpPost("register")]
      public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
      {
          try
          {
              var (user, accessToken, refreshToken) = await _authService.RegisterAsync(
                  request.Email, request.Password, request.FullName);
              
              return Ok(new AuthResponse(
                  user.Id,
                  user.Email,
                  user.FullName,
                  accessToken,
                  refreshToken,
                  DateTime.UtcNow.AddMinutes(15)
              ));
          }
          catch (InvalidOperationException ex)
          {
              return BadRequest(new { message = ex.Message });
          }
      }
      
      [HttpPost("login")]
      public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
      {
          var result = await _authService.LoginAsync(request.Email, request.Password);
          
          if (result == null)
              return Unauthorized(new { message = "Invalid email or password" });
          
          var (user, accessToken, refreshToken) = result.Value;
          
          return Ok(new AuthResponse(
              user.Id,
              user.Email,
              user.FullName,
              accessToken,
              refreshToken,
              DateTime.UtcNow.AddMinutes(15)
          ));
      }
      
      [HttpPost("refresh-token")]
      public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
      {
          var result = await _authService.RefreshTokenAsync(request.RefreshToken);
          
          if (result == null)
              return Unauthorized(new { message = "Invalid or expired refresh token" });
          
          var (accessToken, refreshToken) = result.Value;
          
          return Ok(new { accessToken, refreshToken, expiresAt = DateTime.UtcNow.AddMinutes(15) });
      }
      
      [Authorize]
      [HttpPost("logout")]
      public async Task<IActionResult> Logout()
      {
          if (_currentUserService.UserId.HasValue)
              await _authService.RevokeTokenAsync(_currentUserService.UserId.Value);
          
          return Ok(new { message = "Logged out successfully" });
      }
      
      [Authorize]
      [HttpGet("me")]
      public IActionResult GetCurrentUser()
      {
          return Ok(new
          {
              userId = _currentUserService.UserId,
              email = _currentUserService.Email
          });
      }
  }
  ```

- [ ] Test auth endpoints with Postman
  - POST /api/auth/register
  - POST /api/auth/login
  - POST /api/auth/refresh-token
  - GET /api/auth/me (with Bearer token)
  - POST /api/auth/logout

**Acceptance Criteria:**
- Can register new user
- Can login and receive tokens
- Can refresh access token
- Protected endpoints require valid token

#### Day 7: Angular Auth Module

**Status:** ⏳ Not Started

- [ ] Create Angular auth models
  
  **File:** `src/app/core/models/auth.model.ts`
  ```typescript
  export interface User {
    id: string;
    email: string;
    fullName: string;
  }
  
  export interface LoginRequest {
    email: string;
    password: string;
  }
  
  export interface RegisterRequest {
    email: string;
    password: string;
    fullName: string;
  }
  
  export interface AuthResponse {
    userId: string;
    email: string;
    fullName: string;
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
  }
  ```

- [ ] Create AuthService
  
  **File:** `src/app/core/services/auth.service.ts`
  ```typescript
  import { Injectable } from '@angular/core';
  import { HttpClient } from '@angular/common/http';
  import { Observable, BehaviorSubject, tap } from 'rxjs';
  import { environment } from '../../../environments/environment';
  import { AuthResponse, LoginRequest, RegisterRequest, User } from '../models/auth.model';
  
  @Injectable({ providedIn: 'root' })
  export class AuthService {
    private readonly API_URL = `${environment.apiUrl}/auth`;
    private currentUserSubject = new BehaviorSubject<User | null>(null);
    
    currentUser$ = this.currentUserSubject.asObservable();
    
    constructor(private http: HttpClient) {
      this.loadUserFromStorage();
    }
    
    login(request: LoginRequest): Observable<AuthResponse> {
      return this.http.post<AuthResponse>(`${this.API_URL}/login`, request).pipe(
        tap(response => this.handleAuthResponse(response))
      );
    }
    
    register(request: RegisterRequest): Observable<AuthResponse> {
      return this.http.post<AuthResponse>(`${this.API_URL}/register`, request).pipe(
        tap(response => this.handleAuthResponse(response))
      );
    }
    
    logout(): void {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
      this.currentUserSubject.next(null);
    }
    
    refreshToken(): Observable<{ accessToken: string; refreshToken: string }> {
      const refreshToken = localStorage.getItem('refreshToken');
      return this.http.post<{ accessToken: string; refreshToken: string }>(
        `${this.API_URL}/refresh-token`,
        { refreshToken }
      ).pipe(
        tap(response => {
          localStorage.setItem('accessToken', response.accessToken);
          localStorage.setItem('refreshToken', response.refreshToken);
        })
      );
    }
    
    getAccessToken(): string | null {
      return localStorage.getItem('accessToken');
    }
    
    isAuthenticated(): boolean {
      return !!this.getAccessToken();
    }
    
    private handleAuthResponse(response: AuthResponse): void {
      localStorage.setItem('accessToken', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);
      
      const user: User = {
        id: response.userId,
        email: response.email,
        fullName: response.fullName
      };
      localStorage.setItem('user', JSON.stringify(user));
      this.currentUserSubject.next(user);
    }
    
    private loadUserFromStorage(): void {
      const userJson = localStorage.getItem('user');
      if (userJson) {
        this.currentUserSubject.next(JSON.parse(userJson));
      }
    }
  }
  ```

- [ ] Create Auth Interceptor
  
  **File:** `src/app/core/interceptors/auth.interceptor.ts`
  ```typescript
  import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
  import { inject } from '@angular/core';
  import { Router } from '@angular/router';
  import { catchError, throwError } from 'rxjs';
  import { AuthService } from '../services/auth.service';
  
  export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    
    const token = authService.getAccessToken();
    
    if (token) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }
    
    return next(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          authService.logout();
          router.navigate(['/auth/login']);
        }
        return throwError(() => error);
      })
    );
  };
  ```

- [ ] Create Auth Guard
  
  **File:** `src/app/core/guards/auth.guard.ts`
  ```typescript
  import { inject } from '@angular/core';
  import { Router, CanActivateFn } from '@angular/router';
  import { AuthService } from '../services/auth.service';
  
  export const authGuard: CanActivateFn = () => {
    const authService = inject(AuthService);
    const router = inject(Router);
    
    if (authService.isAuthenticated()) {
      return true;
    }
    
    router.navigate(['/auth/login']);
    return false;
  };
  
  export const guestGuard: CanActivateFn = () => {
    const authService = inject(AuthService);
    const router = inject(Router);
    
    if (!authService.isAuthenticated()) {
      return true;
    }
    
    router.navigate(['/dashboard']);
    return false;
  };
  ```

- [ ] Create LoginComponent
  
  **File:** `src/app/features/auth/components/login/login.component.ts`
  ```typescript
  import { Component } from '@angular/core';
  import { CommonModule } from '@angular/common';
  import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
  import { Router, RouterLink } from '@angular/router';
  import { AuthService } from '../../../../core/services/auth.service';
  
  @Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterLink],
    template: `
      <div class="min-h-screen flex items-center justify-center bg-gray-50">
        <div class="max-w-md w-full p-8 bg-white rounded-lg shadow-md">
          <h2 class="text-2xl font-bold text-center text-gray-900 mb-8">
            Sign in to ProposalPilot
          </h2>
          
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <div class="mb-4">
              <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
              <input type="email" formControlName="email"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500">
              @if (form.get('email')?.touched && form.get('email')?.invalid) {
                <p class="text-red-500 text-sm mt-1">Valid email is required</p>
              }
            </div>
            
            <div class="mb-6">
              <label class="block text-sm font-medium text-gray-700 mb-1">Password</label>
              <input type="password" formControlName="password"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500">
              @if (form.get('password')?.touched && form.get('password')?.invalid) {
                <p class="text-red-500 text-sm mt-1">Password is required</p>
              }
            </div>
            
            @if (error) {
              <div class="mb-4 p-3 bg-red-100 text-red-700 rounded">{{ error }}</div>
            }
            
            <button type="submit" [disabled]="loading || form.invalid"
              class="w-full py-2 px-4 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50">
              {{ loading ? 'Signing in...' : 'Sign In' }}
            </button>
          </form>
          
          <p class="mt-4 text-center text-sm text-gray-600">
            Don't have an account? 
            <a routerLink="/auth/register" class="text-blue-600 hover:underline">Sign up</a>
          </p>
        </div>
      </div>
    `
  })
  export class LoginComponent {
    form: FormGroup;
    loading = false;
    error = '';
    
    constructor(
      private fb: FormBuilder,
      private authService: AuthService,
      private router: Router
    ) {
      this.form = this.fb.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', Validators.required]
      });
    }
    
    onSubmit(): void {
      if (this.form.invalid) return;
      
      this.loading = true;
      this.error = '';
      
      this.authService.login(this.form.value).subscribe({
        next: () => {
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.error = err.error?.message || 'Login failed';
          this.loading = false;
        }
      });
    }
  }
  ```

- [ ] Create RegisterComponent (similar structure)

- [ ] Configure routing
  
  **File:** `src/app/app.routes.ts`
  ```typescript
  import { Routes } from '@angular/router';
  import { authGuard, guestGuard } from './core/guards/auth.guard';
  
  export const routes: Routes = [
    { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
    {
      path: 'auth',
      canActivate: [guestGuard],
      children: [
        { path: 'login', loadComponent: () => import('./features/auth/components/login/login.component').then(m => m.LoginComponent) },
        { path: 'register', loadComponent: () => import('./features/auth/components/register/register.component').then(m => m.RegisterComponent) }
      ]
    },
    {
      path: 'dashboard',
      canActivate: [authGuard],
      loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
    }
  ];
  ```

- [ ] Configure app.config.ts
  ```typescript
  import { ApplicationConfig } from '@angular/core';
  import { provideRouter } from '@angular/router';
  import { provideHttpClient, withInterceptors } from '@angular/common/http';
  import { routes } from './app.routes';
  import { authInterceptor } from './core/interceptors/auth.interceptor';
  
  export const appConfig: ApplicationConfig = {
    providers: [
      provideRouter(routes),
      provideHttpClient(withInterceptors([authInterceptor]))
    ]
  };
  ```

- [ ] Test complete auth flow in browser

**Acceptance Criteria:**
- Can register from Angular app
- Can login and be redirected to dashboard
- Protected routes redirect to login
- Token attached to API requests

### Phase 1 Checkpoint

- [ ] **Solution structure complete** - All projects created with proper references
- [ ] **Database working** - All tables created, can query
- [ ] **Authentication working** - Register, login, token refresh
- [ ] **Angular app running** - Auth flow working end-to-end
- [ ] **Git repository** - All code committed and pushed

---

### Day 8-10: User Management & Profiles

**Status:** ⏳ Not Started

*(Detailed tasks similar to above - continuing the pattern)*

- [ ] Create UserController with profile endpoints
- [ ] Create User DTOs (UserProfileDto, UpdateProfileRequest)
- [ ] Create MediatR commands and queries for user operations
- [ ] Implement profile image upload to Azure Blob
- [ ] Create Angular ProfileComponent
- [ ] Create Angular SettingsComponent
- [ ] Test complete profile management flow

---

## Phase 2: Core AI Features (Week 3-4)

> **Estimated Time:** 10 Days  
> **Status:** ⏳ Not Started  
> **Progress:** 0/38

*(Continue with same detailed format for remaining phases)*

### Day 11-12: Claude API Integration

- [ ] Create IClaudeApiService interface
- [ ] Implement ClaudeApiService with HttpClient
- [ ] Configure retry policy with Polly
- [ ] Implement response caching with Redis
- [ ] Create token counting utility
- [ ] Add cost calculation per request
- [ ] Test AI integration with simple prompt

### Day 13-15: Brief Analyzer Feature

- [ ] Create BriefAnalysisController
- [ ] Create BriefAnalysisRequest/Response DTOs
- [ ] Implement BriefAnalyzerService
- [ ] Add Brief Analyzer system prompt
- [ ] Store analysis in database
- [ ] Create Angular BriefInputComponent
- [ ] Create Angular BriefAnalysisResultComponent
- [ ] Test complete brief analysis flow

### Day 16-18: Proposal Generator

- [ ] Create ProposalGeneratorService
- [ ] Add Proposal Generator system prompt
- [ ] Create ProposalsController with CRUD
- [ ] Implement proposal generation flow
- [ ] Create Angular proposal generation UI
- [ ] Test complete proposal generation

### Day 19-20: Pricing Engine

- [ ] Create PricingService
- [ ] Add Pricing Suggester prompt
- [ ] Create pricing endpoint
- [ ] Create Angular PricingEditorComponent
- [ ] Test pricing suggestions

---

## Phase 3: Essential Features (Week 5-6)

> **Estimated Time:** 10 Days  
> **Status:** ⏳ Not Started  
> **Progress:** 0/42

### Day 21-23: Proposal Editor

- [ ] Create ProposalEditorComponent framework
- [ ] Integrate Quill.js rich text editor
- [ ] Implement section navigation
- [ ] Add auto-save functionality
- [ ] Create preview mode
- [ ] Add "Improve with AI" per section

### Day 24-25: Quality Scorer

- [ ] Create QualityScorerService
- [ ] Add Quality Scorer prompt
- [ ] Create scoring endpoint
- [ ] Create Angular QualityScoreComponent
- [ ] Integrate into editor sidebar

### Day 26-27: PDF/DOCX Export

- [ ] Implement PDF generation with QuestPDF
- [ ] Implement DOCX generation
- [ ] Create download endpoints
- [ ] Add download buttons in Angular
- [ ] Test export with formatting

### Day 28-30: Stripe Integration

- [ ] Create Stripe products and prices
- [ ] Implement StripeService
- [ ] Create checkout and portal endpoints
- [ ] Implement webhook handler
- [ ] Create subscription enforcement middleware
- [ ] Create Angular pricing page
- [ ] Test complete payment flow

---

## Phase 4: Automation & Tracking (Week 7-8)

> **Estimated Time:** 10 Days  
> **Status:** ⏳ Not Started  
> **Progress:** 0/35

### Day 31-33: Email Service & Delivery

- [ ] Set up SendGrid account and domain
- [ ] Implement SendGridEmailService
- [ ] Create proposal email delivery
- [ ] Create send proposal UI
- [ ] Test email delivery

### Day 34-36: Engagement Tracking

- [ ] Create SendGrid webhook handler
- [ ] Implement engagement score calculation
- [ ] Store view events in database
- [ ] Create engagement UI component
- [ ] Test tracking end-to-end

### Day 37-38: Follow-up System

- [ ] Create FollowUp entity and endpoints
- [ ] Implement follow-up scheduling
- [ ] Create Hangfire job for sending
- [ ] Create follow-up UI
- [ ] Test automated follow-ups

### Day 39-40: n8n Workflows

- [ ] Deploy n8n instance
- [ ] Create proposal delivery workflow
- [ ] Create engagement tracking workflow
- [ ] Create follow-up workflow
- [ ] Test automation end-to-end

---

## Phase 5: Polish & Launch (Week 9-10)

> **Estimated Time:** 10 Days
> **Status:** 🔄 In Progress
> **Progress:** 5/32

### Day 41-43: Analytics Dashboard

- [x] Create AnalyticsService
- [x] Create analytics endpoints
- [x] Create Angular AnalyticsDashboardComponent
- [x] Add charts and visualizations
- [x] Test analytics data

### Day 44-45: Testing & Bug Fixes

- [x] Write unit tests for services
  - Created ProposalPilot.Tests.Unit project with xUnit, FluentAssertions, Moq
  - 7 unit tests for AuthService (register, login, refresh token, duplicate email)
- [x] Write integration tests for controllers
  - Created ProposalPilot.Tests.Integration project with WebApplicationFactory
  - 9 integration tests (AuthController: register, login, token validation, health checks)
  - CustomWebApplicationFactory with in-memory database and cache
- [x] Write Angular component tests
  - Configured Angular 21 native Vitest integration (@angular/build:unit-test)
  - 21 component/service tests:
    - App component tests (2)
    - AuthService tests (9)
    - LoginComponent tests (10)
- [x] Fixed package version conflicts (JWT libraries, EF Core)
- [x] Fixed integration test environment configuration
- [ ] E2E tests for critical flows (deferred - core tests complete)
- [x] Fix all critical bugs

### Day 46-47: Documentation

- [ ] Complete API documentation (Swagger)
- [ ] Write README with setup instructions
- [ ] Create user guide
- [ ] Record demo videos

### Day 48-50: Deployment & Launch

- [ ] Create Azure resources
- [ ] Configure production environment
- [ ] Set up CI/CD pipeline
- [ ] Deploy to staging
- [ ] Run smoke tests
- [ ] Deploy to production
- [ ] Enable live Stripe
- [ ] Monitor and fix issues
- [ ] Announce launch

---

## Post-Launch Tasks

- [ ] Monitor error rates daily
- [ ] Respond to user feedback within 24h
- [ ] Fix critical bugs same day
- [ ] Track AI API costs
- [ ] Weekly performance review
- [ ] Plan Phase 2 features

---

## Technical Specifications

### API Endpoints Summary

| Endpoint | Method | Description | Auth |
|----------|--------|-------------|------|
| `/api/auth/register` | POST | Register new user | No |
| `/api/auth/login` | POST | Login user | No |
| `/api/auth/refresh-token` | POST | Refresh access token | No |
| `/api/auth/me` | GET | Get current user | Yes |
| `/api/users/profile` | GET/PUT | User profile | Yes |
| `/api/analysis/analyze-brief` | POST | Analyze client brief | Yes |
| `/api/proposals` | GET/POST | List/Create proposals | Yes |
| `/api/proposals/{id}` | GET/PUT/DELETE | Single proposal | Yes |
| `/api/proposals/{id}/generate` | POST | Generate with AI | Yes |
| `/api/proposals/{id}/send` | POST | Send to client | Yes |
| `/api/proposals/{id}/pdf` | GET | Download PDF | Yes |
| `/api/subscriptions/checkout` | POST | Create checkout | Yes |
| `/api/webhooks/stripe` | POST | Stripe events | No |
| `/api/webhooks/sendgrid` | POST | Email events | No |

### Database Schema

See Entity files in `ProposalPilot.Domain/Entities/` for complete schema.

### Environment Variables

See `.env.template` in Pre-Development section.

---

## File Structure Reference

```
ProposalPilot/
├── src/
│   ├── ProposalPilot.API/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── ProposalPilot.Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── Validators/
│   ├── ProposalPilot.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Common/
│   ├── ProposalPilot.Infrastructure/
│   │   ├── Persistence/
│   │   ├── Services/
│   │   └── External/
│   └── ProposalPilot.Shared/
│       ├── DTOs/
│       └── Configuration/
├── ProposalPilot.Web/
│   └── src/app/
│       ├── core/
│       ├── shared/
│       ├── features/
│       ├── layouts/
│       └── state/
├── docker-compose.yml
├── .gitignore
├── PROJECTPLAN.md
└── README.md
```

---

## Notes & Decisions Log

| Date | Decision | Rationale |
|------|----------|-----------|
| - | Using Clean Architecture | Separation of concerns, testability |
| - | Using MediatR for CQRS | Decouples handlers from controllers |
| - | Using Claude Haiku for analysis | Cost-effective for extraction tasks |
| - | Using Claude Sonnet for generation | Better creative writing quality |
| - | Using QuestPDF for PDF | .NET native, no external dependencies |

---

## Resources & Links

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Angular 18 Documentation](https://angular.dev/)
- [Claude API Documentation](https://docs.anthropic.com/)
- [Stripe API Documentation](https://stripe.com/docs/api)
- [SendGrid Documentation](https://docs.sendgrid.com/)
- [n8n Documentation](https://docs.n8n.io/)

---

*Last updated: [DATE]*  
*Next review: [DATE]*
