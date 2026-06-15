namespace ChinaImportPlatform.Api.Enums;

/// <summary>Status of a structured payment request sent to a customer.</summary>
public enum PaymentRequestStatus
{
    Pending = 0,
    ProofUploaded = 1,
    Approved = 2,
    Queried = 3,
    Cancelled = 4
}
