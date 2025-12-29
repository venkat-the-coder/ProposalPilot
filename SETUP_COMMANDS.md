# ProposalPilot AI - Setup Commands Reference

> All CLI commands needed to set up and run ProposalPilot AI.

---

## 1. Create Solution Structure

```bash
# Create root directory
mkdir ProposalPilot && cd ProposalPilot

# Create solution
dotnet new sln -n ProposalPilot

# Create projects
dotnet new webapi -n ProposalPilot.API -o src/ProposalPilot.API
dotnet new classlib -n ProposalPilot.Application -o src/ProposalPilot.Application
dotnet new classlib -n ProposalPilot.Domain -o src/ProposalPilot.Domain
dotnet new classlib -n ProposalPilot.Infrastructure -o src/ProposalPilot.Infrastructure
dotnet new classlib -n ProposalPilot.Shared -o src/ProposalPilot.Shared

# Add projects to solution
dotnet sln add src/ProposalPilot.API
dotnet sln add src/ProposalPilot.Application
dotnet sln add src/ProposalPilot.Domain
dotnet sln add src/ProposalPilot.Infrastructure
dotnet sln add src/ProposalPilot.Shared

# Set up project references
cd src/ProposalPilot.API
dotnet add reference ../ProposalPilot.Application
dotnet add reference ../ProposalPilot.Infrastructure

cd ../ProposalPilot.Application
dotnet add reference ../ProposalPilot.Domain
dotnet add reference ../ProposalPilot.Shared

cd ../ProposalPilot.Infrastructure
dotnet add reference ../ProposalPilot.Application
dotnet add reference ../ProposalPilot.Domain

cd ../../..
```

---

## 2. Install NuGet Packages

### API Project
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

cd ../..
```

### Application Project
```bash
cd src/ProposalPilot.Application

dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions

cd ../..
```

### Infrastructure Project
```bash
cd src/ProposalPilot.Infrastructure

dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package StackExchange.Redis
dotnet add package SendGrid
dotnet add package Stripe.net
dotnet add package BCrypt.Net-Next
dotnet add package Polly
dotnet add package Polly.Extensions.Http
dotnet add package QuestPDF
dotnet add package DocumentFormat.OpenXml

cd ../..
```

### Shared Project
```bash
cd src/ProposalPilot.Shared

dotnet add package System.Text.Json

cd ../..
```

---

## 3. Create Angular Frontend

```bash
# Create Angular project (run from ProposalPilot root)
ng new ProposalPilot.Web --routing --style=scss --standalone --skip-git

cd ProposalPilot.Web

# Install dependencies
npm install @angular/material @angular/cdk
npm install tailwindcss postcss autoprefixer
npm install @ngrx/store @ngrx/effects @ngrx/store-devtools
npm install ngx-quill quill
npm install chart.js ng2-charts
npm install @auth0/angular-jwt

# Initialize Tailwind
npx tailwindcss init

# Create folder structure
mkdir -p src/app/core/{services,guards,interceptors}
mkdir -p src/app/shared/{components,pipes,directives}
mkdir -p src/app/features/{auth,dashboard,proposals,templates,analytics,settings}
mkdir -p src/app/layouts
mkdir -p src/app/state

cd ..
```

---

## 4. Docker Setup

### Create docker-compose.yml
```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: proposalpilot-sql
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql

  redis:
    image: redis:7-alpine
    container_name: proposalpilot-redis
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data

volumes:
  sqlserver_data:
  redis_data:
```

### Docker Commands
```bash
# Start services
docker-compose up -d

# Stop services
docker-compose down

# View logs
docker-compose logs -f

# Check running containers
docker ps
```

---

## 5. Database Commands

### Entity Framework Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName -p src/ProposalPilot.Infrastructure -s src/ProposalPilot.API

# Update database
dotnet ef database update -p src/ProposalPilot.Infrastructure -s src/ProposalPilot.API

# Remove last migration (if not applied)
dotnet ef migrations remove -p src/ProposalPilot.Infrastructure -s src/ProposalPilot.API

# Generate SQL script
dotnet ef migrations script -p src/ProposalPilot.Infrastructure -s src/ProposalPilot.API -o migrations.sql

# List migrations
dotnet ef migrations list -p src/ProposalPilot.Infrastructure -s src/ProposalPilot.API
```

---

## 6. Run Commands

### Backend
```bash
# Run API (development)
cd src/ProposalPilot.API
dotnet run

# Run with watch (auto-reload)
dotnet watch run

# Run in production mode
dotnet run --configuration Release
```

### Frontend
```bash
# Run Angular dev server
cd ProposalPilot.Web
ng serve

# Run on specific port
ng serve --port 4200

# Run with production config
ng serve --configuration production

# Build for production
ng build --configuration production
```

---

## 7. Build & Test

### Build
```bash
# Build entire solution
dotnet build

# Build for release
dotnet build --configuration Release

# Clean build
dotnet clean && dotnet build
```

### Test
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test src/ProposalPilot.Tests
```

### Angular Tests
```bash
cd ProposalPilot.Web

# Run unit tests
ng test

# Run with coverage
ng test --code-coverage

# Run e2e tests
ng e2e
```

---

## 8. Git Commands

```bash
# Initialize repository
git init

# Add all files
git add .

# Initial commit
git commit -m "Initial commit: Project structure"

# Create and push to remote
git remote add origin https://github.com/USERNAME/ProposalPilot.git
git branch -M main
git push -u origin main
```

---

## 9. Useful Development Commands

### Check installed versions
```bash
dotnet --version
node --version
npm --version
ng version
docker --version
```

### Clear NuGet cache
```bash
dotnet nuget locals all --clear
```

### Restore packages
```bash
dotnet restore
```

### Update packages
```bash
dotnet outdated  # Need dotnet-outdated-tool
dotnet add package PackageName --version X.X.X
```

### Angular CLI
```bash
# Generate component
ng generate component features/proposals/proposal-list

# Generate service
ng generate service core/services/proposal

# Generate guard
ng generate guard core/guards/auth

# Generate module (if needed)
ng generate module features/auth --routing
```

---

## 10. Environment Setup Checklist

```bash
# Verify all tools are installed
dotnet --version    # Should be 8.x.x
node --version      # Should be 20.x.x
npm --version       # Should be 10.x.x
ng version          # Should be 18.x.x
docker --version    # Should be 24.x.x or higher

# Start Docker services
docker-compose up -d

# Verify services are running
docker ps

# Test SQL Server connection
# Use Azure Data Studio or SSMS to connect:
# Server: localhost,1433
# User: sa
# Password: YourStrong@Passw0rd

# Test Redis connection
docker exec -it proposalpilot-redis redis-cli ping
# Should return: PONG
```

---

## Quick Reference Card

| Action | Command |
|--------|---------|
| Build solution | `dotnet build` |
| Run API | `cd src/ProposalPilot.API && dotnet run` |
| Run Angular | `cd ProposalPilot.Web && ng serve` |
| Start Docker | `docker-compose up -d` |
| Add migration | `dotnet ef migrations add Name -p src/ProposalPilot.Infrastructure -s src/ProposalPilot.API` |
| Update DB | `dotnet ef database update -p src/ProposalPilot.Infrastructure -s src/ProposalPilot.API` |
| Run tests | `dotnet test` |
| Build Angular | `ng build --configuration production` |
