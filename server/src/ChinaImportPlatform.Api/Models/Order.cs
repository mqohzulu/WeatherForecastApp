using ChinaImportPlatform.Api.Enums;

namespace ChinaImportPlatform.Api.Models;

/// <summary>
/// A customer order. Line items and the immutable status history are owned by
/// the order aggregate. Monetary totals are stored in ZAR cents.
/// </summary>
public class Order : BaseEntity
{
    /// <summary>Human-readable reference, e.g. ORD-2026-0145.</summary>
    public string OrderNumber { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public Guid? TripId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Placed;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    public long TotalIndicativeCents { get; set; }

    public long? TotalFinalCents { get; set; }

    public List<OrderItem> Items { get; set; } = new();

    public List<OrderStatusHistoryEntry> StatusHistory { get; set; } = new();
}

/// <summary>A single line on an order. Product/variant are nullable for custom items.</summary>
public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public string? ProductNameSnapshot { get; set; }

    /// <summary>Free-text description for custom/special orders.</summary>
    public string? CustomDescription { get; set; }

    public int Quantity { get; set; }

    public string? Note { get; set; }

    public long? UnitPriceFinalCents { get; set; }

    public LineStatus LineStatus { get; set; } = LineStatus.Pending;
}

/// <summary>An immutable audit entry for every status change (powers the order timeline).</summary>
public class OrderStatusHistoryEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public OrderStatus Status { get; set; }

    public string? Note { get; set; }

    public string? PhotoS3Key { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
