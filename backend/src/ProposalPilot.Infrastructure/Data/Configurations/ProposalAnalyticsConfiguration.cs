using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProposalPilot.Domain.Entities;

namespace ProposalPilot.Infrastructure.Data.Configurations;

public class ProposalAnalyticsConfiguration : IEntityTypeConfiguration<ProposalAnalytics>
{
    public void Configure(EntityTypeBuilder<ProposalAnalytics> builder)
    {
        builder.ToTable("ProposalAnalytics");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ViewedAt)
            .IsRequired();

        builder.Property(a => a.ViewerIpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.ViewerUserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.ViewerCountry)
            .HasMaxLength(100);

        builder.Property(a => a.ViewerCity)
            .HasMaxLength(100);

        builder.Property(a => a.SectionId)
            .HasMaxLength(50);

        builder.Property(a => a.TimeSpentSeconds)
            .HasDefaultValue(0);

        builder.Property(a => a.ScrollDepth)
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(a => a.ProposalId);
        builder.HasIndex(a => a.ViewedAt);
        builder.HasIndex(a => new { a.ProposalId, a.ViewedAt });
    }
}
