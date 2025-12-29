using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProposalPilot.Domain.Entities;

namespace ProposalPilot.Infrastructure.Data.Configurations;

public class ProposalSectionConfiguration : IEntityTypeConfiguration<ProposalSection>
{
    public void Configure(EntityTypeBuilder<ProposalSection> builder)
    {
        builder.ToTable("ProposalSections");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.DisplayOrder)
            .IsRequired();

        builder.Property(s => s.IsVisible)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(s => s.ProposalId);
        builder.HasIndex(s => new { s.ProposalId, s.DisplayOrder });
    }
}
