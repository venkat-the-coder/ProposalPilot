using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProposalPilot.Domain.Entities;

namespace ProposalPilot.Infrastructure.Data.Configurations;

public class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
{
    public void Configure(EntityTypeBuilder<Proposal> builder)
    {
        builder.ToTable("Proposals");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.OriginalBrief)
            .HasMaxLength(5000);

        builder.Property(p => p.BriefAnalysis)
            .HasMaxLength(5000);

        // JSON columns for pricing tiers
        builder.Property(p => p.BasicTierJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.StandardTierJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.PremiumTierJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        // JSON columns for deliverables and timeline
        builder.Property(p => p.DeliverablesJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.TimelineJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        // Terms
        builder.Property(p => p.TermsAndConditions)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.PaymentTerms)
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.ShareToken)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.ShareToken)
            .IsUnique();

        builder.Property(p => p.ViewCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.IsPublic)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasMany(p => p.Sections)
            .WithOne(s => s.Proposal)
            .HasForeignKey(s => s.ProposalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Analytics)
            .WithOne(a => a.Proposal)
            .HasForeignKey(a => a.ProposalId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.ClientId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.SentAt);
        builder.HasIndex(p => p.ExpiresAt);
    }
}
