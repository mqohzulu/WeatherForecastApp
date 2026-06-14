using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;

namespace ChinaImportPlatform.Api.IServices;

public interface IOrderService
{
    Task<PagedResult<OrderDto>> GetQueueAsync(int page, int pageSize, CancellationToken ct = default);

    Task<IReadOnlyList<OrderDto>> GetByUserAsync(Guid userId, CancellationToken ct = default);

    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<OrderDto> PlaceOrderAsync(CreateOrderDto dto, CancellationToken ct = default);

    Task<OrderDto> UpdateStatusAsync(Guid orderId, UpdateOrderStatusDto dto, CancellationToken ct = default);

    Task<IReadOnlyList<OrderDto>> BulkUpdateStatusAsync(BulkUpdateOrderStatusDto dto, CancellationToken ct = default);

    Task<OrderDto> SetPricingAsync(Guid orderId, SetOrderPricingDto dto, CancellationToken ct = default);

    Task<OrderDto> CancelAsync(Guid orderId, Guid requestedBy, CancellationToken ct = default);
}
