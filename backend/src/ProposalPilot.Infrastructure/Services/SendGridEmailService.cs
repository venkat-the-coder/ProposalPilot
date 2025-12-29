using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using ProposalPilot.Application.Interfaces;
using ProposalPilot.Shared.Configuration;

namespace ProposalPilot.Infrastructure.Services;

/// <summary>
/// Email service implementation using SendGrid
/// </summary>
public class SendGridEmailService : IEmailService
{
    private readonly SendGridSettings _settings;
    private readonly ILogger<SendGridEmailService> _logger;
    private readonly ISendGridClient _client;

    public SendGridEmailService(
        IOptions<SendGridSettings> settings,
        ILogger<SendGridEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _client = new SendGridClient(_settings.ApiKey);
    }

    public async Task<EmailResult> SendProposalAsync(SendProposalEmailRequest request)
    {
        _logger.LogInformation("Sending proposal email to {Email} for proposal: {Title}",
            request.RecipientEmail, request.ProposalTitle);

        var htmlContent = GenerateProposalEmailHtml(request);
        var plainTextContent = GenerateProposalEmailPlainText(request);

        var emailRequest = new EmailRequest(
            To: request.RecipientEmail,
            ToName: request.RecipientName,
            Subject: $"Proposal: {request.ProposalTitle}",
            HtmlContent: htmlContent,
            PlainTextContent: plainTextContent,
            ReplyTo: request.SenderEmail
        );

        return await SendEmailAsync(emailRequest);
    }

    public async Task<EmailResult> SendFollowUpAsync(SendFollowUpEmailRequest request)
    {
        _logger.LogInformation("Sending follow-up email to {Email} for proposal: {Title}",
            request.RecipientEmail, request.ProposalTitle);

        var htmlContent = GenerateFollowUpEmailHtml(request);
        var plainTextContent = GenerateFollowUpEmailPlainText(request);

        var subject = request.DaysSinceProposalSent switch
        {
            <= 3 => $"Following up: {request.ProposalTitle}",
            <= 7 => $"Quick check-in: {request.ProposalTitle}",
            _ => $"Regarding our proposal: {request.ProposalTitle}"
        };

        var emailRequest = new EmailRequest(
            To: request.RecipientEmail,
            ToName: request.RecipientName,
            Subject: subject,
            HtmlContent: htmlContent,
            PlainTextContent: plainTextContent,
            ReplyTo: request.SenderEmail
        );

        return await SendEmailAsync(emailRequest);
    }

    public async Task<EmailResult> SendEmailAsync(EmailRequest request)
    {
        try
        {
            var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
            var to = new EmailAddress(request.To, request.ToName);

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                request.Subject,
                request.PlainTextContent ?? StripHtml(request.HtmlContent),
                request.HtmlContent
            );

            // Add reply-to if specified
            if (!string.IsNullOrEmpty(request.ReplyTo))
            {
                msg.ReplyTo = new EmailAddress(request.ReplyTo);
            }

            // Add tracking settings
            msg.SetClickTracking(true, true);
            msg.SetOpenTracking(true);

            var response = await _client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                var messageId = response.Headers.TryGetValues("X-Message-Id", out var values)
                    ? values.FirstOrDefault()
                    : null;

                _logger.LogInformation("Email sent successfully to {Email}, MessageId: {MessageId}",
                    request.To, messageId);

                return new EmailResult(true, messageId);
            }
            else
            {
                var body = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send email to {Email}. Status: {StatusCode}, Body: {Body}",
                    request.To, response.StatusCode, body);

