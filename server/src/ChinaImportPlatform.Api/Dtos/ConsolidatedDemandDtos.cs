namespace ChinaImportPlatform.Api.Dtos;

/// <summary>One customer's contribution to a consolidated demand line.</summary>
public record DemandCustomerDto
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public string? Note { get; init; }
}

/// <summary>An aggregated buying-list line: product + variant rolled up across orders (US-S02).</summary>
public record ConsolidatedDemandLineDto
{
    public Guid? ProductId { get; init; }
    public Guid? VariantId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? VariantSummary { get; init; }
    public string? CategoryName { get; init; }
    public int TotalQuantity { get; init; }
    public int CustomerCount { get; init; }
    public IReadOnlyList<DemandCustomerDto> Customers { get; init; } = Array.Empty<DemandCustomerDto>();
}
