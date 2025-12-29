namespace ProposalPilot.Shared.DTOs.Claude;

public class ClaudeRequest
{
    public string Model { get; set; } = string.Empty;
    public int MaxTokens { get; set; }
    public List<ClaudeMessage> Messages { get; set; } = new();
    public string? System { get; set; }
    public double Temperature { get; set; } = 1.0;
}
