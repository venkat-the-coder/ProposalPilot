using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProposalPilot.Domain.Entities;

namespace ProposalPilot.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.StripePaymentIntentId)
            .HasMaxLength(100);

        builder.Property(p => p.StripeInvoiceId)
            .HasMaxLength(100);

        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);

        builder.Property(p => p.ReceiptUrl)
            .HasMaxLength(500);

        builder.Property(p => p.InvoiceUrl)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(p => p.SubscriptionId);
        builder.HasIndex(p => p.StripePaymentIntentId);
        builder.HasIndex(p => p.StripeInvoiceId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.PaidAt);
        builder.HasIndex(p => p.CreatedAt);
    }
}
