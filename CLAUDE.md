# ProposalPilot AI - Claude Code Instructions

> **Read this file first.** This contains all context needed to build ProposalPilot AI.

## Project Overview

ProposalPilot AI is an AI-powered SaaS platform that helps freelancers and agencies create winning proposals in minutes. Users paste a client brief, AI analyzes it, generates a personalized proposal with 3-tier pricing, and tracks engagement.

**Core Promise:** Turn client briefs into winning proposals in 5 minutes, not 5 hours.

## Technology Stack

| Layer | Technology | Version |
|-------|------------|---------|
| Backend | .NET 8 Web API | 8.x |
| Frontend | Angular | 18.x |
| Database | SQL Server | 2022 |
| Cache | Redis | 7.x |
| AI | Claude API (Anthropic) | Latest |
| Email | SendGrid | - |
| Payments | Stripe | - |
| CSS | Tailwind CSS | 3.x |

## Architecture

**Clean Architecture with 5 projects:**

```
ProposalPilot/
├── src/
│   ├── ProposalPilot.API/           # Web API, Controllers, Middleware
│   ├── ProposalPilot.Application/   # Business logic, CQRS, MediatR
│   ├── ProposalPilot.Domain/        # Entities, Enums, Interfaces
│   ├── ProposalPilot.Infrastructure/# EF Core, External services
│   └── ProposalPilot.Shared/        # DTOs, Configuration
├── ProposalPilot.Web/               # Angular 18 frontend
├── docker-compose.yml
└── PROJECTPLAN.md
```

**Project References:**
- API → Application, Infrastructure
- Application → Domain, Shared
- Infrastructure → Application, Domain
- Domain → (no dependencies)
- Shared → (no dependencies)

## Key Files to Reference

| File | Contains |
|------|----------|
| `PROJECTPLAN.md` | Day-by-day development tasks with checkboxes |
| `AI_PROMPTS.md` | All Claude AI system prompts |
| `TECHNICAL_SPECS.md` | Database schema, API endpoints, configurations |
| `SETUP_COMMANDS.md` | All CLI commands for setup |

## Development Workflow

1. **Follow PROJECTPLAN.md sequentially** - It's organized by day (Day 1, Day 2, etc.)
2. **Check off tasks** as you complete them (change `- [ ]` to `- [x]`)
3. **Test each feature** before moving to the next day
4. **Commit frequently** with descriptive messages

## Current Phase

**Phase 1: Foundation (Week 1-2)**
- Day 1-2: Project Setup (solution structure, packages, Angular)
- Day 3-4: Database & Entity Framework
- Day 5-7: Authentication (JWT)
- Day 8-10: User Management & Profiles

## Coding Standards

### Backend (.NET)
- Use async/await for all I/O operations
- Use MediatR for CQRS pattern (Commands/Queries)
- Use FluentValidation for request validation
- Use AutoMapper for DTO mapping
- Use Serilog for structured logging
- Follow REST conventions for API endpoints
- Return appropriate HTTP status codes
- Use `IResult` or `ActionResult<T>` for controller returns

### Frontend (Angular)
- Use standalone components (not NgModules)
- Use signals for reactive state where appropriate
- Use NgRx for global state management
- Use Tailwind CSS for styling
- Use reactive forms with validation
- Create reusable components in `shared/`
- Feature-based folder structure

### Database
- Use GUID for all primary keys
- Include audit fields: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`
- Implement soft delete with `IsDeleted` flag
- Use migrations for schema changes
- Add indexes for frequently queried columns

### Git Commits
- `feat:` for new features
- `fix:` for bug fixes
- `refactor:` for code changes
- `docs:` for documentation
- `chore:` for maintenance

## Environment Variables

Create `.env` file with:
```
DB_CONNECTION_STRING=Server=localhost;Database=ProposalPilotDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True
ANTHROPIC_API_KEY=sk-ant-xxxxx
SENDGRID_API_KEY=SG.xxxxx
STRIPE_SECRET_KEY=sk_test_xxxxx
STRIPE_PUBLISHABLE_KEY=pk_test_xxxxx
JWT_SECRET=your-256-bit-secret-key-minimum-32-chars
JWT_ISSUER=ProposalPilot
JWT_AUDIENCE=ProposalPilotUsers
REDIS_CONNECTION_STRING=localhost:6379
```

## Important Notes

1. **AI Prompts are in `AI_PROMPTS.md`** - Use these exact prompts for Claude API calls
2. **Database schema is in `TECHNICAL_SPECS.md`** - Follow this for entity creation
3. **Don't skip steps** - The plan is sequential and dependencies matter
4. **Test locally** before moving on - Use Docker for SQL Server and Redis

## Quick Commands

```bash
# Build solution
dotnet build

# Run API
cd src/ProposalPilot.API && dotnet run

# Run Angular
cd ProposalPilot.Web && ng serve

# Run migrations
dotnet ef database update -p src/ProposalPilot.Infrastructure -s src/ProposalPilot.API

# Docker services
docker-compose up -d
```

## Getting Help

If you need clarification on any task:
1. Check `PROJECTPLAN.md` for detailed steps
2. Check `TECHNICAL_SPECS.md` for technical details
3. Check `AI_PROMPTS.md` for AI integration specifics

---

**Start with Day 1 Morning in PROJECTPLAN.md**
