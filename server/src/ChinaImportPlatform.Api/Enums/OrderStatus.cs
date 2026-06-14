namespace ChinaImportPlatform.Api.Enums;

/// <summary>
/// The defined lifecycle an order moves through. Every transition is recorded
/// in the order's status history and triggers a customer notification.
/// </summary>
public enum OrderStatus
{
    /// <summary>Customer has submitted the order through the app.</summary>
    Placed = 0,

    /// <summary>Custom order submitted but not yet priced by the seller.</summary>
    AwaitingQuote = 1,

    /// <summary>Seller has reviewed and accepted the order (possibly re-priced).</summary>
    Confirmed = 2,

    /// <summary>Seller is in China purchasing the goods for this order.</summary>
    BeingSourced = 3,

    /// <summary>Goods have left China and are in transit to South Africa.</summary>
    Shipped = 4,

    /// <summary>Goods have cleared customs and are with the seller.</summary>
    ArrivedInSa = 5,

    /// <summary>The order is packed and may be collected or delivered.</summary>
    ReadyForCollection = 6,

    /// <summary>Customer has received the goods and payment is settled.</summary>
    Completed = 7,

    /// <summary>Order cancelled by the customer while still Placed.</summary>
    Cancelled = 8,

    /// <summary>Order or line rejected by the seller, with a reason recorded.</summary>
    Rejected = 9
}
