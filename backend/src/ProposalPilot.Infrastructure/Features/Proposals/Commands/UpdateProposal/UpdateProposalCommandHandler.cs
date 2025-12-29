namespace ProposalPilot.Infrastructure.Features.Proposals.Commands.UpdateProposal;

using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Features.Proposals.Commands.UpdateProposal;
using ProposalPilot.Infrastructure.Data;

public class UpdateProposalCommandHandler : IRequestHandler<UpdateProposalCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UpdateProposalCommandHandler> _logger;

    public UpdateProposalCommandHandler(
        ApplicationDbContext context,
        ILogger<UpdateProposalCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateProposalCommand request, CancellationToken cancellationToken)
    {
        var proposal = await _context.Proposals
            .FirstOrDefaultAsync(p => p.Id == request.ProposalId && p.UserId == request.UserId, cancellationToken);

        if (proposal == null)
        {
            _logger.LogWarning("Proposal {ProposalId} not found for user {UserId}", request.ProposalId, request.UserId);
            return false;
        }

        try
        {
            // Update title and description if provided
            if (!string.IsNullOrEmpty(request.Title))
            {
                proposal.Title = request.Title;
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                proposal.Description = request.Description;
            }

            // Update sections if provided
            if (request.Sections != null && request.Sections.Count > 0)
            {
                // Parse existing content
                var existingContent = string.IsNullOrEmpty(proposal.DeliverablesJson)
                    ? new { sections = new Dictionary<string, string>(), investment = new { intro = "", tiers = new List<object>() }, metadata = new { } }
                    : JsonSerializer.Deserialize<dynamic>(proposal.DeliverablesJson);

                // Get existing sections or create new
                var sections = new Dictionary<string, string>();

                if (existingContent != null)
                {
                    try
                    {
                        var contentDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(proposal.DeliverablesJson ?? "{}");
                        if (contentDict != null && contentDict.ContainsKey("sections"))
                        {
                            var sectionsElement = contentDict["sections"];
                            var existingSections = JsonSerializer.Deserialize<Dictionary<string, string>>(sectionsElement.GetRawText());
                            if (existingSections != null)
                            {
                                sections = new Dictionary<string, string>(existingSections);
                            }
                        }
                    }
                    catch
                    {
                        // If parsing fails, start with empty sections
                    }
                }

                // Update with new section values
                foreach (var section in request.Sections)
                {
                    sections[section.Key] = section.Value;
                }

                // Rebuild the full content JSON
                var updatedContent = new
                {
                    sections = sections,
                    investment = existingContent?.GetProperty("investment"),
                    metadata = existingContent?.GetProperty("metadata")
                };

                proposal.DeliverablesJson = JsonSerializer.Serialize(updatedContent, new JsonSerializerOptions { WriteIndented = true });
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated proposal {ProposalId}", request.ProposalId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating proposal {ProposalId}", request.ProposalId);
            throw;
        }
    }
}
