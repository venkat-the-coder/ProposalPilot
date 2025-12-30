using Microsoft.EntityFrameworkCore;
using Serilog;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProposalPilot.Infrastructure.Middleware;
using AspNetCoreRateLimit;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using ProposalPilot.API.Filters;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Starting ProposalPilot API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Configuration
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    builder.Services.Configure<AnthropicSettings>(builder.Configuration.GetSection("AnthropicSettings"));
    builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGridSettings"));
    builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));

    // Database
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("ProposalPilot.Infrastructure")));

    // Redis
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        options.InstanceName = "ProposalPilot_";
    });

    // MediatR
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(ProposalPilot.Application.Features.Users.Queries.GetUserProfile.GetUserProfileQuery).Assembly);
        cfg.RegisterServicesFromAssembly(typeof(ProposalPilot.Infrastructure.Data.ApplicationDbContext).Assembly);
    });

    // Application Services
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ProposalPilot.Application.Interfaces.IAuthService, ProposalPilot.Infrastructure.Services.AuthService>();
    builder.Services.AddScoped<ProposalPilot.Application.Interfaces.ICurrentUserService, ProposalPilot.Infrastructure.Services.CurrentUserService>();
    builder.Services.AddScoped<ProposalPilot.Application.Interfaces.IUserService, ProposalPilot.Infrastructure.Services.UserService>();

    // Cache Service
    builder.Services.AddScoped<ProposalPilot.Application.Interfaces.ICacheService, ProposalPilot.Infrastructure.Services.RedisCacheService>();

    // Brief Analyzer Service
    builder.Services.AddScoped<ProposalPilot.Application.Interfaces.IBriefAnalyzerService, ProposalPilot.Infrastructure.Services.BriefAnalyzerService>();

    // Proposal Generator Service
    builder.Services.AddScoped<ProposalPilot.Application.Interfaces.IProposalGeneratorService, ProposalPilot.Infrastructure.Services.ProposalGeneratorService>();

    // Quality Scorer Service
    builder.Services.AddScoped<ProposalPilot.Infrastructure.Services.IQualityScorerService, ProposalPilot.Infrastructure.Services.QualityScorerService>();

    // Export Services
    builder.Services.AddScoped<ProposalPilot.Infrastructure.Services.IPdfExportService, ProposalPilot.Infrastructure.Services.PdfExportService>();
    builder.Services.AddScoped<ProposalPilot.Infrastructure.Services.IDocxExportService, ProposalPilot.Infrastructure.Services.DocxExportService>();

    // Stripe Service
    builder.Services.AddScoped<ProposalPilot.Infrastructure.Services.IStripeService, ProposalPilot.Infrastructure.Services.StripeService>();

    // Email Service
    builder.Services.AddScoped<ProposalPilot.Application.Interfaces.IEmailService, ProposalPilot.Infrastructure.Services.SendGridEmailService>();

    // Engagement Service
    builder.Services.AddScoped<ProposalPilot.Infrastructure.Services.IEngagementService, ProposalPilot.Infrastructure.Services.EngagementService>();

    // Analytics Service
    builder.Services.AddScoped<ProposalPilot.Infrastructure.Services.IAnalyticsService, ProposalPilot.Infrastructure.Services.AnalyticsService>();

    // Claude API Service with HttpClient and Caching
    builder.Services.AddHttpClient<ProposalPilot.Application.Interfaces.IClaudeApiService, ProposalPilot.Infrastructure.Services.ClaudeApiServiceWithCache>()
        .SetHandlerLifetime(TimeSpan.FromMinutes(5));
    // Note: Caching and retry policies configured in ClaudeApiServiceWithCache

    // Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    // Controllers and API
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Swagger with enhanced documentation
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "ProposalPilot API",
            Version = "v1",
            Description = "AI-powered SaaS platform that helps freelancers and agencies create winning proposals in minutes. Turn client briefs into winning proposals in 5 minutes, not 5 hours.",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "ProposalPilot Support",
                Email = "support@proposalpilot.ai"
            }
        });

        // Add JWT authentication to Swagger
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your JWT token. Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Include XML comments if available
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (System.IO.File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    // Health Checks (skip external checks in Testing environment)
    var isTesting = builder.Environment.EnvironmentName == "Testing";
    if (!isTesting)
    {
        builder.Services.AddHealthChecks()
            .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "database")
            .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis");
    }
    else
    {
        builder.Services.AddHealthChecks();
    }

    // Rate Limiting
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
    builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    builder.Services.AddInMemoryRateLimiting();

    // FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<ProposalPilot.Application.Validators.RegisterRequestValidator>();
    builder.Services.AddFluentValidationAutoValidation();

    // Hangfire for background jobs (skip in Testing environment)
    if (!isTesting)
    {
        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddHangfireServer();
    }

    // Follow-up Service
    builder.Services.AddScoped<ProposalPilot.Infrastructure.Services.IFollowUpService, ProposalPilot.Infrastructure.Services.FollowUpService>();

    var app = builder.Build();

    // Middleware Pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProposalPilot API v1");
            c.RoutePrefix = string.Empty; // Swagger at root
        });
    }

    app.UseSerilogRequestLogging();

    // Security headers (add security headers to all responses)
    app.UseSecurityHeaders();

    // Rate limiting (before other middleware to protect all endpoints)
    app.UseIpRateLimiting();

    app.UseHttpsRedirection();

    app.UseCors("AllowAngularApp");

    app.UseAuthentication();
    app.UseAuthorization();

    // Subscription enforcement middleware (checks proposal limits)
    app.UseSubscriptionEnforcement();

    app.MapControllers();
    app.MapHealthChecks("/health");

    // Hangfire Dashboard and recurring jobs (skip in Testing environment)
    if (app.Environment.EnvironmentName != "Testing")
    {
        app.MapHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = app.Environment.IsDevelopment()
                ? new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() }
                : new[] { new HangfireAuthorizationFilter() }
        });

        RecurringJob.AddOrUpdate<ProposalPilot.Infrastructure.Services.IFollowUpService>(
            "process-automatic-followups",
            service => service.ProcessAutomaticFollowUpsAsync(),
            Cron.Daily(9)); // Run daily at 9 AM UTC

        Log.Information("Hangfire recurring jobs configured");
    }

    // Database migration on startup (only in development)
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
        Log.Information("Database migration completed");
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Partial class for integration testing
public partial class Program { }
