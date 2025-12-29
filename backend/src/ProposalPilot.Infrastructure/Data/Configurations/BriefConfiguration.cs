using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProposalPilot.Domain.Entities;

namespace ProposalPilot.Infrastructure.Data.Configurations;

public class BriefConfiguration : IEntityTypeConfiguration<Brief>
{
    public void Configure(EntityTypeBuilder<Brief> builder)
    {
        builder.ToTable("Briefs");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.RawContent)
            .IsRequired();

        builder.Property(b => b.AnalyzedContent)
            .HasMaxLength(5000);

        builder.Property(b => b.ProjectType)
            .HasMaxLength(100);

        builder.Property(b => b.Industry)
            .HasMaxLength(100);

        builder.Property(b => b.EstimatedBudget)
            .HasColumnType("decimal(18,2)");

        builder.Property(b => b.Timeline)
            .HasMaxLength(100);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(b => b.AnalysisCost)
            .HasColumnType("decimal(18,6)");

        // Relationships
        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Proposals)
            .WithOne(p => p.Brief)
            .HasForeignKey(p => p.BriefId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.CreatedAt);
    }
}
