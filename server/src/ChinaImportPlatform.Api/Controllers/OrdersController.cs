using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/orders")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;

    public OrdersController(IOrderService orders)
    {
        _orders = orders;
    }

    /// <summary>The seller's order queue (US-S01).</summary>
    [Authorize(Policy = Policies.SellerOnly)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<OrderDto>>>> GetQueue(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _orders.GetQueueAsync(page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<OrderDto>>.Ok(result));
    }

    /// <summary>A customer's own order history (US-C14).</summary>
    [HttpGet("by-user/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OrderDto>>>> GetByUser(Guid userId, CancellationToken ct)
    {
        var result = await _orders.GetByUserAsync(userId, ct);
        return Ok(ApiResponse<IReadOnlyList<OrderDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetById(Guid id, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(id, ct);
        return order is null
            ? NotFound(ApiResponse<OrderDto>.Fail($"Order {id} not found."))
            : Ok(ApiResponse<OrderDto>.Ok(order));
    }

    /// <summary>Submit a cart as an order (US-C09).</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDto>>> Place([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var created = await _orders.PlaceOrderAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<OrderDto>.Ok(created));
    }

    /// <summary>Update a single order's status with an optional note (US-S05).</summary>
    [Authorize(Policy = Policies.SellerOnly)]
    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto, CancellationToken ct)
    {
        var updated = await _orders.UpdateStatusAsync(id, dto, ct);
        return Ok(ApiResponse<OrderDto>.Ok(updated));
    }

    /// <summary>Update many orders' status in one action (US-S06).</summary>
    [Authorize(Policy = Policies.SellerOnly)]
    [HttpPost("bulk-status")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OrderDto>>>> BulkUpdateStatus([FromBody] BulkUpdateOrderStatusDto dto, CancellationToken ct)
    {
        var updated = await _orders.BulkUpdateStatusAsync(dto, ct);
        return Ok(ApiResponse<IReadOnlyList<OrderDto>>.Ok(updated, $"{updated.Count} orders updated."));
    }

    /// <summary>Set/adjust the final price per line item (US-S04).</summary>
    [Authorize(Policy = Policies.SellerOnly)]
    [HttpPut("{id:guid}/pricing")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> SetPricing(Guid id, [FromBody] SetOrderPricingDto dto, CancellationToken ct)
    {
        var updated = await _orders.SetPricingAsync(id, dto, ct);
        return Ok(ApiResponse<OrderDto>.Ok(updated));
    }

    /// <summary>Cancel an order while it is still Placed (US-C12).</summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> Cancel(Guid id, [FromQuery] Guid requestedBy, CancellationToken ct)
    {
        var cancelled = await _orders.CancelAsync(id, requestedBy, ct);
        return Ok(ApiResponse<OrderDto>.Ok(cancelled, "Order cancelled."));
    }
}
