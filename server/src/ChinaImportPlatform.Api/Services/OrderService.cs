using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Enums;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;
    private readonly INotificationService _notifications;

    public OrderService(IOrderRepository orders, IProductRepository products, INotificationService notifications)
    {
        _orders = orders;
        _products = products;
        _notifications = notifications;
    }

    public async Task<PagedResult<OrderDto>> GetQueueAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var orders = (await _orders.GetAllAsync(ct))
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => o.ToDto())
            .ToList();

        return PagedResult<OrderDto>.Create(orders, page, pageSize);
    }

    public async Task<IReadOnlyList<OrderDto>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        (await _orders.GetByUserAsync(userId, ct)).Select(o => o.ToDto()).ToList();

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(id, ct);
        return order?.ToDto();
    }

    public async Task<OrderDto> PlaceOrderAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        if (dto.Items.Count == 0)
        {
            throw new AppException("An order must contain at least one item.");
        }

        var order = new Order
        {
            OrderNumber = await GenerateOrderNumberAsync(ct),
            UserId = dto.UserId,
            TripId = dto.TripId,
            PaymentStatus = PaymentStatus.Unpaid
        };

        var hasCustomItem = false;
        long indicativeTotal = 0;

        foreach (var line in dto.Items)
        {
            if (line.Quantity is < 1 or > 999)
            {
                throw new AppException("Item quantity must be between 1 and 999.");
            }

            var item = new OrderItem
            {
                ProductId = line.ProductId,
                VariantId = line.VariantId,
                CustomDescription = line.CustomDescription,
                Quantity = line.Quantity,
                Note = line.Note,
                LineStatus = LineStatus.Pending
            };

            if (line.ProductId.HasValue)
            {
                var product = await _products.GetByIdAsync(line.ProductId.Value, ct)
                              ?? throw new NotFoundException($"Product {line.ProductId} not found.");

                item.ProductNameSnapshot = product.Name;

                var unit = product.IndicativePriceCents;
                if (line.VariantId.HasValue)
                {
                    var variant = product.Variants.FirstOrDefault(v => v.Id == line.VariantId.Value);
                    if (variant != null)
                    {
                        unit += variant.PriceAdjustmentCents;
                    }
                }

                indicativeTotal += unit * line.Quantity;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(line.CustomDescription) || line.CustomDescription.Trim().Length < 20)
                {
                    throw new AppException("Custom orders require a description of at least 20 characters.");
                }

                hasCustomItem = true;
            }

            order.Items.Add(item);
        }

        order.TotalIndicativeCents = indicativeTotal;
        order.Status = hasCustomItem ? OrderStatus.AwaitingQuote : OrderStatus.Placed;
        order.StatusHistory.Add(new OrderStatusHistoryEntry
        {
            Status = order.Status,
            Note = "Order submitted.",
            CreatedBy = dto.UserId
        });

        await _orders.AddAsync(order, ct);

        await _notifications.NotifyUserAsync(
            order.UserId,
            "Order received",
            $"Your order {order.OrderNumber} has been received.",
            new Dictionary<string, string> { ["orderId"] = order.Id.ToString() },
            ct);

        return order.ToDto();
    }

    public async Task<OrderDto> UpdateStatusAsync(Guid orderId, UpdateOrderStatusDto dto, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(orderId, ct)
                    ?? throw new NotFoundException($"Order {orderId} not found.");

        ApplyStatus(order, dto.Status, dto.Note, dto.PhotoS3Key, dto.UpdatedBy);
        await _orders.UpdateAsync(order, ct);

        await _notifications.NotifyUserAsync(
            order.UserId,
            "Order update",
            $"Order {order.OrderNumber} is now {dto.Status}." + (dto.Note is null ? string.Empty : $" {dto.Note}"),
            new Dictionary<string, string> { ["orderId"] = order.Id.ToString() },
            ct);

        return order.ToDto();
    }

    public async Task<IReadOnlyList<OrderDto>> BulkUpdateStatusAsync(BulkUpdateOrderStatusDto dto, CancellationToken ct = default)
    {
        var results = new List<OrderDto>();

        foreach (var orderId in dto.OrderIds.Distinct())
        {
            var order = await _orders.GetByIdAsync(orderId, ct);
            if (order is null)
            {
                continue;
            }

            ApplyStatus(order, dto.Status, dto.Note, null, dto.UpdatedBy);
            await _orders.UpdateAsync(order, ct);

            // Each affected customer receives an individual notification referencing their own order (US-S06).
            await _notifications.NotifyUserAsync(
                order.UserId,
                "Order update",
                $"Order {order.OrderNumber} is now {dto.Status}." + (dto.Note is null ? string.Empty : $" {dto.Note}"),
                new Dictionary<string, string> { ["orderId"] = order.Id.ToString() },
                ct);

            results.Add(order.ToDto());
        }

        return results;
    }

    public async Task<OrderDto> SetPricingAsync(Guid orderId, SetOrderPricingDto dto, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(orderId, ct)
                    ?? throw new NotFoundException($"Order {orderId} not found.");

        foreach (var line in dto.Lines)
        {
            var item = order.Items.FirstOrDefault(i => i.Id == line.OrderItemId);
            if (item is null)
            {
                continue;
            }

            item.UnitPriceFinalCents = line.UnitPriceFinalCents;
            item.LineStatus = LineStatus.Quoted;
        }

        order.TotalFinalCents = order.Items
            .Where(i => i.UnitPriceFinalCents.HasValue)
            .Sum(i => i.UnitPriceFinalCents!.Value * i.Quantity);

        await _orders.UpdateAsync(order, ct);

        await _notifications.NotifyUserAsync(
            order.UserId,
            "Price ready",
            $"A final price is ready for order {order.OrderNumber}. Please accept or decline.",
            new Dictionary<string, string> { ["orderId"] = order.Id.ToString() },
            ct);

        return order.ToDto();
    }

    public async Task<OrderDto> CancelAsync(Guid orderId, Guid requestedBy, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(orderId, ct)
                    ?? throw new NotFoundException($"Order {orderId} not found.");

        if (order.Status != OrderStatus.Placed && order.Status != OrderStatus.AwaitingQuote)
        {
            throw new AppException("Only orders that are still Placed can be cancelled directly.");
        }

        ApplyStatus(order, OrderStatus.Cancelled, "Cancelled by customer.", null, requestedBy);
        await _orders.UpdateAsync(order, ct);
        return order.ToDto();
    }

    private static void ApplyStatus(Order order, OrderStatus status, string? note, string? photoS3Key, Guid updatedBy)
    {
        order.Status = status;
        order.StatusHistory.Add(new OrderStatusHistoryEntry
        {
            Status = status,
            Note = note,
            PhotoS3Key = photoS3Key,
            CreatedBy = updatedBy
        });
    }

    private async Task<string> GenerateOrderNumberAsync(CancellationToken ct)
    {
        var count = await _orders.CountAsync(ct);
        return $"ORD-{DateTime.UtcNow:yyyy}-{(count + 1):D4}";
    }
}
