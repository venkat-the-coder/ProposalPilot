namespace ProposalPilot.Shared.DTOs.Claude;

public class ClaudeResponse
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<ClaudeContent> Content { get; set; } = new();
    public string Model { get; set; } = string.Empty;
    public string StopReason { get; set; } = string.Empty;
    public ClaudeUsage? Usage { get; set; }
}

public class ClaudeContent
{
    public string Type { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class ClaudeUsage
{
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
}
