using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProposalPilot.Domain.Enums;
using ProposalPilot.Infrastructure.Data;
using System.Text.Json;

namespace ProposalPilot.API.Controllers;

/// <summary>
/// Handles SendGrid webhook events for email tracking
/// </summary>
[ApiController]
[Route("api/webhooks/sendgrid")]
public class SendGridWebhookController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SendGridWebhookController> _logger;

    public SendGridWebhookController(
        ApplicationDbContext context,
        ILogger<SendGridWebhookController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Handle SendGrid email events (delivered, opened, clicked, bounced, etc.)
    /// </summary>
    [HttpPost("events")]
    public async Task<IActionResult> HandleEvents([FromBody] JsonElement[] events)
    {
        _logger.LogInformation("Received {Count} SendGrid webhook events", events.Length);

        foreach (var evt in events)
        {
            try
            {
                await ProcessEventAsync(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SendGrid event: {Event}", evt.GetRawText());
                // Continue processing other events
            }
        }

        return Ok();
    }

    private async Task ProcessEventAsync(JsonElement evt)
    {
        var eventType = evt.GetProperty("event").GetString();
        var messageId = evt.TryGetProperty("sg_message_id", out var msgId)
            ? msgId.GetString()?.Split('.').FirstOrDefault()
            : null;

        if (string.IsNullOrEmpty(messageId))
        {
            _logger.LogWarning("SendGrid event missing message ID: {EventType}", eventType);
            return;
        }

        var emailLog = await _context.EmailLogs
            .FirstOrDefaultAsync(e => e.SendGridMessageId == messageId);

        if (emailLog == null)
        {
            _logger.LogWarning("EmailLog not found for SendGrid message ID: {MessageId}", messageId);
            return;
        }

        var timestamp = evt.TryGetProperty("timestamp", out var ts)
            ? DateTimeOffset.FromUnixTimeSeconds(ts.GetInt64()).UtcDateTime
            : DateTime.UtcNow;

        switch (eventType?.ToLowerInvariant())
        {
            case "delivered":
                await HandleDeliveredAsync(emailLog, timestamp);
                break;

            case "open":
                await HandleOpenAsync(emailLog, evt, timestamp);
                break;

            case "click":
                await HandleClickAsync(emailLog, evt, timestamp);
                break;

            case "bounce":
                await HandleBounceAsync(emailLog, evt, timestamp);
                break;

            case "dropped":
                await HandleDroppedAsync(emailLog, evt, timestamp);
                break;

            case "spamreport":
                await HandleSpamReportAsync(emailLog, timestamp);
                break;

            case "unsubscribe":
                await HandleUnsubscribeAsync(emailLog, timestamp);
                break;

            default:
                _logger.LogDebug("Unhandled SendGrid event type: {EventType}", eventType);
                break;
        }

        await _context.SaveChangesAsync();
    }

    private async Task HandleDeliveredAsync(Domain.Entities.EmailLog emailLog, DateTime timestamp)
    {
        emailLog.Status = EmailStatus.Delivered;
        emailLog.DeliveredAt = timestamp;

        _logger.LogInformation("Email delivered: {EmailLogId} to {Email}",
            emailLog.Id, emailLog.RecipientEmail);

        // Update proposal SentAt if this is the first delivery
        var proposal = await _context.Proposals.FindAsync(emailLog.ProposalId);
        if (proposal != null && proposal.SentAt == null)
        {
            proposal.SentAt = timestamp;
        }
    }

    private async Task HandleOpenAsync(Domain.Entities.EmailLog emailLog, JsonElement evt, DateTime timestamp)
    {
        emailLog.OpenCount++;
        emailLog.LastOpenedAt = timestamp;

        if (emailLog.FirstOpenedAt == null)
        {
            emailLog.FirstOpenedAt = timestamp;
            emailLog.Status = EmailStatus.Opened;
        }

        // Capture user agent and IP
        if (evt.TryGetProperty("useragent", out var ua))
            emailLog.LastUserAgent = ua.GetString();
        if (evt.TryGetProperty("ip", out var ip))
            emailLog.LastIpAddress = ip.GetString();

        _logger.LogInformation("Email opened: {EmailLogId}, Open count: {Count}",
            emailLog.Id, emailLog.OpenCount);

        // Update proposal first viewed
        var proposal = await _context.Proposals.FindAsync(emailLog.ProposalId);
        if (proposal != null)
        {
            if (proposal.FirstViewedAt == null)
                proposal.FirstViewedAt = timestamp;
            proposal.LastViewedAt = timestamp;
            proposal.ViewCount++;

            // Update status if still in Sent status
            if (proposal.Status == ProposalStatus.Sent)
                proposal.Status = ProposalStatus.Viewed;
        }
    }

    private Task HandleClickAsync(Domain.Entities.EmailLog emailLog, JsonElement evt, DateTime timestamp)
    {
        emailLog.ClickCount++;
        emailLog.LastClickedAt = timestamp;

        if (emailLog.FirstClickedAt == null)
        {
            emailLog.FirstClickedAt = timestamp;
            emailLog.Status = EmailStatus.Clicked;
            emailLog.UniqueClickCount++;
        }

        // Track clicked URL
        if (evt.TryGetProperty("url", out var url))
        {
            var clickedLinks = string.IsNullOrEmpty(emailLog.ClickedLinksJson)
                ? new List<ClickedLink>()
                : JsonSerializer.Deserialize<List<ClickedLink>>(emailLog.ClickedLinksJson) ?? new List<ClickedLink>();

            clickedLinks.Add(new ClickedLink
            {
                Url = url.GetString() ?? "",
                ClickedAt = timestamp
            });

            emailLog.ClickedLinksJson = JsonSerializer.Serialize(clickedLinks);
        }

        _logger.LogInformation("Email link clicked: {EmailLogId}, Click count: {Count}",
            emailLog.Id, emailLog.ClickCount);

        return Task.CompletedTask;
    }

    private Task HandleBounceAsync(Domain.Entities.EmailLog emailLog, JsonElement evt, DateTime timestamp)
    {
        emailLog.Status = EmailStatus.Bounced;
        emailLog.BouncedAt = timestamp;

        if (evt.TryGetProperty("type", out var bounceType))
            emailLog.BounceType = bounceType.GetString();
        if (evt.TryGetProperty("reason", out var reason))
            emailLog.BounceReason = reason.GetString();

        _logger.LogWarning("Email bounced: {EmailLogId} to {Email}, Type: {BounceType}, Reason: {Reason}",
            emailLog.Id, emailLog.RecipientEmail, emailLog.BounceType, emailLog.BounceReason);

        return Task.CompletedTask;
    }

    private Task HandleDroppedAsync(Domain.Entities.EmailLog emailLog, JsonElement evt, DateTime timestamp)
    {
        emailLog.Status = EmailStatus.Failed;

        if (evt.TryGetProperty("reason", out var reason))
            emailLog.BounceReason = $"Dropped: {reason.GetString()}";

        _logger.LogWarning("Email dropped: {EmailLogId} to {Email}, Reason: {Reason}",
            emailLog.Id, emailLog.RecipientEmail, emailLog.BounceReason);

        return Task.CompletedTask;
    }

    private Task HandleSpamReportAsync(Domain.Entities.EmailLog emailLog, DateTime timestamp)
    {
        emailLog.Status = EmailStatus.SpamReported;
        emailLog.SpamReportedAt = timestamp;

        _logger.LogWarning("Email marked as spam: {EmailLogId} by {Email}",
            emailLog.Id, emailLog.RecipientEmail);

        return Task.CompletedTask;
    }

    private Task HandleUnsubscribeAsync(Domain.Entities.EmailLog emailLog, DateTime timestamp)
    {
        emailLog.Status = EmailStatus.Unsubscribed;
        emailLog.UnsubscribedAt = timestamp;

        _logger.LogInformation("User unsubscribed: {EmailLogId}, Email: {Email}",
            emailLog.Id, emailLog.RecipientEmail);

        return Task.CompletedTask;
    }

    private class ClickedLink
    {
        public string Url { get; set; } = string.Empty;
        public DateTime ClickedAt { get; set; }
    }
}
