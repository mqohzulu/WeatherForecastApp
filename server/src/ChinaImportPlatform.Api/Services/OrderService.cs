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
    private readonly IUserRepository _users;
    private readonly IPaymentRequestRepository _paymentRequests;
    private readonly INotificationService _notifications;

    public OrderService(
        IOrderRepository orders,
        IProductRepository products,
        IUserRepository users,
        IPaymentRequestRepository paymentRequests,
        INotificationService notifications)
    {
        _orders = orders;
        _products = products;
        _users = users;
        _paymentRequests = paymentRequests;
        _notifications = notifications;
    }

    public async Task<PagedResult<OrderDto>> GetQueueAsync(int page, int pageSize, string? sort, OrderStatus? status, CancellationToken ct = default)
    {
        var orders = (IEnumerable<Order>)await _orders.GetAllAsync(ct);

        if (status.HasValue)
        {
            orders = orders.Where(o => o.Status == status.Value);
        }

        var users = (await _users.GetAllAsync(ct)).ToDictionary(u => u.Id);

        orders = sort switch
        {
            "value" => orders.OrderByDescending(o => o.TotalFinalCents ?? o.TotalIndicativeCents),
            "status" => orders.OrderBy(o => o.Status),
            "customer" => orders.OrderBy(o => users.TryGetValue(o.UserId, out var u) ? u.FullName : string.Empty),
            _ => orders.OrderByDescending(o => o.CreatedAt)
        };

        var dtos = orders
            .Select(o => o.ToDto() with
            {
                CustomerName = users.TryGetValue(o.UserId, out var u) ? u.FullName : null
            })
            .ToList();

        return PagedResult<OrderDto>.Create(dtos, page, pageSize);
    }

    public async Task<int> CountNewAsync(CancellationToken ct = default) =>
        (await _orders.FindAsync(o => o.Status == OrderStatus.Placed, ct)).Count;

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

        await NotifyStatusAsync(order, dto.Note, ct);
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
            await NotifyStatusAsync(order, dto.Note, ct);
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
            .Where(i => i.UnitPriceFinalCents.HasValue && i.LineStatus != LineStatus.Rejected)
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

    public async Task<OrderDto> RejectLineAsync(Guid orderId, RejectLineDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            throw new AppException("A rejection reason is required.");
        }

        var order = await _orders.GetByIdAsync(orderId, ct)
                    ?? throw new NotFoundException($"Order {orderId} not found.");

        var item = order.Items.FirstOrDefault(i => i.Id == dto.OrderItemId)
                   ?? throw new NotFoundException($"Order item {dto.OrderItemId} not found.");

        item.LineStatus = LineStatus.Rejected;
        item.RejectionReason = dto.Reason;

        order.StatusHistory.Add(new OrderStatusHistoryEntry
        {
            Status = order.Status,
            Note = $"Line rejected: {dto.Reason}",
            CreatedBy = dto.UpdatedBy
        });

        await _orders.UpdateAsync(order, ct);

        await _notifications.NotifyUserAsync(
            order.UserId,
            "Item unavailable",
            $"An item on order {order.OrderNumber} could not be sourced: {dto.Reason}",
            new Dictionary<string, string> { ["orderId"] = order.Id.ToString() },
            ct);

        return order.ToDto();
    }

    public async Task<OrderDto> MarkReadyForCollectionAsync(Guid orderId, MarkReadyForCollectionDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.CollectionAddress))
        {
            throw new AppException("A collection address is required.");
        }

        var order = await _orders.GetByIdAsync(orderId, ct)
                    ?? throw new NotFoundException($"Order {orderId} not found.");

        order.CollectionAddress = dto.CollectionAddress;
        order.CollectionWindowStart = dto.WindowStart;
        order.CollectionWindowEnd = dto.WindowEnd;
        ApplyStatus(order, OrderStatus.ReadyForCollection, dto.Note, null, dto.UpdatedBy);
        await _orders.UpdateAsync(order, ct);

        var outstanding = await CalculateOutstandingAsync(order, ct);
        var balanceText = outstanding > 0 ? $" Outstanding balance: R {outstanding / 100m:0.00}." : string.Empty;

        await _notifications.NotifyUserAsync(
            order.UserId,
            "Ready for collection",
            $"Order {order.OrderNumber} is ready. Collect at {dto.CollectionAddress} between " +
            $"{dto.WindowStart:dd MMM HH:mm} and {dto.WindowEnd:dd MMM HH:mm}.{balanceText}",
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

    private async Task<long> CalculateOutstandingAsync(Order order, CancellationToken ct)
    {
        var total = order.TotalFinalCents ?? order.TotalIndicativeCents;
        var requests = await _paymentRequests.GetByOrderAsync(order.Id, ct);
        var approved = requests
            .SelectMany(r => r.Payments)
            .Where(p => p.ApprovedAt != null)
            .Sum(p => p.AmountCents);
        return Math.Max(0, total - approved);
    }

    private Task NotifyStatusAsync(Order order, string? note, CancellationToken ct) =>
        _notifications.NotifyUserAsync(
            order.UserId,
            "Order update",
            $"Order {order.OrderNumber} is now {order.Status}." + (note is null ? string.Empty : $" {note}"),
            new Dictionary<string, string> { ["orderId"] = order.Id.ToString() },
            ct);

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
