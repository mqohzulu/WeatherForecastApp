using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Enums;
using ChinaImportPlatform.Api.Models;
using ChinaImportPlatform.Api.Services;
using ChinaImportPlatform.Api.Tests.Fakes;
using Xunit;

namespace ChinaImportPlatform.Api.Tests;

public class OrderServiceTests
{
    private static readonly Guid CustomerId = Guid.Parse("00000000-0000-0000-0000-000000000002");

    private static (OrderService Service, FakeOrderRepository Orders) BuildService()
    {
        var product = new Product
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
            Name = "Angle Grinder 230 mm",
            IndicativePriceCents = 129900,
            Variants =
            {
                new ProductVariant { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), PriceAdjustmentCents = 25000 }
            }
        };

        var orders = new FakeOrderRepository();
        var products = new FakeProductRepository(new[] { product });
        var users = new FakeUserRepository(new[]
        {
            new User { Id = CustomerId, FullName = "Thabo Mokoena", PhoneNumber = "+27820000002" }
        });
        var paymentRequests = new FakePaymentRequestRepository();
        var service = new OrderService(orders, products, users, paymentRequests, new NullNotificationService());
        return (service, orders);
    }

    [Fact]
    public async Task PlaceOrder_AssignsHumanReadableOrderNumber()
    {
        var (service, _) = BuildService();

        var dto = new CreateOrderDto
        {
            UserId = CustomerId,
            Items = { new CreateOrderItemDto { ProductId = Guid.Parse("20000000-0000-0000-0000-000000000001"), Quantity = 2 } }
        };

        var order = await service.PlaceOrderAsync(dto);

        Assert.StartsWith("ORD-", order.OrderNumber);
        Assert.Equal(OrderStatus.Placed, order.Status);
    }

    [Fact]
    public async Task PlaceOrder_ComputesIndicativeTotalIncludingVariantAdjustment()
    {
        var (service, _) = BuildService();

        var dto = new CreateOrderDto
        {
            UserId = CustomerId,
            Items =
            {
                new CreateOrderItemDto
                {
                    ProductId = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                    VariantId = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                    Quantity = 2
                }
            }
        };

        var order = await service.PlaceOrderAsync(dto);

        // (129900 base + 25000 variant) * 2 = 309800
        Assert.Equal(309800, order.TotalIndicativeCents);
    }

    [Fact]
    public async Task PlaceOrder_WithCustomItem_StartsAwaitingQuote()
    {
        var (service, _) = BuildService();

        var dto = new CreateOrderDto
        {
            UserId = CustomerId,
            Items = { new CreateOrderItemDto { CustomDescription = "Industrial sewing machine with servo motor", Quantity = 1 } }
        };

        var order = await service.PlaceOrderAsync(dto);

        Assert.Equal(OrderStatus.AwaitingQuote, order.Status);
    }

    [Fact]
    public async Task UpdateStatus_AppendsImmutableHistoryEntry()
    {
        var (service, _) = BuildService();
        var dto = new CreateOrderDto
        {
            UserId = CustomerId,
            Items = { new CreateOrderItemDto { ProductId = Guid.Parse("20000000-0000-0000-0000-000000000001"), Quantity = 1 } }
        };
        var order = await service.PlaceOrderAsync(dto);

        var updated = await service.UpdateStatusAsync(order.Id, new UpdateOrderStatusDto
        {
            Status = OrderStatus.Confirmed,
            Note = "Confirmed.",
            UpdatedBy = Guid.Parse("00000000-0000-0000-0000-000000000001")
        });

        Assert.Equal(OrderStatus.Confirmed, updated.Status);
        Assert.Contains(updated.StatusHistory, h => h.Status == OrderStatus.Confirmed);
        Assert.True(updated.StatusHistory.Count >= 2);
    }

    [Fact]
    public async Task RejectLine_SetsLineRejectedWithReason()
    {
        var (service, _) = BuildService();
        var order = await service.PlaceOrderAsync(new CreateOrderDto
        {
            UserId = CustomerId,
            Items = { new CreateOrderItemDto { ProductId = Guid.Parse("20000000-0000-0000-0000-000000000001"), Quantity = 1 } }
        });
        var lineId = order.Items[0].Id;

        var updated = await service.RejectLineAsync(order.Id, new RejectLineDto
        {
            OrderItemId = lineId,
            Reason = "Supplier out of stock",
            UpdatedBy = Guid.Parse("00000000-0000-0000-0000-000000000001")
        });

        var line = updated.Items.Single(i => i.Id == lineId);
        Assert.Equal(LineStatus.Rejected, line.LineStatus);
        Assert.Equal("Supplier out of stock", line.RejectionReason);
    }

    [Fact]
    public async Task MarkReadyForCollection_SetsCollectionDetailsAndStatus()
    {
        var (service, _) = BuildService();
        var order = await service.PlaceOrderAsync(new CreateOrderDto
        {
            UserId = CustomerId,
            Items = { new CreateOrderItemDto { ProductId = Guid.Parse("20000000-0000-0000-0000-000000000001"), Quantity = 1 } }
        });

        var updated = await service.MarkReadyForCollectionAsync(order.Id, new MarkReadyForCollectionDto
        {
            CollectionAddress = "Warehouse, Kloof",
            WindowStart = new DateTime(2026, 7, 20, 9, 0, 0, DateTimeKind.Utc),
            WindowEnd = new DateTime(2026, 7, 20, 17, 0, 0, DateTimeKind.Utc),
            UpdatedBy = Guid.Parse("00000000-0000-0000-0000-000000000001")
        });

        Assert.Equal(OrderStatus.ReadyForCollection, updated.Status);
        Assert.Equal("Warehouse, Kloof", updated.CollectionAddress);
        Assert.NotNull(updated.CollectionWindowStart);
    }
}
