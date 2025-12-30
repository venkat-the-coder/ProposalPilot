namespace ProposalPilot.Infrastructure.Features.Proposals.Commands.GenerateProposal;

using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Features.Proposals.Commands.GenerateProposal;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Domain.Enums;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.DTOs.Brief;

public class GenerateProposalCommandHandler : IRequestHandler<GenerateProposalCommand, Guid>
{
    private readonly ApplicationDbContext _context;
    private readonly IProposalGeneratorService _proposalGeneratorService;
    private readonly IClaudeApiService _claudeApiService;
    private readonly ILogger<GenerateProposalCommandHandler> _logger;

    public GenerateProposalCommandHandler(
        ApplicationDbContext context,
        IProposalGeneratorService proposalGeneratorService,
        IClaudeApiService claudeApiService,
        ILogger<GenerateProposalCommandHandler> logger)
    {
        _context = context;
        _proposalGeneratorService = proposalGeneratorService;
        _claudeApiService = claudeApiService;
        _logger = logger;
    }

    public async Task<Guid> Handle(GenerateProposalCommand request, CancellationToken cancellationToken)
    {
        // Get the brief with analysis
        var brief = await _context.Briefs
            .FirstOrDefaultAsync(b => b.Id == request.BriefId && b.UserId == request.UserId, cancellationToken);

        if (brief == null)
        {
            throw new InvalidOperationException($"Brief with ID {request.BriefId} not found");
        }

        if (brief.Status != BriefStatus.Analyzed || string.IsNullOrEmpty(brief.AnalyzedContent))
        {
            throw new InvalidOperationException("Brief must be analyzed before generating a proposal");
        }

        // Get the user
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {request.UserId} not found");
        }

        // Get the client
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == request.ClientId && c.UserId == request.UserId, cancellationToken);

        if (client == null)
        {
            throw new InvalidOperationException($"Client with ID {request.ClientId} not found");
        }

        // Get the template if specified
        ProposalTemplate? template = null;
        if (request.TemplateId.HasValue)
        {
            template = await _context.ProposalTemplates
                .FirstOrDefaultAsync(t => t.Id == request.TemplateId.Value &&
                                         (t.UserId == request.UserId || t.IsPublic),
                                     cancellationToken);

            if (template == null)
            {
                _logger.LogWarning("Template {TemplateId} not found or not accessible for user {UserId}",
                    request.TemplateId, request.UserId);
                // Continue without template instead of failing
            }
            else
            {
                _logger.LogInformation("Using template {TemplateName} for proposal generation", template.Name);
            }
        }

        try
        {
            _logger.LogInformation("Generating proposal for brief {BriefId}", brief.Id);

            // Parse the brief analysis
            var briefAnalysis = JsonSerializer.Deserialize<BriefAnalysisResult>(
                brief.AnalyzedContent,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }
            );

            if (briefAnalysis == null)
            {
                throw new InvalidOperationException("Failed to parse brief analysis");
            }

            // Generate the proposal using AI
            var generationResult = await _proposalGeneratorService.GenerateProposalAsync(
                briefAnalysis,
                $"{user.FirstName} {user.LastName}",
                user.CompanyName,
                100m, // Default hourly rate - this should come from user profile in the future
                request.PreferredTone ?? briefAnalysis.RecommendedApproach.ProposalTone,
                request.ProposalLength,
                template,
                cancellationToken
            );

            // Create the proposal entity
            var proposal = new Proposal
            {
                UserId = request.UserId,
                ClientId = request.ClientId,
                BriefId = request.BriefId,
                Title = generationResult.Title,
                Description = $"Proposal for {brief.Title}",
                Status = ProposalStatus.Draft
            };

            // Store the generated sections as JSON
            var sections = new
            {
                opening_hook = generationResult.Sections.OpeningHook,
                problem_statement = generationResult.Sections.ProblemStatement,
                proposed_solution = generationResult.Sections.ProposedSolution,
                methodology = generationResult.Sections.Methodology,
                timeline = generationResult.Sections.Timeline,
                why_choose_us = generationResult.Sections.WhyChooseUs,
                next_steps = generationResult.Sections.NextSteps
            };

            // Store sections in the GeneratedContent field as JSON
            var sectionsJson = JsonSerializer.Serialize(sections, new JsonSerializerOptions { WriteIndented = true });

            // Use the existing fields in Proposal entity to store the content
            // For now, we'll store the full sections JSON in OriginalBrief field (we can rename this later)
            // and the tiers in the tier JSON fields

            // Store basic tier
            if (generationResult.Sections.Investment.Tiers.Count > 0)
            {
                proposal.BasicTierJson = JsonSerializer.Serialize(generationResult.Sections.Investment.Tiers[0]);
            }

            // Store standard/recommended tier
            if (generationResult.Sections.Investment.Tiers.Count > 1)
            {
                proposal.StandardTierJson = JsonSerializer.Serialize(generationResult.Sections.Investment.Tiers[1]);
            }

            // Store premium tier
            if (generationResult.Sections.Investment.Tiers.Count > 2)
            {
                proposal.PremiumTierJson = JsonSerializer.Serialize(generationResult.Sections.Investment.Tiers[2]);
            }

            // Store the brief analysis in BriefAnalysis field
            proposal.BriefAnalysis = brief.AnalyzedContent;

            // Store the generated sections
            // We'll use the existing OriginalBrief field for now (can be refactored later)
            proposal.OriginalBrief = brief.RawContent;

            // Store a JSON representation of all sections and metadata in a structured way
            var fullContent = new
            {
                sections = sections,
                investment = generationResult.Sections.Investment,
                metadata = generationResult.Metadata
            };

            // Store this in DeliverablesJson (we'll use this for the full generated content)
            proposal.DeliverablesJson = JsonSerializer.Serialize(fullContent, new JsonSerializerOptions { WriteIndented = true });

            // Add the proposal to database
            _context.Proposals.Add(proposal);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully generated proposal {ProposalId} for brief {BriefId}",
                proposal.Id, brief.Id);

            return proposal.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating proposal for brief {BriefId}", brief.Id);
            throw;
        }
    }
}
