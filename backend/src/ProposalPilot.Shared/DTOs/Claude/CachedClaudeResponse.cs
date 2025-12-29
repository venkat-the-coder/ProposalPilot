namespace ProposalPilot.Shared.DTOs.Claude;

public class CachedClaudeResponse
{
    public ClaudeResponse Response { get; set; } = new();
    public DateTime CachedAt { get; set; }
    public string CacheKey { get; set; } = string.Empty;
}
