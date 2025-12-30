namespace ProposalPilot.Shared.DTOs.Proposal;

// Proposal Generation Result from Claude API
public record ProposalGenerationResult(
    string Title,
    ProposalSections Sections,
    ProposalMetadata Metadata
);

public record ProposalSections(
    string OpeningHook,
    string ProblemStatement,
    string ProposedSolution,
    string Methodology,
    string Timeline,
    InvestmentSection Investment,
    string WhyChooseUs,
    string NextSteps
);

public record InvestmentSection(
    string Intro,
    List<PricingTier> Tiers
);

public record PricingTier(
    string Name,
    decimal Price,
    string Description,
    List<string> Features,
    string Timeline,
    bool Highlighted = false
);

public record ProposalMetadata(
    int WordCount,
    string EstimatedReadTime,
    string Tone
);

// Request DTOs
public record GenerateProposalRequest(
    Guid BriefId,
    Guid ClientId,
    string? PreferredTone = null,
    string? ProposalLength = null,
    string? Emphasis = null,
    Guid? TemplateId = null
);

public record UpdateProposalRequest(
    string? Title = null,
    string? Description = null,
    Dictionary<string, string>? Sections = null
);

/// <summary>
/// Request to send a proposal via email
/// </summary>
public record SendProposalEmailApiRequest(
    string RecipientEmail,
    string RecipientName,
    string? PersonalMessage = null
);

/// <summary>
/// Response after sending a proposal email
/// </summary>
public record SendProposalEmailResponse(
    bool Success,
    string Message,
    DateTime? SentAt = null
);
