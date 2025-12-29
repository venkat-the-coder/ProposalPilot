namespace ProposalPilot.Infrastructure.Features.Briefs.Commands.CreateBrief;

using MediatR;
using ProposalPilot.Application.Features.Briefs.Commands.CreateBrief;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Domain.Enums;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Shared.DTOs.Brief;

public class CreateBriefCommandHandler : IRequestHandler<CreateBriefCommand, BriefDto>
{
    private readonly ApplicationDbContext _context;

    public CreateBriefCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BriefDto> Handle(CreateBriefCommand request, CancellationToken cancellationToken)
    {
        var brief = new Brief
        {
            UserId = request.UserId,
            Title = request.Title,
            RawContent = request.RawContent,
            Status = BriefStatus.Draft
        };

        _context.Briefs.Add(brief);
        await _context.SaveChangesAsync(cancellationToken);

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
