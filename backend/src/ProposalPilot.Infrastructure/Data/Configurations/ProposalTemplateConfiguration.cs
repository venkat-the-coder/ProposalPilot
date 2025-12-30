using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProposalPilot.Domain.Entities;

namespace ProposalPilot.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ProposalTemplate entity
/// </summary>
public class ProposalTemplateConfiguration : IEntityTypeConfiguration<ProposalTemplate>
{
    public void Configure(EntityTypeBuilder<ProposalTemplate> builder)
    {
        builder.ToTable("ProposalTemplates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Tags)
            .HasColumnType("nvarchar(max)"); // JSON array of tags

        builder.Property(t => t.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)"); // JSON content structure

        builder.Property(t => t.DefaultPricing)
            .HasColumnType("nvarchar(max)"); // JSON pricing structure

        builder.Property(t => t.IsSystemTemplate)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsPublic)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.UsageCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.WinRate)
            .HasColumnType("decimal(5,2)"); // 0-100 with 2 decimal places

        builder.Property(t => t.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(t => t.EstimatedTimeMinutes);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes for performance
        builder.HasIndex(t => t.Category)
            .HasDatabaseName("IX_ProposalTemplates_Category");

        builder.HasIndex(t => t.IsSystemTemplate)
            .HasDatabaseName("IX_ProposalTemplates_IsSystemTemplate");

        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("IX_ProposalTemplates_UserId");

        builder.HasIndex(t => t.IsPublic)
            .HasDatabaseName("IX_ProposalTemplates_IsPublic");

        builder.HasIndex(t => new { t.IsDeleted, t.IsSystemTemplate, t.UserId })
            .HasDatabaseName("IX_ProposalTemplates_Composite")
            .HasFilter("[IsDeleted] = 0"); // Filtered index for active templates

        // Relationships
        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict) // Don't cascade delete
            .IsRequired(false);

        builder.HasMany(t => t.Proposals)
            .WithOne(p => p.Template)
            .HasForeignKey(p => p.TemplateId)
            .OnDelete(DeleteBehavior.SetNull); // Set null when template deleted

        // Query filter for soft delete
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
