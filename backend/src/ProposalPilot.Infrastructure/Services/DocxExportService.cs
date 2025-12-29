using System.Text.Json;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Shared.DTOs.Proposal;

namespace ProposalPilot.Infrastructure.Services;

public interface IDocxExportService
{
    byte[] GenerateProposalDocx(Proposal proposal);
}

public class DocxExportService : IDocxExportService
{
    public byte[] GenerateProposalDocx(Proposal proposal)
    {
        using var memoryStream = new MemoryStream();
        using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
        {
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            // Title
            AddHeading(body, proposal.Title, 1);
            AddParagraph(body, $"Created: {proposal.CreatedAt:MMMM dd, yyyy}", true);
            AddParagraph(body, $"Status: {proposal.Status}", true);
            AddParagraph(body, ""); // Empty line

            // Content
            if (!string.IsNullOrEmpty(proposal.DeliverablesJson))
            {
                try
                {
                    var content = JsonSerializer.Deserialize<ProposalGenerationResult>(proposal.DeliverablesJson);
                    if (content != null)
                    {
                        RenderProposalContent(body, content);
                    }
                }
                catch
                {
                    AddParagraph(body, "Error rendering proposal content");
                }
            }

            mainPart.Document.Save();
        }

        return memoryStream.ToArray();
    }

    private void RenderProposalContent(Body body, ProposalGenerationResult content)
    {
        // Opening Hook
        if (!string.IsNullOrEmpty(content.Sections.OpeningHook))
        {
            AddHtmlContent(body, content.Sections.OpeningHook);
            AddParagraph(body, "");
        }

        // Problem Statement
        if (!string.IsNullOrEmpty(content.Sections.ProblemStatement))
        {
            AddHeading(body, "The Challenge", 2);
            AddHtmlContent(body, content.Sections.ProblemStatement);
            AddParagraph(body, "");
        }

        // Proposed Solution
        if (!string.IsNullOrEmpty(content.Sections.ProposedSolution))
        {
            AddHeading(body, "Our Solution", 2);
            AddHtmlContent(body, content.Sections.ProposedSolution);
            AddParagraph(body, "");
        }

        // Methodology
        if (!string.IsNullOrEmpty(content.Sections.Methodology))
        {
            AddHeading(body, "How We'll Do It", 2);
            AddHtmlContent(body, content.Sections.Methodology);
            AddParagraph(body, "");
        }

        // Timeline
        if (!string.IsNullOrEmpty(content.Sections.Timeline))
        {
            AddHeading(body, "Timeline", 2);
            AddHtmlContent(body, content.Sections.Timeline);
            AddParagraph(body, "");
        }

        // Investment
        if (content.Sections.Investment != null && content.Sections.Investment.Tiers.Count > 0)
        {
            AddHeading(body, "Investment Options", 2);
            if (!string.IsNullOrEmpty(content.Sections.Investment.Intro))
            {
                AddHtmlContent(body, content.Sections.Investment.Intro);
                AddParagraph(body, "");
            }

            foreach (var tier in content.Sections.Investment.Tiers)
            {
                AddHeading(body, tier.Name, 3);
                AddParagraph(body, tier.Price > 0 ? $"${tier.Price:N0}" : "TBD", true);
                AddParagraph(body, tier.Description);
                AddParagraph(body, "");

                foreach (var feature in tier.Features)
                {
                    AddParagraph(body, $"✓ {feature}");
                }

                AddParagraph(body, $"Timeline: {tier.Timeline}", true);
                AddParagraph(body, "");
            }
        }

        // Why Choose Us
        if (!string.IsNullOrEmpty(content.Sections.WhyChooseUs))
        {
            AddHeading(body, "Why Choose Us", 2);
            AddHtmlContent(body, content.Sections.WhyChooseUs);
            AddParagraph(body, "");
        }

        // Next Steps
        if (!string.IsNullOrEmpty(content.Sections.NextSteps))
        {
            AddHeading(body, "Next Steps", 2);
            AddHtmlContent(body, content.Sections.NextSteps);
        }
    }

    private void AddHeading(Body body, string text, int level)
    {
        var paragraph = body.AppendChild(new Paragraph());
        var run = paragraph.AppendChild(new Run());

        var runProperties = run.AppendChild(new RunProperties());
        runProperties.AppendChild(new Bold());

        var fontSize = level == 1 ? "32" : level == 2 ? "24" : "20";
        runProperties.AppendChild(new FontSize { Val = fontSize });

        if (level == 1 || level == 2)
        {
            runProperties.AppendChild(new Color { Val = "1F4E78" }); // Blue color
        }

        run.AppendChild(new Text(text));

        // Add spacing after
        var paragraphProperties = paragraph.InsertAt(new ParagraphProperties(), 0);
        paragraphProperties.AppendChild(new SpacingBetweenLines { After = "200" });
    }

    private void AddParagraph(Body body, string text, bool italic = false)
    {
        var paragraph = body.AppendChild(new Paragraph());
        var run = paragraph.AppendChild(new Run());

        if (italic)
        {
            var runProperties = run.AppendChild(new RunProperties());
            runProperties.AppendChild(new Italic());
            runProperties.AppendChild(new Color { Val = "666666" });
        }

        run.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });

        // Add line spacing
        var paragraphProperties = paragraph.InsertAt(new ParagraphProperties(), 0);
        paragraphProperties.AppendChild(new SpacingBetweenLines { Line = "276", LineRule = LineSpacingRuleValues.Auto });
    }

    private void AddHtmlContent(Body body, string htmlContent)
    {
        // Remove HTML tags (simplified)
        var cleanContent = System.Text.RegularExpressions.Regex.Replace(htmlContent, "<[^>]*>", "");
        cleanContent = cleanContent.Replace("&nbsp;", " ");
        cleanContent = cleanContent.Replace("&amp;", "&");
        cleanContent = cleanContent.Replace("&lt;", "<");
        cleanContent = cleanContent.Replace("&gt;", ">");
        cleanContent = cleanContent.Trim();

        if (!string.IsNullOrEmpty(cleanContent))
        {
            // Split by line breaks and create paragraphs
            var lines = cleanContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    AddParagraph(body, line.Trim());
                }
            }
        }
    }
}
