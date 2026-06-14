using ChinaImportPlatform.Api.Enums;

namespace ChinaImportPlatform.Api.Dtos;

public record OrderItemDto
{
    public Guid Id { get; init; }
    public Guid? ProductId { get; init; }
    public Guid? VariantId { get; init; }
    public string? ProductNameSnapshot { get; init; }
    public string? CustomDescription { get; init; }
    public int Quantity { get; init; }
    public string? Note { get; init; }
    public long? UnitPriceFinalCents { get; init; }
    public LineStatus LineStatus { get; init; }
}

public record OrderStatusHistoryDto
{
    public OrderStatus Status { get; init; }
    public string? Note { get; init; }
    public string? PhotoS3Key { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record OrderDto
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public Guid? TripId { get; init; }
    public OrderStatus Status { get; init; }
    public PaymentStatus PaymentStatus { get; init; }
    public long TotalIndicativeCents { get; init; }
    public long? TotalFinalCents { get; init; }
    public DateTime CreatedAt { get; init; }
    public IReadOnlyList<OrderItemDto> Items { get; init; } = Array.Empty<OrderItemDto>();
    public IReadOnlyList<OrderStatusHistoryDto> StatusHistory { get; init; } = Array.Empty<OrderStatusHistoryDto>();
}

public record CreateOrderItemDto
{
    public Guid? ProductId { get; init; }
    public Guid? VariantId { get; init; }
    public string? CustomDescription { get; init; }
    public int Quantity { get; init; } = 1;
    public string? Note { get; init; }
}

public record CreateOrderDto
{
    public Guid UserId { get; init; }
    public Guid? TripId { get; init; }
    public List<CreateOrderItemDto> Items { get; init; } = new();
}

public record UpdateOrderStatusDto
{
    public OrderStatus Status { get; init; }
    public string? Note { get; init; }
    public string? PhotoS3Key { get; init; }
    public Guid UpdatedBy { get; init; }
}

public record BulkUpdateOrderStatusDto
{
    public List<Guid> OrderIds { get; init; } = new();
    public OrderStatus Status { get; init; }
    public string? Note { get; init; }
    public Guid UpdatedBy { get; init; }
}

/// <summary>Seller sets/adjusts the final price of a line item (US-S04).</summary>
public record SetLinePriceDto
{
    public Guid OrderItemId { get; init; }
    public long UnitPriceFinalCents { get; init; }
}

public record SetOrderPricingDto
{
    public List<SetLinePriceDto> Lines { get; init; } = new();
}
