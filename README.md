# ProposalPilot AI

> Turn client briefs into winning proposals in 5 minutes, not 5 hours.

ProposalPilot AI is an AI-powered SaaS platform that helps freelancers and agencies create professional, customized proposals from client briefs using Claude AI. Features include intelligent brief analysis, 3-tier pricing generation, real-time collaboration, client engagement tracking, and subscription management.

## Features

### Core Features
- **AI Brief Analysis**: Upload client briefs and get instant analysis with key requirements, scope, budget insights, and risk assessment
- **Smart Proposal Generation**: Generate professional proposals with 3-tier pricing (Good/Better/Best) tailored to client needs
- **Quality Scoring**: Real-time quality assessment with actionable improvement suggestions
- **Rich Text Editor**: Quill-based editor for customizing proposals with formatting, images, and tables
- **Client Engagement Tracking**: Monitor proposal views, time spent, and section-level analytics
- **Public Sharing**: Generate shareable links for clients with real-time view tracking
- **Export Options**: Download proposals as PDF or DOCX

### Business Features
- **Subscription Management**: Stripe-integrated plans (Free, Pro, Enterprise) with proposal limits
- **Analytics Dashboard**: Track proposal performance, conversion rates, and revenue metrics
- **Email Notifications**: Automated follow-ups and engagement alerts via SendGrid
- **Multi-user Support**: Team collaboration with role-based access

## Tech Stack

### Backend
- **.NET 8** Web API with Clean Architecture
- **SQL Server** for data persistence
- **Redis** for caching and rate limiting
- **Entity Framework Core** for ORM
- **MediatR** for CQRS pattern
- **FluentValidation** for request validation
- **Serilog** for structured logging
- **Hangfire** for background jobs

### Frontend
- **Angular 18** with standalone components
- **NgRx** for state management
- **TailwindCSS** + **PrimeNG** for UI
- **Quill** rich text editor
- **Chart.js** for analytics visualizations
- **Vitest** for testing

