# ProposalPilot - Azure Deployment Guide

Complete step-by-step guide to deploy ProposalPilot to Azure with cost-effective resources.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Cost Overview](#cost-overview)
3. [Azure Resources Setup](#azure-resources-setup)
4. [Database Setup & Migration](#database-setup--migration)
5. [Backend API Deployment](#backend-api-deployment)
6. [Frontend Deployment](#frontend-deployment)
7. [CI/CD Pipeline Setup](#cicd-pipeline-setup)
8. [DNS & SSL Configuration](#dns--ssl-configuration)
9. [Monitoring & Logging](#monitoring--logging)
10. [Post-Deployment Checklist](#post-deployment-checklist)
11. [Cost Optimization](#cost-optimization)

---

## Prerequisites

### Required Tools

```bash
# Install Azure CLI
# Windows (PowerShell as Administrator)
winget install -e --id Microsoft.AzureCLI

# Verify installation
az --version

# Install .NET 8 SDK
winget install Microsoft.DotNet.SDK.8

# Install Node.js 20 LTS
winget install OpenJS.NodeJS.LTS

# Install Git
winget install Git.Git
```

### Azure Account Setup

1. **Create Azure Account**
   - Visit https://azure.microsoft.com/free/
   - Get $200 free credit for 30 days
   - Free tier services for 12 months

2. **Login to Azure CLI**
   ```bash
   az login
   # Follow browser authentication

   # Set subscription (if you have multiple)
   az account list --output table
   az account set --subscription "Your-Subscription-Name"
   ```

### External Services Setup

1. **Anthropic API Key** (Required)
   - Sign up at https://console.anthropic.com/
   - Go to API Keys section
   - Create new API key
   - Cost: ~$0.003 per 1K tokens (Claude 3 Sonnet)

2. **Stripe Account** (Required)
   - Sign up at https://stripe.com/
   - Get test API keys from Dashboard > Developers > API keys
   - Free for testing, 2.9% + $0.30 per transaction in production

3. **SendGrid API Key** (Required)
   - Sign up at https://sendgrid.com/
   - Free tier: 100 emails/day
   - Upgrade to Essentials: $19.95/month for 50K emails

---

## Cost Overview

### Monthly Cost Estimate (Production-Ready)

| Resource | Tier | Monthly Cost | Purpose |
|----------|------|--------------|---------|
| App Service Plan | B1 (Basic) | ~$13 | API hosting |
| SQL Database | Basic (5 DTU) | ~$5 | Data storage |
| Azure Cache for Redis | Basic C0 (250MB) | ~$16 | Caching |
| Static Web App | Free | $0 | Frontend hosting |
| Application Insights | Free (5GB) | $0 | Monitoring |
| **Total** | | **~$34/month** | Base infrastructure |

**Additional Costs:**
- Anthropic API: ~$10-50/month (depends on usage)
- SendGrid: Free tier (100 emails/day) or $19.95/month
- Stripe: Pay-per-transaction (2.9% + $0.30)

**Total Estimated Monthly Cost: $44-104/month**

### Development Environment (Lower Cost)

| Resource | Tier | Monthly Cost |
|----------|------|--------------|
| App Service Plan | F1 (Free) | $0 |
| SQL Database | Free tier | $0 |
| Azure Cache for Redis | Basic C0 | ~$16 |
| Static Web App | Free | $0 |
| **Total** | | **~$16/month** |

---

## Azure Resources Setup

### Step 1: Create Resource Group

```bash
# Set variables
RESOURCE_GROUP="ProposalPilot-RG"
LOCATION="eastus"  # Change to your preferred region

# Create resource group
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Verify creation
az group show --name $RESOURCE_GROUP --output table
```

### Step 2: Create SQL Database (Cost-Effective)

```bash
# Variables
SQL_SERVER_NAME="proposalpilot-sql-${RANDOM}"  # Must be globally unique
SQL_ADMIN_USER="sqladmin"
SQL_ADMIN_PASSWORD="YourStrong@Password123"  # Change this!
SQL_DATABASE_NAME="ProposalPilotDB"

# Create SQL Server
az sql server create \
  --name $SQL_SERVER_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $SQL_ADMIN_USER \
  --admin-password $SQL_ADMIN_PASSWORD

# Configure firewall (allow Azure services)
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Allow your IP for management
MY_IP=$(curl -s ifconfig.me)
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name AllowMyIP \
  --start-ip-address $MY_IP \
  --end-ip-address $MY_IP

# Create database (Basic tier - most cost-effective)
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name $SQL_DATABASE_NAME \
  --service-objective Basic \
  --backup-storage-redundancy Local

# Get connection string
az sql db show-connection-string \
  --client ado.net \
  --name $SQL_DATABASE_NAME \
  --server $SQL_SERVER_NAME

# Save this connection string - you'll need it later
```

**Cost Optimization:**
- Basic tier: ~$5/month
- 2GB storage included
- Good for 100-500 users
- Auto-pause not available (always on)

### Step 3: Create Azure Cache for Redis

```bash
# Variables
REDIS_NAME="proposalpilot-redis-${RANDOM}"

# Create Redis Cache (Basic C0 - most cost-effective)
az redis create \
  --location $LOCATION \
  --name $REDIS_NAME \
  --resource-group $RESOURCE_GROUP \
  --sku Basic \
  --vm-size c0 \
  --enable-non-ssl-port false

# This takes 10-15 minutes to provision
# Check status
az redis show \
  --name $REDIS_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "provisioningState" \
  --output tsv

# Get connection string (wait until provisioning is complete)
az redis list-keys \
  --name $REDIS_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "primaryKey" \
  --output tsv

# Full connection string format:
# {REDIS_NAME}.redis.cache.windows.net:6380,password={PRIMARY_KEY},ssl=True,abortConnect=False
```

**Cost Optimization:**
- Basic C0: ~$16/month
- 250MB cache
- Good for 10,000 connections
- No high availability (single instance)

### Step 4: Create App Service Plan

```bash
# Variables
APP_SERVICE_PLAN="ProposalPilot-Plan"

# Create App Service Plan (B1 - Basic tier)
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku B1 \
  --is-linux

# Verify creation
az appservice plan show \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --output table
```

**Plan Options:**
- **F1 (Free)**: $0/month - For testing only, 60 min/day limit
- **B1 (Basic)**: ~$13/month - Recommended for production
- **S1 (Standard)**: ~$70/month - Better performance, auto-scaling

### Step 5: Create Web App for Backend API

```bash
# Variables
API_APP_NAME="proposalpilot-api-${RANDOM}"

# Create Web App
az webapp create \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --name $API_APP_NAME \
  --runtime "DOTNETCORE:8.0"

# Enable HTTPS only
az webapp update \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --https-only true

# Configure always on (keeps app warm)
az webapp config set \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --always-on true

# Get URL
echo "API URL: https://${API_APP_NAME}.azurewebsites.net"
```

### Step 6: Create Static Web App for Frontend

```bash
# Variables
STATIC_APP_NAME="proposalpilot-web"

# Create Static Web App (Free tier)
az staticwebapp create \
  --name $STATIC_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Free

# Get deployment token (needed for GitHub Actions)
az staticwebapp secrets list \
  --name $STATIC_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "properties.apiKey" \
  --output tsv

# Save this token for CI/CD setup
```

**Static Web App Benefits:**
- **Free tier**: Perfect for frontend
- Global CDN included
- Automatic SSL certificate
- Unlimited bandwidth (within fair use)

### Step 7: Create Application Insights (Monitoring)

```bash
# Variables
APP_INSIGHTS_NAME="proposalpilot-insights"

# Create Application Insights
az monitor app-insights component create \
  --app $APP_INSIGHTS_NAME \
  --location $LOCATION \
  --resource-group $RESOURCE_GROUP \
  --application-type web

# Get instrumentation key
az monitor app-insights component show \
  --app $APP_INSIGHTS_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "instrumentationKey" \
  --output tsv
```

---

## Database Setup & Migration

### Step 1: Update Connection String Locally

```bash
cd backend/src/ProposalPilot.API

# Create appsettings.Production.json
```

Create file `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:{SQL_SERVER_NAME}.database.windows.net,1433;Initial Catalog=ProposalPilotDB;Persist Security Info=False;User ID={SQL_ADMIN_USER};Password={SQL_ADMIN_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    "Redis": "{REDIS_NAME}.redis.cache.windows.net:6380,password={REDIS_PRIMARY_KEY},ssl=True,abortConnect=False"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Step 2: Run Database Migrations

```bash
# Install EF Core tools if not already installed
dotnet tool install --global dotnet-ef

# Navigate to API project
cd backend/src/ProposalPilot.API

# Generate SQL migration script
dotnet ef migrations script \
  --project ../ProposalPilot.Infrastructure \
  --startup-project . \
  --idempotent \
  --output migration.sql

# Apply migrations to Azure SQL
# Option 1: Using Azure Data Studio or SSMS (recommended)
# - Connect to Azure SQL Server
# - Run migration.sql script

# Option 2: Using sqlcmd
sqlcmd -S $SQL_SERVER_NAME.database.windows.net \
  -d $SQL_DATABASE_NAME \
  -U $SQL_ADMIN_USER \
  -P $SQL_ADMIN_PASSWORD \
  -i migration.sql

# Verify tables created
# Connect to database and run:
# SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'
```

---

## Backend API Deployment

### Step 1: Configure Application Settings

```bash
# Set connection strings
az webapp config connection-string set \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:${SQL_SERVER_NAME}.database.windows.net,1433;Initial Catalog=${SQL_DATABASE_NAME};User ID=${SQL_ADMIN_USER};Password=${SQL_ADMIN_PASSWORD};Encrypt=True;"

az webapp config connection-string set \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --connection-string-type Custom \
  --settings Redis="${REDIS_NAME}.redis.cache.windows.net:6380,password={REDIS_KEY},ssl=True,abortConnect=False"

# Set application settings (environment variables)
az webapp config appsettings set \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    JwtSettings__Secret="your-production-jwt-secret-min-32-chars-long-change-this" \
    JwtSettings__Issuer="ProposalPilot" \
    JwtSettings__Audience="ProposalPilotUsers" \
    JwtSettings__ExpirationInMinutes="60" \
    JwtSettings__RefreshTokenExpirationInDays="7" \
    AnthropicSettings__ApiKey="sk-ant-your-anthropic-api-key" \
    AnthropicSettings__Model="claude-3-sonnet-20240229" \
    SendGridSettings__ApiKey="SG.your-sendgrid-api-key" \
    SendGridSettings__FromEmail="noreply@proposalpilot.ai" \
    SendGridSettings__FromName="ProposalPilot" \
    StripeSettings__SecretKey="sk_live_your-stripe-secret-key" \
    StripeSettings__PublishableKey="pk_live_your-stripe-publishable-key" \
    StripeSettings__WebhookSecret="whsec_your-webhook-secret" \
    ASPNETCORE_ENVIRONMENT="Production"
```

**Important:** Replace all placeholder values with your actual API keys!

### Step 2: Build and Publish API

```bash
# Navigate to API project
cd backend/src/ProposalPilot.API

# Clean previous builds
dotnet clean
dotnet restore

# Build in Release mode
dotnet build --configuration Release

# Publish to folder
dotnet publish --configuration Release --output ./publish

# Create deployment package
cd publish
Compress-Archive -Path * -DestinationPath ../deploy.zip -Force
cd ..
```

### Step 3: Deploy to Azure

```bash
# Deploy using Azure CLI
az webapp deployment source config-zip \
  --resource-group $RESOURCE_GROUP \
  --name $API_APP_NAME \
  --src deploy.zip

# Check deployment status
az webapp log tail \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP
```

### Step 4: Verify API Deployment

```bash
# Get API URL
API_URL=$(az webapp show \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "defaultHostName" \
  --output tsv)

echo "API URL: https://${API_URL}"
echo "Swagger URL: https://${API_URL}/swagger"
echo "Health Check: https://${API_URL}/health"

# Test health endpoint
curl https://${API_URL}/health
```

---

## Frontend Deployment

### Step 1: Configure Frontend Environment

```bash
cd frontend/ProposalPilot.Web
```

Create/update `src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://{API_APP_NAME}.azurewebsites.net/api',
  stripePublishableKey: 'pk_live_your-stripe-publishable-key'
};
```

### Step 2: Build Frontend

```bash
# Install dependencies
npm install

# Build for production
npm run build

# Output will be in dist/proposal-pilot.web/browser/
```

### Step 3: Deploy to Static Web App

**Option A: Manual Deployment (Quick Test)**

```bash
# Install Static Web Apps CLI
npm install -g @azure/static-web-apps-cli

# Get deployment token (from earlier step)
DEPLOYMENT_TOKEN=$(az staticwebapp secrets list \
  --name $STATIC_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "properties.apiKey" \
  --output tsv)

# Deploy
cd dist/proposal-pilot.web/browser
az staticwebapp deploy \
  --app-name $STATIC_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --no-use-keyring
```

**Option B: GitHub Actions (Recommended - see CI/CD section below)**

### Step 4: Configure CORS on API

```bash
# Get Static Web App URL
FRONTEND_URL=$(az staticwebapp show \
  --name $STATIC_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "defaultHostname" \
  --output tsv)

echo "Frontend URL: https://${FRONTEND_URL}"

# Update API CORS settings
az webapp cors add \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --allowed-origins "https://${FRONTEND_URL}"

# Verify CORS settings
az webapp cors show \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP
```

---

## CI/CD Pipeline Setup

### GitHub Actions Setup

### Step 1: Create GitHub Repository

```bash
# Initialize git if not already done
cd /path/to/ProposalPilot
git init
git add .
git commit -m "Initial commit"

# Create GitHub repo (using GitHub CLI)
gh repo create ProposalPilot --private --source=. --remote=origin

# Push to GitHub
git push -u origin master
```

### Step 2: Add Azure Secrets to GitHub

```bash
# Get publish profile for API
az webapp deployment list-publishing-profiles \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --xml > api-publish-profile.xml

# Get Static Web App deployment token (already have from earlier)
echo $DEPLOYMENT_TOKEN

# Add secrets to GitHub (using GitHub CLI)
gh secret set AZURE_WEBAPP_PUBLISH_PROFILE < api-publish-profile.xml
gh secret set AZURE_STATIC_WEB_APPS_API_TOKEN --body "$DEPLOYMENT_TOKEN"
```

### Step 3: Create Backend CI/CD Workflow

Create `.github/workflows/backend-deploy.yml`:

```yaml
name: Deploy Backend API

on:
  push:
    branches: [ master, main ]
    paths:
      - 'backend/**'
  workflow_dispatch:

env:
  DOTNET_VERSION: '8.0.x'
  AZURE_WEBAPP_NAME: 'proposalpilot-api-xxxxx'  # Replace with your app name
  AZURE_WEBAPP_PACKAGE_PATH: './backend/src/ProposalPilot.API'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}

    - name: Restore dependencies
      run: dotnet restore
      working-directory: ./backend

    - name: Build
      run: dotnet build --configuration Release --no-restore
      working-directory: ./backend

    - name: Run tests
      run: dotnet test --no-build --verbosity normal
      working-directory: ./backend

    - name: Publish
      run: dotnet publish -c Release -o ${{env.DOTNET_ROOT}}/api
      working-directory: ${{ env.AZURE_WEBAPP_PACKAGE_PATH }}

    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME }}
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ${{env.DOTNET_ROOT}}/api
```

### Step 4: Create Frontend CI/CD Workflow

Create `.github/workflows/frontend-deploy.yml`:

```yaml
name: Deploy Frontend

on:
  push:
    branches: [ master, main ]
    paths:
      - 'frontend/**'
  workflow_dispatch:

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4
      with:
        submodules: true

    - name: Setup Node.js
      uses: actions/setup-node@v4
      with:
        node-version: '20'
        cache: 'npm'
        cache-dependency-path: frontend/ProposalPilot.Web/package-lock.json

    - name: Install dependencies
      run: npm ci
      working-directory: ./frontend/ProposalPilot.Web

    - name: Run tests
      run: npm test
      working-directory: ./frontend/ProposalPilot.Web

    - name: Build
      run: npm run build
      working-directory: ./frontend/ProposalPilot.Web
      env:
        NODE_ENV: production

    - name: Deploy to Azure Static Web Apps
      uses: Azure/static-web-apps-deploy@v1
      with:
        azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
        repo_token: ${{ secrets.GITHUB_TOKEN }}
        action: "upload"
        app_location: "/frontend/ProposalPilot.Web"
        api_location: ""
        output_location: "dist/proposal-pilot.web/browser"
```

### Step 5: Commit and Push Workflows

```bash
git add .github/workflows/
git commit -m "Add CI/CD workflows"
git push

# Verify workflows are running
gh run list
```

---

## DNS & SSL Configuration

### Step 1: Configure Custom Domain (Optional)

```bash
# Purchase domain from Azure or external provider (e.g., Namecheap, GoDaddy)

# For API (App Service)
# 1. Add custom domain
az webapp config hostname add \
  --webapp-name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --hostname api.yourdomain.com

# 2. Bind SSL certificate (Free App Service Managed Certificate)
az webapp config ssl create \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --hostname api.yourdomain.com

az webapp config ssl bind \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --certificate-thumbprint {THUMBPRINT} \
  --ssl-type SNI

# For Frontend (Static Web App)
# 1. Add custom domain
az staticwebapp hostname set \
  --name $STATIC_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --hostname www.yourdomain.com

# SSL is automatically configured by Azure Static Web Apps
```

### Step 2: Update DNS Records

Add these records to your domain registrar:

**For API (api.yourdomain.com):**
```
Type: CNAME
Name: api
Value: {API_APP_NAME}.azurewebsites.net
TTL: 3600
```

**For Frontend (www.yourdomain.com):**
```
Type: CNAME
Name: www
Value: {STATIC_APP_NAME}.azurestaticapps.net
TTL: 3600
```

---

## Monitoring & Logging

### Step 1: Enable Application Insights

```bash
# Get instrumentation key (from earlier step)
INSTRUMENTATION_KEY=$(az monitor app-insights component show \
  --app $APP_INSIGHTS_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "instrumentationKey" \
  --output tsv)

# Configure API to use Application Insights
az webapp config appsettings set \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=${INSTRUMENTATION_KEY}" \
    ApplicationInsightsAgent_EXTENSION_VERSION="~3"
```

### Step 2: Configure Alerts

```bash
# Create action group for email notifications
az monitor action-group create \
  --name "ProposalPilot-Alerts" \
  --resource-group $RESOURCE_GROUP \
  --short-name "PP-Alert" \
  --email-receiver \
    name="Admin" \
    email-address="admin@yourdomain.com"

# Create alert for API errors
az monitor metrics alert create \
  --name "API-Errors" \
  --resource-group $RESOURCE_GROUP \
  --scopes $(az webapp show --name $API_APP_NAME --resource-group $RESOURCE_GROUP --query id --output tsv) \
  --condition "count Http5xx > 10" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --action $(az monitor action-group show --name "ProposalPilot-Alerts" --resource-group $RESOURCE_GROUP --query id --output tsv)

# Create alert for high CPU
az monitor metrics alert create \
  --name "High-CPU" \
  --resource-group $RESOURCE_GROUP \
  --scopes $(az webapp show --name $API_APP_NAME --resource-group $RESOURCE_GROUP --query id --output tsv) \
  --condition "avg CpuPercentage > 80" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --action $(az monitor action-group show --name "ProposalPilot-Alerts" --resource-group $RESOURCE_GROUP --query id --output tsv)
```

### Step 3: View Logs

```bash
# Stream live logs
az webapp log tail \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP

# Download logs
az webapp log download \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --log-file app-logs.zip

# View in Azure Portal
echo "Application Insights: https://portal.azure.com/#@/resource$(az monitor app-insights component show --app $APP_INSIGHTS_NAME --resource-group $RESOURCE_GROUP --query id --output tsv)"
```

---

## Post-Deployment Checklist

### Immediate After Deployment

- [ ] **Verify API Health**
  ```bash
  curl https://{API_APP_NAME}.azurewebsites.net/health
  ```
  Expected: `Healthy` response

- [ ] **Test Authentication**
  - Register a new user
  - Login
  - Verify JWT token received

- [ ] **Test Frontend**
  - Visit `https://{STATIC_APP_NAME}.azurestaticapps.net`
  - Navigate through pages
  - Check console for errors

- [ ] **Verify Database Connection**
  - Create a proposal
  - Verify data is saved
  - Check SQL Database in Azure Portal

- [ ] **Test Redis Cache**
  - Monitor Redis metrics in Azure Portal
  - Verify cache hit/miss ratios

### External Services Configuration

- [ ] **Stripe Webhook**
  ```bash
  # Configure webhook endpoint in Stripe Dashboard
  # URL: https://{API_APP_NAME}.azurewebsites.net/api/stripe/webhook
  # Events to listen: checkout.session.completed, customer.subscription.updated, etc.
  ```

- [ ] **SendGrid Domain Authentication**
  - Add domain to SendGrid
  - Configure DNS records
  - Verify domain

- [ ] **Anthropic API**
  - Verify API key is working
  - Check usage limits
  - Set up billing alerts

### Security Checklist

- [ ] **Change Default Passwords**
  - SQL admin password
  - JWT secret key
  - All API keys rotated

- [ ] **Enable HTTPS Only**
  ```bash
  az webapp update --name $API_APP_NAME --resource-group $RESOURCE_GROUP --https-only true
  ```

- [ ] **Configure CORS Properly**
  - Remove wildcard origins
  - Add only specific frontend domains

- [ ] **Enable Managed Identity** (Optional but recommended)
  ```bash
  az webapp identity assign --name $API_APP_NAME --resource-group $RESOURCE_GROUP
  ```

- [ ] **Review Firewall Rules**
  - SQL Server firewall
  - Redis firewall
  - App Service access restrictions

### Performance Optimization

- [ ] **Enable CDN** (Optional)
  - For static assets
  - Reduce latency globally

- [ ] **Configure Auto-Scaling** (If using Standard tier)
  ```bash
  az monitor autoscale create \
    --resource-group $RESOURCE_GROUP \
    --resource $API_APP_NAME \
    --resource-type Microsoft.Web/serverfarms \
    --name autoscale-rules \
    --min-count 1 \
    --max-count 3 \
    --count 1
  ```

- [ ] **Enable Compression**
  ```bash
  az webapp config set --name $API_APP_NAME --resource-group $RESOURCE_GROUP --use-32bit-worker-process false
  ```

### Monitoring Setup

- [ ] **Configure Alerts**
  - Error rate alerts
  - CPU/Memory alerts
  - SQL DTU alerts
  - Redis memory alerts

- [ ] **Set Up Dashboards**
  - Application Insights dashboard
  - Azure Monitor workbooks

- [ ] **Enable Backup** (SQL Database)
  ```bash
  # Automatic backups enabled by default
  # Verify retention policy
  az sql db ltr-policy show \
    --database $SQL_DATABASE_NAME \
    --server $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP
  ```

---

## Cost Optimization

### Immediate Optimizations

1. **Use Reserved Instances** (Save up to 72%)
   - Commit to 1 or 3 years
   - Significant savings on App Service Plans

2. **Auto-Shutdown Dev Resources**
   ```bash
   # Create automation account to stop/start resources on schedule
   # Shut down dev environment nights and weekends
   ```

3. **Right-Size Resources**
   - Monitor actual usage for 30 days
   - Downgrade if consistently under 50% utilization

4. **Enable Azure Advisor Recommendations**
   ```bash
   az advisor recommendation list --output table
   ```

### Ongoing Optimizations

1. **Monitor Costs Daily**
   ```bash
   # Set budget alerts
   az consumption budget create \
     --budget-name "ProposalPilot-Budget" \
     --amount 100 \
     --time-grain Monthly \
     --start-date 2025-01-01 \
     --end-date 2025-12-31 \
     --resource-group $RESOURCE_GROUP
   ```

2. **Review Unused Resources**
   - Delete orphaned disks
   - Remove old backups
   - Clean up test resources

3. **Optimize Database**
   - Regular index maintenance
   - Archive old data
   - Consider elastic pools for multiple databases

4. **Cache Aggressively**
   - Use Redis for frequently accessed data
   - Implement client-side caching
   - Use CDN for static content

### Cost Breakdown Tools

```bash
# View current month costs
az consumption usage list \
  --start-date $(date -d "first day of this month" +%Y-%m-%d) \
  --end-date $(date +%Y-%m-%d) \
  --query "[?contains(instanceName,'proposalpilot')].{Name:instanceName,Cost:pretaxCost}" \
  --output table

# Export cost data
az costmanagement export create \
  --name "monthly-costs" \
  --type Usage \
  --scope "/subscriptions/{subscription-id}/resourceGroups/$RESOURCE_GROUP" \
  --storage-account-id "{storage-account-id}" \
  --storage-container "cost-exports" \
  --recurrence Daily \
  --recurrence-period from="2025-01-01T00:00:00Z" to="2025-12-31T23:59:59Z"
```

---

## Troubleshooting Deployment Issues

### Common Issues

**Issue: Database Migration Fails**
```bash
# Check SQL Server firewall
az sql server firewall-rule list --server $SQL_SERVER_NAME --resource-group $RESOURCE_GROUP

# Verify connection string
az webapp config connection-string list --name $API_APP_NAME --resource-group $RESOURCE_GROUP

# Manual migration
dotnet ef database update --connection "YourConnectionString"
```

**Issue: API Returns 500 Error**
```bash
# Check logs
az webapp log tail --name $API_APP_NAME --resource-group $RESOURCE_GROUP

# Enable detailed errors
az webapp config set --name $API_APP_NAME --resource-group $RESOURCE_GROUP --detailed-error-logging true
```

**Issue: Redis Connection Timeout**
```bash
# Verify Redis is running
az redis show --name $REDIS_NAME --resource-group $RESOURCE_GROUP --query "provisioningState"

# Check connection string format
# Should be: {name}.redis.cache.windows.net:6380,password={key},ssl=True
```

**Issue: Static Web App Not Updating**
```bash
# Check deployment status
az staticwebapp show --name $STATIC_APP_NAME --resource-group $RESOURCE_GROUP --query "stagingEnvironmentPolicy"

# Redeploy manually
cd frontend/ProposalPilot.Web/dist/proposal-pilot.web/browser
az staticwebapp deploy --name $STATIC_APP_NAME --resource-group $RESOURCE_GROUP
```

---

## Next Steps After Deployment

1. **Load Testing**
   - Use Azure Load Testing service
   - Test with 100-1000 concurrent users
   - Identify bottlenecks

2. **Backup Strategy**
   - Configure SQL Database backup retention
   - Export critical configuration
   - Document disaster recovery plan

3. **Scaling Plan**
   - Define scaling triggers
   - Plan for 10x growth
   - Budget for increased usage

4. **Support Setup**
   - Configure support email monitoring
   - Set up ticketing system
   - Create runbooks for common issues

---

## Useful Commands Reference

```bash
# Quick health check of all resources
az resource list --resource-group $RESOURCE_GROUP --output table

# View all costs
az consumption usage list --output table

# Restart API
az webapp restart --name $API_APP_NAME --resource-group $RESOURCE_GROUP

# Scale API
az appservice plan update --name $APP_SERVICE_PLAN --resource-group $RESOURCE_GROUP --sku B2

# Backup database
az sql db export \
  --server $SQL_SERVER_NAME \
  --name $SQL_DATABASE_NAME \
  --admin-user $SQL_ADMIN_USER \
  --admin-password $SQL_ADMIN_PASSWORD \
  --storage-key-type StorageAccessKey \
  --storage-key {storage-key} \
  --storage-uri "https://{storage-account}.blob.core.windows.net/backups/backup.bacpac"

# Delete all resources (DANGER!)
az group delete --name $RESOURCE_GROUP --yes --no-wait
```

---

**ProposalPilot is now deployed to Azure! 🚀**

For issues or questions, refer to the troubleshooting section or check Azure Portal logs.