                return new EmailResult(false, ErrorMessage: $"Failed to send email: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending email to {Email}", request.To);
            return new EmailResult(false, ErrorMessage: "An error occurred while sending the email");
        }
    }

    #region Email Templates

    private string GenerateProposalEmailHtml(SendProposalEmailRequest request)
    {
        var personalMessageSection = string.IsNullOrEmpty(request.PersonalMessage)
            ? ""
            : $@"<p style=""color: #374151; font-size: 16px; line-height: 1.6; margin-bottom: 20px; font-style: italic; border-left: 4px solid #3B82F6; padding-left: 16px;"">""{request.PersonalMessage}""</p>";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Proposal: {HtmlEncode(request.ProposalTitle)}</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #F3F4F6;"">
    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #F3F4F6; padding: 40px 20px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" width=""600"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #FFFFFF; border-radius: 12px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #3B82F6 0%, #1D4ED8 100%); padding: 40px; border-radius: 12px 12px 0 0; text-align: center;"">
                            <h1 style=""color: #FFFFFF; margin: 0; font-size: 28px; font-weight: 600;"">New Proposal</h1>
                            <p style=""color: #BFDBFE; margin: 10px 0 0 0; font-size: 16px;"">from {HtmlEncode(request.SenderName)}</p>
                        </td>
                    </tr>

                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px;"">
                            <p style=""color: #374151; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;"">
                                Hi {HtmlEncode(request.RecipientName)},
                            </p>

                            <p style=""color: #374151; font-size: 16px; line-height: 1.6; margin-bottom: 20px;"">
                                I've prepared a proposal for you regarding <strong>{HtmlEncode(request.ProposalTitle)}</strong>.
                            </p>

                            {personalMessageSection}

                            <p style=""color: #374151; font-size: 16px; line-height: 1.6; margin-bottom: 30px;"">
                                {HtmlEncode(request.ProposalSummary)}
                            </p>

                            <!-- CTA Button -->
                            <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"">
                                <tr>
                                    <td align=""center"">
                                        <a href=""{HtmlEncode(request.ProposalViewUrl)}""
                                           style=""display: inline-block; background: linear-gradient(135deg, #3B82F6 0%, #1D4ED8 100%); color: #FFFFFF; padding: 16px 40px; text-decoration: none; border-radius: 8px; font-size: 16px; font-weight: 600; box-shadow: 0 4px 6px rgba(59, 130, 246, 0.3);"">
                                            View Full Proposal
                                        </a>
                                    </td>
                                </tr>
                            </table>

                            <p style=""color: #6B7280; font-size: 14px; line-height: 1.6; margin-top: 30px; text-align: center;"">
                                Or copy this link: <a href=""{HtmlEncode(request.ProposalViewUrl)}"" style=""color: #3B82F6;"">{HtmlEncode(request.ProposalViewUrl)}</a>
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #F9FAFB; padding: 30px; border-radius: 0 0 12px 12px; text-align: center; border-top: 1px solid #E5E7EB;"">
                            <p style=""color: #6B7280; font-size: 14px; margin: 0 0 10px 0;"">
                                Sent by {HtmlEncode(request.SenderName)} via ProposalPilot
                            </p>
                            <p style=""color: #9CA3AF; font-size: 12px; margin: 0;"">
                                Questions? Reply directly to this email.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string GenerateProposalEmailPlainText(SendProposalEmailRequest request)
    {
        var personalMessage = string.IsNullOrEmpty(request.PersonalMessage)
            ? ""
            : $"\n\"{request.PersonalMessage}\"\n";

        return $@"
NEW PROPOSAL from {request.SenderName}

Hi {request.RecipientName},

I've prepared a proposal for you regarding ""{request.ProposalTitle}"".
{personalMessage}
{request.ProposalSummary}

View the full proposal here:
{request.ProposalViewUrl}

---
Sent via ProposalPilot
Questions? Reply directly to this email.
";
    }

    private string GenerateFollowUpEmailHtml(SendFollowUpEmailRequest request)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Following up: {HtmlEncode(request.ProposalTitle)}</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #F3F4F6;"">
    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #F3F4F6; padding: 40px 20px;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" width=""600"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #FFFFFF; border-radius: 12px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #10B981 0%, #059669 100%); padding: 40px; border-radius: 12px 12px 0 0; text-align: center;"">
                            <h1 style=""color: #FFFFFF; margin: 0; font-size: 28px; font-weight: 600;"">Quick Follow-up</h1>
                            <p style=""color: #A7F3D0; margin: 10px 0 0 0; font-size: 16px;"">Regarding: {HtmlEncode(request.ProposalTitle)}</p>
                        </td>
                    </tr>

                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px;"">
                            <p style=""color: #374151; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;"">
                                Hi {HtmlEncode(request.RecipientName)},
                            </p>

                            <p style=""color: #374151; font-size: 16px; line-height: 1.6; margin-bottom: 30px;"">
                                {HtmlEncode(request.FollowUpMessage)}
                            </p>

                            <!-- CTA Button -->
                            <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"">
                                <tr>
                                    <td align=""center"">
                                        <a href=""{HtmlEncode(request.ProposalViewUrl)}""
                                           style=""display: inline-block; background: linear-gradient(135deg, #10B981 0%, #059669 100%); color: #FFFFFF; padding: 16px 40px; text-decoration: none; border-radius: 8px; font-size: 16px; font-weight: 600; box-shadow: 0 4px 6px rgba(16, 185, 129, 0.3);"">
                                            View Proposal
                                        </a>
                                    </td>
                                </tr>
                            </table>

                            <p style=""color: #374151; font-size: 16px; line-height: 1.6; margin-top: 30px;"">
                                Looking forward to hearing from you!
                            </p>

                            <p style=""color: #374151; font-size: 16px; line-height: 1.6; margin-top: 20px;"">
                                Best regards,<br>
                                <strong>{HtmlEncode(request.SenderName)}</strong>
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #F9FAFB; padding: 30px; border-radius: 0 0 12px 12px; text-align: center; border-top: 1px solid #E5E7EB;"">
                            <p style=""color: #9CA3AF; font-size: 12px; margin: 0;"">
                                Sent via ProposalPilot | Reply directly to this email
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string GenerateFollowUpEmailPlainText(SendFollowUpEmailRequest request)
    {
        return $@"
FOLLOW-UP: {request.ProposalTitle}

Hi {request.RecipientName},

{request.FollowUpMessage}

View the proposal here:
{request.ProposalViewUrl}

Looking forward to hearing from you!

Best regards,
{request.SenderName}

---
Sent via ProposalPilot
";
    }

    #endregion

    #region Helpers

    private static string HtmlEncode(string? text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return System.Net.WebUtility.HtmlEncode(text);
    }

    private static string StripHtml(string html)
    {
        if (string.IsNullOrEmpty(html)) return "";
        return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", "");
    }

    #endregion
}
