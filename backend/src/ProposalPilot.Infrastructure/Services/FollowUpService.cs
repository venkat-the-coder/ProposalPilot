using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Domain.Enums;
using ProposalPilot.Infrastructure.Data;

namespace ProposalPilot.Infrastructure.Services;

/// <summary>
/// Service for managing proposal follow-up emails with Hangfire scheduling
/// </summary>
public class FollowUpService : IFollowUpService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IEngagementService _engagementService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<FollowUpService> _logger;

    public FollowUpService(
        ApplicationDbContext context,
        IEmailService emailService,
        IEngagementService engagementService,
        IBackgroundJobClient backgroundJobClient,
        ILogger<FollowUpService> logger)
    {
        _context = context;
        _emailService = emailService;
        _engagementService = engagementService;
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public async Task<ScheduleFollowUpResult> ScheduleFollowUpAsync(ScheduleFollowUpRequest request)
    {
        // Validate proposal exists and belongs to user
        var proposal = await _context.Proposals
            .Include(p => p.Client)
            .Include(p => p.User)
            .Include(p => p.FollowUps)
            .FirstOrDefaultAsync(p => p.Id == request.ProposalId && p.UserId == request.UserId);

        if (proposal == null)
        {
            return new ScheduleFollowUpResult(false, null, "Proposal not found");
        }

        // Check if proposal has been sent
        if (!proposal.SentAt.HasValue)
        {
            return new ScheduleFollowUpResult(false, null, "Proposal hasn't been sent yet");
        }

        // Check proposal status
        if (proposal.Status == ProposalStatus.Accepted)
        {
            return new ScheduleFollowUpResult(false, null, "Proposal already accepted");
        }

        if (proposal.Status == ProposalStatus.Rejected)
        {
            return new ScheduleFollowUpResult(false, null, "Proposal was rejected");
        }

        // Check for existing pending follow-ups
        var pendingFollowUp = proposal.FollowUps.FirstOrDefault(f => f.Status == FollowUpStatus.Scheduled);
        if (pendingFollowUp != null)
        {
            return new ScheduleFollowUpResult(
                false,
                pendingFollowUp.Id,
                $"A follow-up is already scheduled for {pendingFollowUp.ScheduledFor:MMM dd, yyyy HH:mm}"
            );
        }

        // Check maximum follow-ups (3)
        var sentFollowUpsCount = proposal.FollowUps.Count(f => f.Status == FollowUpStatus.Sent);
        if (sentFollowUpsCount >= 3)
        {
            return new ScheduleFollowUpResult(false, null, "Maximum follow-ups reached (3)");
        }

        // Validate scheduled time is in the future
        if (request.ScheduledFor <= DateTime.UtcNow)
        {
            return new ScheduleFollowUpResult(false, null, "Scheduled time must be in the future");
        }

        // Determine sequence number
        var sequenceNumber = sentFollowUpsCount + 1;

        // Generate follow-up content
        var recommendation = await _engagementService.GetFollowUpRecommendationAsync(proposal.Id);
        var message = !string.IsNullOrEmpty(request.CustomMessage)
            ? request.CustomMessage
            : recommendation.SuggestedMessage;

        var subject = sequenceNumber switch
        {
            1 => $"Following up: {proposal.Title}",
            2 => $"Quick check-in: {proposal.Title}",
            _ => $"Regarding our proposal: {proposal.Title}"
        };

        // Create follow-up record
        var followUp = new FollowUp
        {
            ProposalId = proposal.Id,
            SequenceNumber = sequenceNumber,
            ScheduledFor = request.ScheduledFor,
            Status = FollowUpStatus.Scheduled,
            Subject = subject,
            Content = message,
            TriggerReason = request.IsAutomatic ? "Automatic based on engagement" : "Manually scheduled"
        };

        _context.FollowUps.Add(followUp);
        await _context.SaveChangesAsync();

        // Schedule Hangfire job
        var delay = request.ScheduledFor - DateTime.UtcNow;
        var jobId = _backgroundJobClient.Schedule<IFollowUpService>(
            service => service.ProcessScheduledFollowUpAsync(followUp.Id),
            delay
        );

        _logger.LogInformation(
            "Scheduled follow-up {FollowUpId} for proposal {ProposalId} at {ScheduledFor}, HangfireJobId: {JobId}",
            followUp.Id, proposal.Id, request.ScheduledFor, jobId);

        return new ScheduleFollowUpResult(
            true,
            followUp.Id,
            $"Follow-up scheduled for {request.ScheduledFor:MMM dd, yyyy HH:mm} UTC",
            request.ScheduledFor
        );
    }

    public async Task<bool> CancelFollowUpAsync(Guid followUpId, Guid userId)
    {
        var followUp = await _context.FollowUps
            .Include(f => f.Proposal)
            .FirstOrDefaultAsync(f => f.Id == followUpId && f.Proposal.UserId == userId);

        if (followUp == null)
        {
            return false;
        }

        if (followUp.Status != FollowUpStatus.Scheduled)
        {
            return false; // Can only cancel scheduled follow-ups
        }

        followUp.Status = FollowUpStatus.Cancelled;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cancelled follow-up {FollowUpId} for proposal {ProposalId}",
            followUpId, followUp.ProposalId);

        return true;
    }

    public async Task<List<FollowUpDto>> GetFollowUpsAsync(Guid proposalId, Guid userId)
    {
        var followUps = await _context.FollowUps
            .Include(f => f.Proposal)
            .Where(f => f.ProposalId == proposalId && f.Proposal.UserId == userId)
            .OrderByDescending(f => f.ScheduledFor)
            .ToListAsync();

        return followUps.Select(f => new FollowUpDto(
            f.Id,
            f.ProposalId,
            f.SequenceNumber,
            f.ScheduledFor,
            f.SentAt,
            f.Status.ToString(),
            f.Subject,
            f.TriggerReason,
            f.Status == FollowUpStatus.Scheduled // Can only cancel if scheduled
        )).ToList();
    }

    public async Task ProcessScheduledFollowUpAsync(Guid followUpId)
    {
        var followUp = await _context.FollowUps
            .Include(f => f.Proposal)
                .ThenInclude(p => p.User)
            .Include(f => f.Proposal)
                .ThenInclude(p => p.Client)
            .FirstOrDefaultAsync(f => f.Id == followUpId);

        if (followUp == null)
        {
            _logger.LogWarning("Follow-up {FollowUpId} not found for processing", followUpId);
            return;
        }

        if (followUp.Status != FollowUpStatus.Scheduled)
        {
            _logger.LogInformation("Follow-up {FollowUpId} is no longer scheduled (status: {Status})",
                followUpId, followUp.Status);
            return;
        }

        var proposal = followUp.Proposal;

        // Check if proposal status has changed
        if (proposal.Status == ProposalStatus.Accepted)
        {
            followUp.Status = FollowUpStatus.Skipped;
            followUp.TriggerReason += " - Skipped: Proposal accepted";
            await _context.SaveChangesAsync();
            _logger.LogInformation("Skipped follow-up {FollowUpId}: Proposal accepted", followUpId);
            return;
        }

        if (proposal.Status == ProposalStatus.Rejected)
        {
            followUp.Status = FollowUpStatus.Skipped;
            followUp.TriggerReason += " - Skipped: Proposal rejected";
            await _context.SaveChangesAsync();
            _logger.LogInformation("Skipped follow-up {FollowUpId}: Proposal rejected", followUpId);
            return;
        }

        // Prepare and send the email
        try
        {
            var clientEmail = proposal.Client?.Email ?? "";
            var clientName = proposal.Client?.Name ?? proposal.Client?.CompanyName ?? "there";

            if (string.IsNullOrEmpty(clientEmail))
            {
                followUp.Status = FollowUpStatus.Failed;
                followUp.TriggerReason += " - Failed: No client email";
                await _context.SaveChangesAsync();
                _logger.LogWarning("Follow-up {FollowUpId} failed: No client email", followUpId);
                return;
            }

            var daysSinceSent = proposal.SentAt.HasValue
                ? (int)(DateTime.UtcNow - proposal.SentAt.Value).TotalDays
                : 0;

            var shareUrl = $"https://proposalpilot.com/share/{proposal.ShareToken}";

            var emailRequest = new SendFollowUpEmailRequest(
                ProposalId: proposal.Id,
                FollowUpId: followUp.Id,
                RecipientEmail: clientEmail,
                RecipientName: clientName,
                SenderName: $"{proposal.User.FirstName} {proposal.User.LastName}",
                SenderEmail: proposal.User.Email,
                ProposalTitle: proposal.Title,
                FollowUpMessage: followUp.Content,
                ProposalViewUrl: shareUrl,
                DaysSinceProposalSent: daysSinceSent
            );

            var result = await _emailService.SendFollowUpAsync(emailRequest);

            if (result.Success)
            {
                followUp.Status = FollowUpStatus.Sent;
                followUp.SentAt = DateTime.UtcNow;
                _logger.LogInformation("Follow-up {FollowUpId} sent successfully to {Email}",
                    followUpId, clientEmail);
            }
            else
            {
                followUp.Status = FollowUpStatus.Failed;
                followUp.TriggerReason += $" - Failed: {result.ErrorMessage}";
                _logger.LogError("Follow-up {FollowUpId} failed: {Error}", followUpId, result.ErrorMessage);
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            followUp.Status = FollowUpStatus.Failed;
            followUp.TriggerReason += $" - Failed: {ex.Message}";
            await _context.SaveChangesAsync();
            _logger.LogError(ex, "Error processing follow-up {FollowUpId}", followUpId);
        }
    }

    public async Task ProcessAutomaticFollowUpsAsync()
    {
        _logger.LogInformation("Processing automatic follow-ups");

        // Get all sent proposals that might need follow-ups
        var proposals = await _context.Proposals
            .Include(p => p.Client)
            .Include(p => p.User)
            .Include(p => p.FollowUps)
            .Include(p => p.EmailLogs)
            .Where(p =>
                p.SentAt.HasValue &&
                p.Status == ProposalStatus.Sent &&
                p.FollowUps.Count(f => f.Status == FollowUpStatus.Sent) < 3 &&
                !p.FollowUps.Any(f => f.Status == FollowUpStatus.Scheduled))
            .ToListAsync();

        _logger.LogInformation("Found {Count} proposals eligible for automatic follow-ups", proposals.Count);

        foreach (var proposal in proposals)
        {
            try
            {
                var recommendation = await _engagementService.GetFollowUpRecommendationAsync(proposal.Id);

                if (recommendation.ShouldFollowUp && recommendation.RecommendedDelayDays == 0)
                {
                    // Schedule follow-up for tomorrow at 10 AM UTC (business friendly time)
                    var scheduledFor = DateTime.UtcNow.Date.AddDays(1).AddHours(10);

                    var result = await ScheduleFollowUpAsync(new ScheduleFollowUpRequest(
                        ProposalId: proposal.Id,
                        UserId: proposal.UserId,
                        ScheduledFor: scheduledFor,
                        CustomMessage: recommendation.SuggestedMessage,
                        IsAutomatic: true
                    ));

                    if (result.Success)
                    {
                        _logger.LogInformation(
                            "Automatically scheduled follow-up for proposal {ProposalId}: {Reason}",
                            proposal.Id, recommendation.Reason);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing automatic follow-up for proposal {ProposalId}",
                    proposal.Id);
            }
        }
    }
}
