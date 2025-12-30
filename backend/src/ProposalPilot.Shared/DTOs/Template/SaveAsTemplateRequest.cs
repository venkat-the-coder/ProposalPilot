namespace ProposalPilot.Shared.DTOs.Template;

/// <summary>
/// Request to save an existing proposal as a template
/// </summary>
public class SaveAsTemplateRequest
{
    public Guid ProposalId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; } = false;
}
