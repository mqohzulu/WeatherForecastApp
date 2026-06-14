using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class PaymentRequestRepository : JsonRepository<PaymentRequest>, IPaymentRequestRepository
{
    public PaymentRequestRepository(JsonStoreOptions options) : base(options, "payment-requests.json") { }

    public async Task<IReadOnlyList<PaymentRequest>> GetByOrderAsync(Guid orderId, CancellationToken ct = default) =>
        (await FindAsync(p => p.OrderId == orderId, ct))
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
}