### External Services
- **Claude API** (Anthropic) for AI generation
- **Stripe** for subscription payments
- **SendGrid** for email delivery

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/) and npm
- [SQL Server 2022](https://www.microsoft.com/sql-server/sql-server-downloads) (or Azure SQL)
- [Redis 7.x](https://redis.io/download)
- [Git](https://git-scm.com/downloads)

### API Keys Required
- [Anthropic API Key](https://console.anthropic.com/) for Claude AI
- [Stripe Account](https://stripe.com/) for payments
- [SendGrid API Key](https://sendgrid.com/) for emails

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/ProposalPilot.git
cd ProposalPilot
```

### 2. Setup Backend

#### Configure Database

```bash
# Update connection string in appsettings.Development.json
cd backend/src/ProposalPilot.API

# Run migrations
dotnet ef database update -p ../ProposalPilot.Infrastructure
```

#### Configure Environment Variables

Create `appsettings.Development.json` in `backend/src/ProposalPilot.API/`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProposalPilotDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "Secret": "your-256-bit-secret-key-minimum-32-characters-long",
    "Issuer": "ProposalPilot",
    "Audience": "ProposalPilotUsers",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  },
  "AnthropicSettings": {
    "ApiKey": "sk-ant-your-api-key-here",
    "Model": "claude-3-sonnet-20240229"
  },
  "SendGridSettings": {
    "ApiKey": "SG.your-sendgrid-api-key",
    "FromEmail": "noreply@proposalpilot.ai",
    "FromName": "ProposalPilot"
  },
  "StripeSettings": {
    "SecretKey": "sk_test_your-stripe-secret-key",
    "PublishableKey": "pk_test_your-stripe-publishable-key",
    "WebhookSecret": "whsec_your-webhook-secret"
  },
  "CorsSettings": {
    "AllowedOrigins": ["http://localhost:4200"]
  }
}
```

#### Start Redis

```bash
# Windows (using Docker)
docker run -d -p 6379:6379 --name redis redis:7-alpine

# Or use Redis installed locally
redis-server
```

#### Run Backend API

```bash
cd backend/src/ProposalPilot.API
dotnet run
# API will be available at https://localhost:5001
# Swagger UI at https://localhost:5001/swagger
```

### 3. Setup Frontend

```bash
cd frontend/ProposalPilot.Web

# Install dependencies
npm install

# Start development server
npm start
# App will be available at http://localhost:4200
```

## Development

### Running Tests

#### Backend Tests

```bash
cd backend
dotnet test

# Run specific test project
dotnet test tests/ProposalPilot.Tests.Unit
dotnet test tests/ProposalPilot.Tests.Integration
```

**Test Coverage:**
- 7 unit tests (AuthService)
- 8 integration tests (AuthController, Health Checks)

#### Frontend Tests

```bash
cd frontend/ProposalPilot.Web
npm test
```

**Test Coverage:**
- 21 component/service tests (Angular + Vitest)

### Database Migrations

```bash
# Create a new migration
dotnet ef migrations add MigrationName -p backend/src/ProposalPilot.Infrastructure -s backend/src/ProposalPilot.API

# Update database
dotnet ef database update -p backend/src/ProposalPilot.Infrastructure -s backend/src/ProposalPilot.API

# Revert migration
dotnet ef database update PreviousMigrationName -p backend/src/ProposalPilot.Infrastructure -s backend/src/ProposalPilot.API
```

### Project Structure

```
ProposalPilot/
├── backend/
│   ├── src/
│   │   ├── ProposalPilot.API/              # Web API, Controllers, Middleware
│   │   ├── ProposalPilot.Application/      # Business logic, CQRS, MediatR
│   │   ├── ProposalPilot.Domain/           # Entities, Enums, Interfaces
│   │   ├── ProposalPilot.Infrastructure/   # EF Core, External services
│   │   └── ProposalPilot.Shared/           # DTOs, Configuration
│   └── tests/
│       ├── ProposalPilot.Tests.Unit/       # Unit tests
│       └── ProposalPilot.Tests.Integration/ # Integration tests
├── frontend/
│   └── ProposalPilot.Web/                  # Angular 18 app
│       ├── src/app/
│       │   ├── core/                       # Services, Guards, Interceptors
│       │   ├── features/                   # Feature modules
│       │   └── shared/                     # Shared components
│       └── public/                         # Static assets
├── CLAUDE.md                               # Claude Code instructions
├── PROJECTPLAN.md                          # Development roadmap
├── TECHNICAL_SPECS.md                      # Technical specifications
└── README.md                               # This file
```

## API Documentation

API documentation is available via Swagger UI when running in development mode:

**URL:** `https://localhost:5001/swagger`

### Authentication

Most API endpoints require JWT authentication. To authenticate:

1. Register a new account: `POST /api/auth/register`
2. Login: `POST /api/auth/login` - Returns access token
3. Include token in requests: `Authorization: Bearer <your-token>`

### Key Endpoints

#### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/refresh-token` - Refresh access token
- `GET /api/auth/me` - Get current user info

#### Briefs
- `POST /api/briefs` - Upload and analyze client brief
- `GET /api/briefs/{id}` - Get brief details
- `DELETE /api/briefs/{id}` - Delete brief

#### Proposals
- `POST /api/proposals/generate` - Generate proposal from brief
- `GET /api/proposals` - List user's proposals
- `GET /api/proposals/{id}` - Get proposal details
- `PUT /api/proposals/{id}` - Update proposal
- `POST /api/proposals/{id}/quality-score` - Get quality score
- `GET /api/proposals/{id}/export/pdf` - Export as PDF
- `GET /api/proposals/public/{shareToken}` - View public proposal

#### Subscriptions
- `GET /api/subscriptions/plans` - List available plans
- `POST /api/subscriptions/checkout` - Create Stripe checkout session
- `GET /api/subscriptions/current` - Get user's subscription

#### Analytics
- `GET /api/analytics/overview` - Dashboard overview stats
- `GET /api/analytics/proposals/{id}/engagement` - Proposal engagement data

## Environment Variables

### Backend (appsettings.json)

| Variable | Description | Required | Example |
|----------|-------------|----------|---------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string | Yes | `Server=localhost;Database=ProposalPilotDB;...` |
| `ConnectionStrings:Redis` | Redis connection string | Yes | `localhost:6379` |
| `JwtSettings:Secret` | JWT signing secret (min 32 chars) | Yes | `your-secret-key` |
| `JwtSettings:Issuer` | JWT issuer | Yes | `ProposalPilot` |
| `JwtSettings:Audience` | JWT audience | Yes | `ProposalPilotUsers` |
| `AnthropicSettings:ApiKey` | Claude API key | Yes | `sk-ant-xxxxx` |
| `SendGridSettings:ApiKey` | SendGrid API key | Yes | `SG.xxxxx` |
| `StripeSettings:SecretKey` | Stripe secret key | Yes | `sk_test_xxxxx` |
| `StripeSettings:WebhookSecret` | Stripe webhook secret | Yes | `whsec_xxxxx` |

### Frontend (environment.ts)

| Variable | Description | Required | Example |
|----------|-------------|----------|---------|
| `apiUrl` | Backend API URL | Yes | `https://localhost:5001/api` |
| `stripePublishableKey` | Stripe publishable key | Yes | `pk_test_xxxxx` |

## Deployment

### Azure Deployment (Recommended)

#### Prerequisites
- Azure subscription
- Azure CLI installed

#### Steps

1. **Create Azure Resources**

```bash
# Login to Azure
az login

# Create resource group
az group create --name ProposalPilot-RG --location eastus

# Create SQL Database
az sql server create --name proposalpilot-sql --resource-group ProposalPilot-RG --location eastus --admin-user sqladmin --admin-password YourP@ssw0rd123
az sql db create --resource-group ProposalPilot-RG --server proposalpilot-sql --name ProposalPilotDB --service-objective S0

# Create Redis Cache
az redis create --location eastus --name proposalpilot-redis --resource-group ProposalPilot-RG --sku Basic --vm-size c0

# Create App Service Plan
az appservice plan create --name ProposalPilot-Plan --resource-group ProposalPilot-RG --sku B1 --is-linux

# Create Web App for API
az webapp create --resource-group ProposalPilot-RG --plan ProposalPilot-Plan --name proposalpilot-api --runtime "DOTNETCORE:8.0"

# Create Static Web App for Frontend
az staticwebapp create --name proposalpilot-web --resource-group ProposalPilot-RG --location eastus
```

2. **Configure Connection Strings**

```bash
az webapp config connection-string set --resource-group ProposalPilot-RG --name proposalpilot-api --settings DefaultConnection="..." Redis="..." --connection-string-type SQLAzure
```

3. **Deploy Backend**

```bash
cd backend/src/ProposalPilot.API
dotnet publish -c Release -o ./publish
az webapp deployment source config-zip --resource-group ProposalPilot-RG --name proposalpilot-api --src publish.zip
```

4. **Deploy Frontend**

```bash
cd frontend/ProposalPilot.Web
npm run build
# Follow Azure Static Web Apps deployment instructions
```

## Troubleshooting

### Common Issues

#### Database Connection Fails
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database exists: `dotnet ef database update`

#### Redis Connection Fails
- Verify Redis is running: `redis-cli ping` should return `PONG`
- Check Redis connection string in `appsettings.json`

#### JWT Token Invalid
- Verify JWT secret is at least 32 characters
- Check token expiration settings
- Ensure token is included in `Authorization: Bearer <token>` header

#### Stripe Webhook Fails
- Use Stripe CLI for local testing: `stripe listen --forward-to localhost:5001/api/stripe/webhook`
- Verify webhook secret matches in `appsettings.json`

#### Claude API Errors
- Verify API key is valid
- Check API rate limits
- Ensure sufficient Claude API credits

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is proprietary software. All rights reserved.

## Support

For support, email support@proposalpilot.ai or open an issue in the GitHub repository.

## Acknowledgments

- Built with [Claude AI](https://www.anthropic.com/claude) by Anthropic
- UI components from [PrimeNG](https://primeng.org/)
- Payments powered by [Stripe](https://stripe.com/)
- Email delivery by [SendGrid](https://sendgrid.com/)

---

**ProposalPilot** - Powered by AI, Built for Success 🚀
