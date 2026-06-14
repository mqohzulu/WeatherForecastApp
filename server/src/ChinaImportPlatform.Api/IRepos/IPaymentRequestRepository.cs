using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.IRepos;

public interface IPaymentRequestRepository : IRepository<PaymentRequest>
{
    Task<IReadOnlyList<PaymentRequest>> GetByOrderAsync(Guid orderId, CancellationToken ct = default);
}
