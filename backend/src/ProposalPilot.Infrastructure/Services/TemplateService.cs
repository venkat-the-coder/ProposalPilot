using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.DTOs.Template;

namespace ProposalPilot.Infrastructure.Services;

/// <summary>
/// Service for managing proposal templates with caching and security
/// </summary>
public class TemplateService : ITemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<TemplateService> _logger;
    private const string SystemTemplatesCacheKey = "system_templates";
    private const int CacheDurationMinutes = 60;

    public TemplateService(
        ApplicationDbContext context,
        IDistributedCache cache,
        ILogger<TemplateService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<TemplateListItemDto>> GetTemplatesAsync(
        Guid userId,
        string? category = null,
        string? searchTerm = null,
        bool includePublic = true,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            var query = _context.Set<ProposalTemplate>()
                .Where(t => !t.IsDeleted && (
                    t.IsSystemTemplate ||  // System templates
                    t.UserId == userId ||   // User's own templates
                    (includePublic && t.IsPublic) // Public templates
                ));

            // Filter by category
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(t => t.Category == category);
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t =>
                    t.Name.Contains(searchTerm) ||
                    (t.Description != null && t.Description.Contains(searchTerm)) ||
                    (t.Tags != null && t.Tags.Contains(searchTerm))
                );
            }

            var templates = await query
                .OrderByDescending(t => t.IsSystemTemplate)
                .ThenByDescending(t => t.UsageCount)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(t => t.User)
                .Select(t => new TemplateListItemDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Category = t.Category,
                    Tags = ParseTags(t.Tags),
                    IsSystemTemplate = t.IsSystemTemplate,
                    IsPublic = t.IsPublic,
                    UserName = t.User != null ? $"{t.User.FirstName} {t.User.LastName}" : null,
                    UsageCount = t.UsageCount,
                    WinRate = t.WinRate,
                    ThumbnailUrl = t.ThumbnailUrl,
                    EstimatedTimeMinutes = t.EstimatedTimeMinutes,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} templates for user {UserId}", templates.Count, userId);
            return templates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving templates for user {UserId}", userId);
            throw;
        }
    }

    public async Task<TemplateDto?> GetTemplateByIdAsync(Guid templateId, Guid userId)
    {
        try
        {
            var template = await _context.Set<ProposalTemplate>()
                .Where(t => !t.IsDeleted &&
                    t.Id == templateId &&
                    (t.IsSystemTemplate || t.UserId == userId || t.IsPublic))
                .Include(t => t.User)
                .FirstOrDefaultAsync();

            if (template == null)
            {
                _logger.LogWarning("Template {TemplateId} not found or not accessible to user {UserId}", templateId, userId);
                return null;
            }

            return MapToTemplateDto(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template {TemplateId}", templateId);
            throw;
        }
    }

    public async Task<List<TemplateListItemDto>> GetSystemTemplatesAsync()
    {
        try
        {
            // Try cache first
            var cachedData = await _cache.GetStringAsync(SystemTemplatesCacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Returning system templates from cache");
                return JsonSerializer.Deserialize<List<TemplateListItemDto>>(cachedData) ?? new List<TemplateListItemDto>();
            }

            // Cache miss - fetch from database
            var templates = await _context.Set<ProposalTemplate>()
                .Where(t => !t.IsDeleted && t.IsSystemTemplate)
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .Select(t => new TemplateListItemDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Category = t.Category,
                    Tags = ParseTags(t.Tags),
                    IsSystemTemplate = true,
                    IsPublic = true,
                    UsageCount = t.UsageCount,
                    WinRate = t.WinRate,
                    ThumbnailUrl = t.ThumbnailUrl,
                    EstimatedTimeMinutes = t.EstimatedTimeMinutes,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            // Cache the results
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheDurationMinutes)
            };
            await _cache.SetStringAsync(SystemTemplatesCacheKey, JsonSerializer.Serialize(templates), cacheOptions);

            _logger.LogInformation("Retrieved and cached {Count} system templates", templates.Count);
            return templates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system templates");
            throw;
        }
    }

    public async Task<List<TemplateListItemDto>> GetUserTemplatesAsync(Guid userId)
    {
        try
        {
            var templates = await _context.Set<ProposalTemplate>()
                .Where(t => !t.IsDeleted && t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TemplateListItemDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Category = t.Category,
                    Tags = ParseTags(t.Tags),
                    IsSystemTemplate = false,
                    IsPublic = t.IsPublic,
                    UsageCount = t.UsageCount,
                    WinRate = t.WinRate,
                    ThumbnailUrl = t.ThumbnailUrl,
                    EstimatedTimeMinutes = t.EstimatedTimeMinutes,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return templates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving templates for user {UserId}", userId);
            throw;
        }
    }

    public async Task<TemplateDto> CreateTemplateAsync(CreateTemplateRequest request, Guid userId)
    {
        try
        {
            var template = new ProposalTemplate
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Tags = SerializeTags(request.Tags),
                Content = SerializeContent(request.Content),
                DefaultPricing = SerializePricing(request.DefaultPricing),
                IsSystemTemplate = false, // Users can't create system templates
                IsPublic = request.IsPublic,
                UserId = userId,
                ThumbnailUrl = request.ThumbnailUrl,
                EstimatedTimeMinutes = request.EstimatedTimeMinutes
            };

            _context.Set<ProposalTemplate>().Add(template);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created template {TemplateId} for user {UserId}", template.Id, userId);
            return MapToTemplateDto(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template for user {UserId}", userId);
            throw;
        }
    }

    public async Task<TemplateDto?> UpdateTemplateAsync(Guid templateId, UpdateTemplateRequest request, Guid userId)
    {
        try
        {
            var template = await _context.Set<ProposalTemplate>()
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == templateId && t.UserId == userId);

            if (template == null)
            {
                _logger.LogWarning("Template {TemplateId} not found or user {UserId} not authorized", templateId, userId);
                return null;
            }

            // Users cannot modify system templates
            if (template.IsSystemTemplate)
            {
                _logger.LogWarning("User {UserId} attempted to modify system template {TemplateId}", userId, templateId);
                throw new UnauthorizedAccessException("Cannot modify system templates");
            }

            template.Name = request.Name;
            template.Description = request.Description;
            template.Category = request.Category;
            template.Tags = SerializeTags(request.Tags);
            template.Content = SerializeContent(request.Content);
            template.DefaultPricing = SerializePricing(request.DefaultPricing);
            template.IsPublic = request.IsPublic;
            template.ThumbnailUrl = request.ThumbnailUrl;
            template.EstimatedTimeMinutes = request.EstimatedTimeMinutes;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated template {TemplateId}", templateId);
            return MapToTemplateDto(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", templateId);
            throw;
        }
    }

    public async Task<bool> DeleteTemplateAsync(Guid templateId, Guid userId)
    {
        try
        {
            var template = await _context.Set<ProposalTemplate>()
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == templateId && t.UserId == userId);

            if (template == null)
            {
                _logger.LogWarning("Template {TemplateId} not found or user {UserId} not authorized", templateId, userId);
                return false;
            }

            // Users cannot delete system templates
            if (template.IsSystemTemplate)
            {
                _logger.LogWarning("User {UserId} attempted to delete system template {TemplateId}", userId, templateId);
                throw new UnauthorizedAccessException("Cannot delete system templates");
            }

            // Soft delete
            template.IsDeleted = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted template {TemplateId}", templateId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", templateId);
            throw;
        }
    }

    public async Task<TemplateDto> SaveProposalAsTemplateAsync(SaveAsTemplateRequest request, Guid userId)
    {
        try
        {
            // Get the proposal
            var proposal = await _context.Set<Proposal>()
                .Include(p => p.Sections)
                .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == request.ProposalId && p.UserId == userId);

            if (proposal == null)
            {
                throw new InvalidOperationException("Proposal not found or access denied");
            }

            // Create template content from proposal
            var content = new TemplateContentDto
            {
                Introduction = GetSectionContent(proposal.Sections, "Introduction"),
                ProblemStatement = GetSectionContent(proposal.Sections, "Problem Statement"),
                ProposedSolution = GetSectionContent(proposal.Sections, "Proposed Solution"),
                Methodology = GetSectionContent(proposal.Sections, "Methodology"),
                Deliverables = proposal.DeliverablesJson,
                Timeline = proposal.TimelineJson,
                TeamAndExperience = GetSectionContent(proposal.Sections, "Team and Experience"),
                TermsAndConditions = proposal.TermsAndConditions ?? string.Empty,
                CallToAction = GetSectionContent(proposal.Sections, "Call to Action")
            };

            // Create pricing template from proposal pricing
            var pricing = new PricingTemplateDto
            {
                Basic = DeserializePricingTier(proposal.BasicTierJson),
                Standard = DeserializePricingTier(proposal.StandardTierJson),
                Premium = DeserializePricingTier(proposal.PremiumTierJson)
            };

            var createRequest = new CreateTemplateRequest
            {
                Name = request.TemplateName,
                Description = request.Description,
                Category = request.Category,
                Tags = request.Tags,
                Content = content,
                DefaultPricing = pricing,
                IsPublic = request.IsPublic
            };

            return await CreateTemplateAsync(createRequest, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving proposal {ProposalId} as template", request.ProposalId);
            throw;
        }
    }

    public async Task<Guid> CreateProposalFromTemplateAsync(CreateFromTemplateRequest request, Guid userId)
    {
        try
        {
            var template = await _context.Set<ProposalTemplate>()
                .FirstOrDefaultAsync(t => !t.IsDeleted &&
                    t.Id == request.TemplateId &&
                    (t.IsSystemTemplate || t.UserId == userId || t.IsPublic));

            if (template == null)
            {
                throw new InvalidOperationException("Template not found or access denied");
            }

            var content = DeserializeContent(template.Content);
            var pricing = DeserializePricing(template.DefaultPricing);

            // Create new proposal
            var proposal = new Proposal
            {
                Title = request.ProposalTitle,
                UserId = userId,
                ClientId = request.ClientId,
                TemplateId = template.Id,
                Status = Domain.Enums.ProposalStatus.Draft,
                BasicTierJson = SerializePricingTier(pricing?.Basic),
                StandardTierJson = SerializePricingTier(pricing?.Standard),
                PremiumTierJson = SerializePricingTier(pricing?.Premium),
                DeliverablesJson = content.Deliverables,
                TimelineJson = content.Timeline,
                TermsAndConditions = content.TermsAndConditions
            };

            _context.Set<Proposal>().Add(proposal);

            // Create sections from template
            var sections = new[]
            {
                new ProposalSection { ProposalId = proposal.Id, Title = "Introduction", Content = content.Introduction, DisplayOrder = 1 },
                new ProposalSection { ProposalId = proposal.Id, Title = "Problem Statement", Content = content.ProblemStatement, DisplayOrder = 2 },
                new ProposalSection { ProposalId = proposal.Id, Title = "Proposed Solution", Content = content.ProposedSolution, DisplayOrder = 3 },
                new ProposalSection { ProposalId = proposal.Id, Title = "Methodology", Content = content.Methodology, DisplayOrder = 4 },
                new ProposalSection { ProposalId = proposal.Id, Title = "Team and Experience", Content = content.TeamAndExperience, DisplayOrder = 5 },
                new ProposalSection { ProposalId = proposal.Id, Title = "Call to Action", Content = content.CallToAction, DisplayOrder = 6 }
            };

            _context.Set<ProposalSection>().AddRange(sections);

            // Increment template usage
            template.UsageCount++;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Created proposal {ProposalId} from template {TemplateId}", proposal.Id, template.Id);
            return proposal.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating proposal from template {TemplateId}", request.TemplateId);
            throw;
        }
    }

    public async Task<TemplateDto> DuplicateTemplateAsync(Guid templateId, Guid userId)
    {
        try
        {
            var template = await GetTemplateByIdAsync(templateId, userId);
            if (template == null)
            {
                throw new InvalidOperationException("Template not found or access denied");
            }

            var createRequest = new CreateTemplateRequest
            {
                Name = $"{template.Name} (Copy)",
                Description = template.Description,
                Category = template.Category,
                Tags = template.Tags,
                Content = template.Content,
                DefaultPricing = template.DefaultPricing,
                IsPublic = false, // Duplicates are private by default
                ThumbnailUrl = template.ThumbnailUrl,
                EstimatedTimeMinutes = template.EstimatedTimeMinutes
            };

            return await CreateTemplateAsync(createRequest, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error duplicating template {TemplateId}", templateId);
            throw;
        }
    }

    public async Task<Dictionary<string, int>> GetTemplateCategoriesAsync(Guid userId)
    {
        try
        {
            var categories = await _context.Set<ProposalTemplate>()
                .Where(t => !t.IsDeleted &&
                    (t.IsSystemTemplate || t.UserId == userId || t.IsPublic))
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);

            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template categories");
            throw;
        }
    }

    public async Task IncrementUsageCountAsync(Guid templateId)
    {
        try
        {
            var template = await _context.Set<ProposalTemplate>()
                .FirstOrDefaultAsync(t => t.Id == templateId);

            if (template != null)
            {
                template.UsageCount++;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing usage count for template {TemplateId}", templateId);
            // Don't throw - this is not critical
        }
    }

    // Helper methods
    private TemplateDto MapToTemplateDto(ProposalTemplate template)
    {
        return new TemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            Category = template.Category,
            Tags = ParseTags(template.Tags),
            Content = DeserializeContent(template.Content),
            DefaultPricing = DeserializePricing(template.DefaultPricing),
            IsSystemTemplate = template.IsSystemTemplate,
            IsPublic = template.IsPublic,
            UserId = template.UserId,
            UserName = template.User != null ? $"{template.User.FirstName} {template.User.LastName}" : null,
            UsageCount = template.UsageCount,
            WinRate = template.WinRate,
            ThumbnailUrl = template.ThumbnailUrl,
            EstimatedTimeMinutes = template.EstimatedTimeMinutes,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }

    private static List<string> ParseTags(string? tagsJson)
    {
        if (string.IsNullOrWhiteSpace(tagsJson)) return new List<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(tagsJson) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static string SerializeTags(List<string> tags)
    {
        return JsonSerializer.Serialize(tags);
    }

    private static TemplateContentDto DeserializeContent(string contentJson)
    {
        try
        {
            return JsonSerializer.Deserialize<TemplateContentDto>(contentJson) ?? new TemplateContentDto();
        }
        catch
        {
            return new TemplateContentDto();
        }
    }

    private static string SerializeContent(TemplateContentDto content)
    {
        return JsonSerializer.Serialize(content);
    }

    private static PricingTemplateDto? DeserializePricing(string? pricingJson)
    {
        if (string.IsNullOrWhiteSpace(pricingJson)) return null;
        try
        {
            return JsonSerializer.Deserialize<PricingTemplateDto>(pricingJson);
        }
        catch
        {
            return null;
        }
    }

    private static string? SerializePricing(PricingTemplateDto? pricing)
    {
        if (pricing == null) return null;
        return JsonSerializer.Serialize(pricing);
    }

    private static string GetSectionContent(ICollection<ProposalSection> sections, string title)
    {
        return sections.FirstOrDefault(s => s.Title == title)?.Content ?? string.Empty;
    }

    private static PricingTierDto DeserializePricingTier(string tierJson)
    {
        try
        {
            return JsonSerializer.Deserialize<PricingTierDto>(tierJson) ?? new PricingTierDto();
        }
        catch
        {
            return new PricingTierDto();
        }
    }

    private static string SerializePricingTier(PricingTierDto? tier)
    {
        if (tier == null) return JsonSerializer.Serialize(new PricingTierDto());
        return JsonSerializer.Serialize(tier);
    }
}
