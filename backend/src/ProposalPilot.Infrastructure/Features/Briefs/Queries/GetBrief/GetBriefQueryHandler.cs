namespace ProposalPilot.Infrastructure.Features.Briefs.Queries.GetBrief;

using MediatR;
using Microsoft.EntityFrameworkCore;
using ProposalPilot.Application.Features.Briefs.Queries.GetBrief;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.DTOs.Brief;

public class GetBriefQueryHandler : IRequestHandler<GetBriefQuery, BriefDto?>
{
    private readonly ApplicationDbContext _context;

    public GetBriefQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BriefDto?> Handle(GetBriefQuery request, CancellationToken cancellationToken)
    {
        var brief = await _context.Briefs
            .Where(b => b.Id == request.BriefId && b.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (brief == null)
            return null;

        return new BriefDto(
            brief.Id,
            brief.Title,
            brief.RawContent,
            brief.AnalyzedContent,
            brief.ProjectType,
            brief.Industry,
            brief.EstimatedBudget,
            brief.Timeline,
            brief.KeyRequirements,
            brief.TechnicalRequirements,
            brief.TargetAudience,
            brief.Status.ToString(),
            brief.AnalyzedAt,
            brief.TokensUsed,
            brief.AnalysisCost,
            brief.CreatedAt
        );
    }
}
