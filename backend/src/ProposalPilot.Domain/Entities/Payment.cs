using ProposalPilot.Domain.Enums;

namespace ProposalPilot.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid SubscriptionId { get; set; }
    public Subscription Subscription { get; set; } = null!;

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? StripePaymentIntentId { get; set; }
    public string? StripeInvoiceId { get; set; }

    public DateTime? PaidAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? FailureReason { get; set; }

    public string? ReceiptUrl { get; set; }
    public string? InvoiceUrl { get; set; }
}
