namespace ProposalPilot.Shared.DTOs.Template;

/// <summary>
/// Request to create a proposal from a template
/// </summary>
public class CreateFromTemplateRequest
{
    public Guid TemplateId { get; set; }
    public Guid ClientId { get; set; }
    public string ProposalTitle { get; set; } = string.Empty;
    public Dictionary<string, string>? Customizations { get; set; }
}
