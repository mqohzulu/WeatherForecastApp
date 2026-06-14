using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Enums;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Services;

public class ConsolidatedDemandService : IConsolidatedDemandService
{
    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;
    private readonly IUserRepository _users;
    private readonly ICategoryRepository _categories;

    public ConsolidatedDemandService(
        IOrderRepository orders,
        IProductRepository products,
        IUserRepository users,
        ICategoryRepository categories)
    {
        _orders = orders;
        _products = products;
        _users = users;
        _categories = categories;
    }

    public async Task<IReadOnlyList<ConsolidatedDemandLineDto>> GetAsync(
        Guid? tripId,
        OrderStatus? minStatus,
        Guid? categoryId,
        CancellationToken ct = default)
    {
        var orders = await _orders.GetAllAsync(ct);
        var products = (await _products.GetAllAsync(ct)).ToDictionary(p => p.Id);
        var users = (await _users.GetAllAsync(ct)).ToDictionary(u => u.Id);
        var categories = (await _categories.GetAllAsync(ct)).ToDictionary(c => c.Id);

        // Default to "Confirmed and beyond" so only committed demand is purchased.
        var floor = minStatus ?? OrderStatus.Confirmed;

        var relevant = orders.Where(o =>
            (tripId is null || o.TripId == tripId) &&
            o.Status >= floor &&
            o.Status != OrderStatus.Cancelled &&
            o.Status != OrderStatus.Rejected);

        // Group by (product, variant) across all matching orders (US-S02).
        var groups = new Dictionary<(Guid? ProductId, Guid? VariantId), List<(Order Order, OrderItem Item)>>();

        foreach (var order in relevant)
        {
            foreach (var item in order.Items)
            {
                if (item.LineStatus == LineStatus.Rejected)
                {
                    continue;
                }

                if (categoryId.HasValue)
                {
                    if (item.ProductId is null ||
                        !products.TryGetValue(item.ProductId.Value, out var p) ||
                        p.CategoryId != categoryId.Value)
                    {
                        continue;
                    }
                }

                var key = (item.ProductId, item.VariantId);
                if (!groups.TryGetValue(key, out var list))
                {
                    list = new List<(Order, OrderItem)>();
                    groups[key] = list;
                }

                list.Add((order, item));
            }
        }

        var lines = new List<ConsolidatedDemandLineDto>();

        foreach (var (key, entries) in groups)
        {
            Product? product = key.ProductId.HasValue && products.TryGetValue(key.ProductId.Value, out var p) ? p : null;
            var variant = product?.Variants.FirstOrDefault(v => v.Id == key.VariantId);

            var customers = entries.Select(e => new DemandCustomerDto
            {
                OrderId = e.Order.Id,
                OrderNumber = e.Order.OrderNumber,
                CustomerId = e.Order.UserId,
                CustomerName = users.TryGetValue(e.Order.UserId, out var u) ? u.FullName : "Unknown",
                Quantity = e.Item.Quantity,
                Note = e.Item.Note
            }).ToList();

            lines.Add(new ConsolidatedDemandLineDto
            {
                ProductId = key.ProductId,
                VariantId = key.VariantId,
                ProductName = product?.Name
                              ?? entries.First().Item.ProductNameSnapshot
                              ?? entries.First().Item.CustomDescription
                              ?? "Custom item",
                VariantSummary = variant is null ? null : string.Join(", ", variant.Attributes.Select(a => $"{a.Key}: {a.Value}")),
                CategoryName = product is not null && categories.TryGetValue(product.CategoryId, out var c) ? c.Name : null,
                TotalQuantity = entries.Sum(e => e.Item.Quantity),
                CustomerCount = customers.Select(cu => cu.CustomerId).Distinct().Count(),
                Customers = customers
            });
        }

        return lines
            .OrderByDescending(l => l.TotalQuantity)
            .ToList();
    }
}
