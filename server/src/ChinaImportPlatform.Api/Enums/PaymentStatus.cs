namespace ChinaImportPlatform.Api.Enums;

/// <summary>Tracks how much of an order has been paid.</summary>
public enum PaymentStatus
{
    Unpaid = 0,
    DepositPaid = 1,
    PaidInFull = 2,
    Refunded = 3
}
