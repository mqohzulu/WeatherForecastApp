using ChinaImportPlatform.Api.Dtos;

namespace ChinaImportPlatform.Api.IServices;

public interface IPaymentService
{
    Task<IReadOnlyList<PaymentRequestDto>> GetByOrderAsync(Guid orderId, CancellationToken ct = default);

    Task<PaymentRequestDto> CreateRequestAsync(CreatePaymentRequestDto dto, CancellationToken ct = default);

    Task<PaymentRequestDto> UploadProofAsync(Guid paymentRequestId, UploadProofDto dto, CancellationToken ct = default);

    Task<PaymentRequestDto> ApproveAsync(Guid paymentRequestId, Guid paymentId, CancellationToken ct = default);
}
