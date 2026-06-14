using ChinaImportPlatform.Api.Enums;

namespace ChinaImportPlatform.Api.Models;

/// <summary>
/// A structured payment request against an order. Kept separate from <see cref="Payment"/>
/// so a future gateway integration is cheap (each payment carries a gateway reference).
/// </summary>
public class PaymentRequest : BaseEntity
{
    public Guid OrderId { get; set; }

    /// <summary>Amount due in ZAR cents.</summary>
    public long AmountCents { get; set; }

    public DateTime DueDate { get; set; }

    /// <summary>Reference shown to the customer; typically the order number.</summary>
    public string Reference { get; set; } = string.Empty;

    public string? Instructions { get; set; }

    public PaymentRequestStatus Status { get; set; } = PaymentRequestStatus.Pending;

    public List<Payment> Payments { get; set; } = new();
}

/// <summary>A payment recorded against a request (manual proof first, gateway later).</summary>
public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public long AmountCents { get; set; }

    /// <summary>e.g. "EFT", "Card", "InstantEFT".</summary>
    public string Method { get; set; } = "EFT";

    public string? ProofS3Key { get; set; }

    /// <summary>Populated by a payment gateway in a later phase.</summary>
    public string? GatewayReference { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
