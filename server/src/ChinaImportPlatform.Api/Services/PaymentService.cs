using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Enums;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRequestRepository _paymentRequests;
    private readonly IOrderRepository _orders;
    private readonly INotificationService _notifications;

    public PaymentService(
        IPaymentRequestRepository paymentRequests,
        IOrderRepository orders,
        INotificationService notifications)
    {
        _paymentRequests = paymentRequests;
        _orders = orders;
        _notifications = notifications;
    }

    public async Task<IReadOnlyList<PaymentRequestDto>> GetByOrderAsync(Guid orderId, CancellationToken ct = default) =>
        (await _paymentRequests.GetByOrderAsync(orderId, ct)).Select(p => p.ToDto()).ToList();

    public async Task<PaymentRequestDto> CreateRequestAsync(CreatePaymentRequestDto dto, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(dto.OrderId, ct)
                    ?? throw new NotFoundException($"Order {dto.OrderId} not found.");

        var request = new PaymentRequest
        {
            OrderId = order.Id,
            AmountCents = dto.AmountCents,
            DueDate = dto.DueDate,
            Reference = order.OrderNumber,
            Instructions = dto.Instructions,
            Status = PaymentRequestStatus.Pending
        };

        await _paymentRequests.AddAsync(request, ct);

        await _notifications.NotifyUserAsync(
            order.UserId,
            "Payment requested",
            $"A payment of R {dto.AmountCents / 100m:0.00} is due for {order.OrderNumber} by {dto.DueDate:dd MMM yyyy}.",
            new Dictionary<string, string> { ["orderId"] = order.Id.ToString() },
            ct);

        return request.ToDto();
    }

    public async Task<PaymentRequestDto> UploadProofAsync(Guid paymentRequestId, UploadProofDto dto, CancellationToken ct = default)
    {
        var request = await _paymentRequests.GetByIdAsync(paymentRequestId, ct)
                      ?? throw new NotFoundException($"Payment request {paymentRequestId} not found.");

        request.Payments.Add(new Payment
        {
            AmountCents = dto.AmountCents,
            Method = dto.Method,
            ProofS3Key = dto.ProofS3Key
        });
        request.Status = PaymentRequestStatus.ProofUploaded;

        await _paymentRequests.UpdateAsync(request, ct);

        var order = await _orders.GetByIdAsync(request.OrderId, ct);
        if (order is not null)
        {
            await _notifications.NotifyUserAsync(
                order.UserId,
                "Proof received",
                $"Your proof of payment for {order.OrderNumber} was received and is awaiting approval.",
                null,
                ct);
        }

        return request.ToDto();
    }

    public async Task<PaymentRequestDto> ApproveAsync(Guid paymentRequestId, Guid paymentId, CancellationToken ct = default)
    {
        var request = await _paymentRequests.GetByIdAsync(paymentRequestId, ct)
                      ?? throw new NotFoundException($"Payment request {paymentRequestId} not found.");

        var payment = request.Payments.FirstOrDefault(p => p.Id == paymentId)
                      ?? throw new NotFoundException($"Payment {paymentId} not found.");

        payment.ApprovedAt = DateTime.UtcNow;
        request.Status = PaymentRequestStatus.Approved;
        await _paymentRequests.UpdateAsync(request, ct);

        // Recompute the order's payment status from approved payments against the request total.
        var order = await _orders.GetByIdAsync(request.OrderId, ct);
        if (order is not null)
        {
            var approved = request.Payments.Where(p => p.ApprovedAt != null).Sum(p => p.AmountCents);
            order.PaymentStatus = approved >= request.AmountCents
                ? PaymentStatus.PaidInFull
                : PaymentStatus.DepositPaid;
            await _orders.UpdateAsync(order, ct);
        }

        return request.ToDto();
    }
}
