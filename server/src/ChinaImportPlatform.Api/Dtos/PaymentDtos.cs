using ChinaImportPlatform.Api.Enums;

namespace ChinaImportPlatform.Api.Dtos;

public record PaymentDto
{
    public Guid Id { get; init; }
    public long AmountCents { get; init; }
    public string Method { get; init; } = "EFT";
    public string? ProofS3Key { get; init; }
    public string? GatewayReference { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record PaymentRequestDto
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public long AmountCents { get; init; }
    public DateTime DueDate { get; init; }
    public string Reference { get; init; } = string.Empty;
    public string? Instructions { get; init; }
    public PaymentRequestStatus Status { get; init; }
    public IReadOnlyList<PaymentDto> Payments { get; init; } = Array.Empty<PaymentDto>();
}

public record CreatePaymentRequestDto
{
    public Guid OrderId { get; init; }
    public long AmountCents { get; init; }
    public DateTime DueDate { get; init; }
    public string? Instructions { get; init; }
}

/// <summary>Customer uploads proof of an EFT against a payment request.</summary>
public record UploadProofDto
{
    public long AmountCents { get; init; }
    public string Method { get; init; } = "EFT";
    public string ProofS3Key { get; init; } = string.Empty;
}
