using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfPaymentRequestRepository : EfRepository<PaymentRequest>, IPaymentRequestRepository
{
    public EfPaymentRequestRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<PaymentRequest>> GetByOrderAsync(Guid orderId, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);
}
