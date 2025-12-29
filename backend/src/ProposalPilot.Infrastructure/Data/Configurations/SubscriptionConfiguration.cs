using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProposalPilot.Domain.Entities;

namespace ProposalPilot.Infrastructure.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Plan)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.MonthlyPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.AutoRenew)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.StripeSubscriptionId)
            .HasMaxLength(100);

        builder.Property(s => s.StripeCustomerId)
            .HasMaxLength(100);

        builder.Property(s => s.ProposalsPerMonth)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.ProposalsUsedThisMonth)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.UsageResetDate)
            .IsRequired();

        // Feature flags
        builder.Property(s => s.HasAIAnalysis)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.HasAdvancedTemplates)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.HasPrioritySupport)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.HasWhiteLabeling)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasMany(s => s.Payments)
            .WithOne(p => p.Subscription)
            .HasForeignKey(p => p.SubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.StripeSubscriptionId);
        builder.HasIndex(s => s.StripeCustomerId);
        builder.HasIndex(s => s.IsActive);
        builder.HasIndex(s => s.EndDate);
    }
}
