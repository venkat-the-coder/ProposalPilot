using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Shared.DTOs.Proposal;

namespace ProposalPilot.Infrastructure.Services;

public interface IPdfExportService
{
    byte[] GenerateProposalPdf(Proposal proposal);
}

public class PdfExportService : IPdfExportService
{
    public PdfExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateProposalPdf(Proposal proposal)
    {
        // Store uppercase status to avoid span-related compilation issues
        var statusString = proposal.Status.ToString();
        var statusText = new string(statusString.ToCharArray()).ToUpperInvariant();

#pragma warning disable CS0618 // Type or member is obsolete
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(RenderHeader);
                page.Content().Element(c => RenderContent(c, proposal));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });

                void RenderHeader(IContainer headerContainer)
                {
                    headerContainer.BorderBottom(1).BorderColor(Colors.Blue.Darken2).Padding(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(proposal.Title).FontSize(20).Bold().FontColor(Colors.Blue.Darken3);
                            col.Item().Text($"Created: {proposal.CreatedAt:MMMM dd, yyyy}").FontSize(9).FontColor(Colors.Grey.Darken2);
                        });
                        row.ConstantItem(100).AlignRight().Text(statusText).FontSize(9).Bold().FontColor(Colors.Green.Darken1);
                    });
                }
            });
        });
#pragma warning restore CS0618 // Type or member is obsolete

        return document.GeneratePdf();
    }

    private void RenderContent(IContainer container, Proposal proposal)
    {
        container.PaddingVertical(1, Unit.Centimetre).Column(column =>
        {
            if (!string.IsNullOrEmpty(proposal.DeliverablesJson))
            {
                try
                {
                    var content = JsonSerializer.Deserialize<ProposalGenerationResult>(proposal.DeliverablesJson);
                    if (content != null)
                    {
                        RenderProposalSections(column, content);
                    }
                }
                catch
                {
                    column.Item().Text("Error rendering proposal content").FontColor(Colors.Red.Medium);
                }
            }
        });
    }

    private void RenderProposalSections(ColumnDescriptor column, ProposalGenerationResult content)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        if (!string.IsNullOrEmpty(content.Sections.OpeningHook))
        {
            column.Item().PaddingTop(15).Text(CleanHtml(content.Sections.OpeningHook)).LineHeight(1.5f);
        }

        if (!string.IsNullOrEmpty(content.Sections.ProblemStatement))
        {
            RenderSection(column, "The Challenge", content.Sections.ProblemStatement);
        }

        if (!string.IsNullOrEmpty(content.Sections.ProposedSolution))
        {
            RenderSection(column, "Our Solution", content.Sections.ProposedSolution);
        }

        if (!string.IsNullOrEmpty(content.Sections.Methodology))
        {
            RenderSection(column, "How We'll Do It", content.Sections.Methodology);
        }

        if (!string.IsNullOrEmpty(content.Sections.Timeline))
        {
            RenderSection(column, "Timeline", content.Sections.Timeline);
        }

        if (content.Sections.Investment != null && content.Sections.Investment.Tiers.Count > 0)
        {
            RenderInvestment(column, content.Sections.Investment);
        }

        if (!string.IsNullOrEmpty(content.Sections.WhyChooseUs))
        {
            RenderSection(column, "Why Choose Us", content.Sections.WhyChooseUs);
        }

        if (!string.IsNullOrEmpty(content.Sections.NextSteps))
        {
            RenderSection(column, "Next Steps", content.Sections.NextSteps);
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }

    private void RenderSection(ColumnDescriptor column, string title, string htmlContent)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        column.Item().PaddingTop(15).Column(sectionColumn =>
        {
            sectionColumn.Item().PaddingBottom(8).Text(title).FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
            sectionColumn.Item().Text(CleanHtml(htmlContent)).LineHeight(1.5f);
        });
#pragma warning restore CS0618 // Type or member is obsolete
    }

    private void RenderInvestment(ColumnDescriptor column, InvestmentSection investment)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        column.Item().PaddingTop(15).Column(investmentColumn =>
        {
            investmentColumn.Item().PaddingBottom(8).Text("Investment Options").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);

            if (!string.IsNullOrEmpty(investment.Intro))
            {
                investmentColumn.Item().PaddingBottom(12).Text(CleanHtml(investment.Intro));
            }

            investmentColumn.Item().Row(row =>
            {
                foreach (var tier in investment.Tiers)
                {
                    row.RelativeItem().Padding(5).Border(tier.Highlighted ? 2 : 1)
                        .BorderColor(tier.Highlighted ? Colors.Blue.Medium : Colors.Grey.Lighten1)
                        .Background(tier.Highlighted ? Colors.Blue.Lighten4 : Colors.White)
                        .Padding(12)
                        .Column(tierColumn =>
                        {
                            if (tier.Highlighted)
                            {
                                tierColumn.Item().PaddingBottom(4).Text("RECOMMENDED").FontSize(8).Bold().FontColor(Colors.Blue.Darken2);
                            }

                            tierColumn.Item().PaddingBottom(4).Text(tier.Name).FontSize(14).Bold();
                            tierColumn.Item().PaddingBottom(4).Text(tier.Price > 0 ? $"${tier.Price:N0}" : "TBD").FontSize(18).Bold().FontColor(Colors.Blue.Darken3);
                            tierColumn.Item().PaddingBottom(8).Text(tier.Description).FontSize(9).FontColor(Colors.Grey.Darken2);

                            foreach (var feature in tier.Features)
                            {
                                tierColumn.Item().Row(featureRow =>
                                {
                                    featureRow.ConstantItem(12).Text("✓").FontColor(Colors.Green.Medium).FontSize(10);
                                    featureRow.RelativeItem().Text(feature).FontSize(9).LineHeight(1.3f);
                                });
                            }

                            tierColumn.Item().PaddingTop(8).Text(tier.Timeline).FontSize(9).FontColor(Colors.Grey.Darken1);
                        });
                }
            });
        });
#pragma warning restore CS0618 // Type or member is obsolete
    }

    private string CleanHtml(string html)
    {
        var clean = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", "");
        clean = clean.Replace("&nbsp;", " ");
        clean = clean.Replace("&amp;", "&");
        clean = clean.Replace("&lt;", "<");
        clean = clean.Replace("&gt;", ">");
        return clean.Trim();
    }
}
