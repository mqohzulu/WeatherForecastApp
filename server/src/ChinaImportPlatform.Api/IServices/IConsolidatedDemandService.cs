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

    /// <summary>
    /// Tick an item off the buying list: marks the matching line sourced on every linked
    /// order and moves those orders to Being Sourced in one action (US-S02). Returns the
    /// affected orders.
    /// </summary>
    Task<IReadOnlyList<OrderDto>> MarkSourcedAsync(MarkDemandSourcedDto dto, CancellationToken ct = default);
}
