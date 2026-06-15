using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Enums;

namespace ChinaImportPlatform.Api.IServices;

public interface IOrderService
{
    Task<PagedResult<OrderDto>> GetQueueAsync(int page, int pageSize, string? sort, OrderStatus? status, CancellationToken ct = default);

    /// <summary>Count of new (Placed) orders for the seller's queue badge (US-S01).</summary>
    Task<int> CountNewAsync(CancellationToken ct = default);

    Task<IReadOnlyList<OrderDto>> GetByUserAsync(Guid userId, CancellationToken ct = default);

    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<OrderDto> PlaceOrderAsync(CreateOrderDto dto, CancellationToken ct = default);

    Task<OrderDto> UpdateStatusAsync(Guid orderId, UpdateOrderStatusDto dto, CancellationToken ct = default);

    Task<IReadOnlyList<OrderDto>> BulkUpdateStatusAsync(BulkUpdateOrderStatusDto dto, CancellationToken ct = default);

    Task<OrderDto> SetPricingAsync(Guid orderId, SetOrderPricingDto dto, CancellationToken ct = default);

    /// <summary>Reject a single line with a reason shown to the customer (US-S04).</summary>
    Task<OrderDto> RejectLineAsync(Guid orderId, RejectLineDto dto, CancellationToken ct = default);

    /// <summary>Mark an order Ready for Collection with address + time window (US-S07).</summary>
    Task<OrderDto> MarkReadyForCollectionAsync(Guid orderId, MarkReadyForCollectionDto dto, CancellationToken ct = default);

    Task<OrderDto> CancelAsync(Guid orderId, Guid requestedBy, CancellationToken ct = default);
}
