using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Shared.Configuration;
using ProposalPilot.Shared.DTOs.Claude;

namespace ProposalPilot.Infrastructure.Services;

public class ClaudeApiServiceWithCache : ClaudeApiService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<ClaudeApiServiceWithCache> _loggerWithCache;

    public ClaudeApiServiceWithCache(
        HttpClient httpClient,
        IOptions<AnthropicSettings> settings,
        ICacheService cacheService,
        ILogger<ClaudeApiServiceWithCache> logger)
        : base(httpClient, settings, logger)
    {
        _cacheService = cacheService;
        _loggerWithCache = logger;
    }

    public async Task<ClaudeResponse> SendMessageWithCacheAsync(
        string message,
        string? systemPrompt = null,
        TimeSpan? cacheExpiration = null,
        CancellationToken cancellationToken = default)
    {
        // Generate cache key
        var cacheKey = GenerateCacheKey(message, systemPrompt);

        // Try to get from cache
        var cachedResponse = await _cacheService.GetAsync<CachedClaudeResponse>(cacheKey, cancellationToken);

        if (cachedResponse != null)
        {
            _loggerWithCache.LogInformation("Cache hit for Claude request. Cached at: {CachedAt}", cachedResponse.CachedAt);
            return cachedResponse.Response;
        }

        // Cache miss - call API
        _loggerWithCache.LogInformation("Cache miss for Claude request. Calling API...");
        var response = await SendMessageAsync(message, systemPrompt, cancellationToken);

        // Cache the response
        var cached = new CachedClaudeResponse
        {
            Response = response,
            CachedAt = DateTime.UtcNow,
            CacheKey = cacheKey
        };

        await _cacheService.SetAsync(
            cacheKey,
            cached,
            cacheExpiration ?? TimeSpan.FromHours(24),
            cancellationToken);

        _loggerWithCache.LogInformation("Response cached with key: {CacheKey}", cacheKey);

        return response;
    }

    private string GenerateCacheKey(string message, string? systemPrompt)
    {
        // Create a hash of the message and system prompt for cache key
        var content = $"{message}|{systemPrompt ?? ""}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(content));
        var hash = Convert.ToHexString(hashBytes)[..16].ToLower();

        return _cacheService.GenerateKey("claude", "message", hash);
    }
}
