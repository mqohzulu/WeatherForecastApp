using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/payment-requests")]
[Produces("application/json")]
public class PaymentRequestsController : ControllerBase
{
    private readonly IPaymentService _payments;

    public PaymentRequestsController(IPaymentService payments)
    {
        _payments = payments;
    }

    [HttpGet("by-order/{orderId:guid}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PaymentRequestDto>>>> GetByOrder(Guid orderId, CancellationToken ct)
    {
        var result = await _payments.GetByOrderAsync(orderId, ct);
        return Ok(ApiResponse<IReadOnlyList<PaymentRequestDto>>.Ok(result));
    }

    /// <summary>Seller sends a structured payment request against an order (US-S10).</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PaymentRequestDto>>> Create([FromBody] CreatePaymentRequestDto dto, CancellationToken ct)
    {
        var created = await _payments.CreateRequestAsync(dto, ct);
        return Ok(ApiResponse<PaymentRequestDto>.Ok(created));
    }

    /// <summary>Customer uploads proof of payment (US-C18).</summary>
    [HttpPost("{id:guid}/proof")]
    public async Task<ActionResult<ApiResponse<PaymentRequestDto>>> UploadProof(Guid id, [FromBody] UploadProofDto dto, CancellationToken ct)
    {
        var result = await _payments.UploadProofAsync(id, dto, ct);
        return Ok(ApiResponse<PaymentRequestDto>.Ok(result));
    }

    /// <summary>Seller approves a proof of payment (US-S10).</summary>
    [HttpPost("{id:guid}/payments/{paymentId:guid}/approve")]
    public async Task<ActionResult<ApiResponse<PaymentRequestDto>>> Approve(Guid id, Guid paymentId, CancellationToken ct)
    {
        var result = await _payments.ApproveAsync(id, paymentId, ct);
        return Ok(ApiResponse<PaymentRequestDto>.Ok(result, "Payment approved."));
    }
}
