using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Enums;

namespace ChinaImportPlatform.Api.IServices;

public interface IConsolidatedDemandService
{
    /// <summary>
    /// Aggregates order lines by product and variant for a trip window so the seller
    /// has an exact buying list for China (US-S02).
    /// </summary>
    Task<IReadOnlyList<ConsolidatedDemandLineDto>> GetAsync(
        Guid? tripId,
        OrderStatus? minStatus,
        Guid? categoryId,
        CancellationToken ct = default);
}
