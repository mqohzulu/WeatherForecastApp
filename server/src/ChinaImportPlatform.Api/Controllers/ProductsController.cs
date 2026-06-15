using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;

    public ProductsController(IProductService products)
    {
        _products = products;
    }

    /// <summary>
    /// Lists products with optional category filter, keyword search, price range and
    /// sort (US-C05/C07). Query: categoryId, search, minPriceCents, maxPriceCents,
    /// sort (name|price_asc|price_desc|newest), page, pageSize.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> Get([FromQuery] ProductQuery query, CancellationToken ct = default)
    {
        var result = await _products.GetAsync(query, ct);
        return Ok(ApiResponse<PagedResult<ProductDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(Guid id, CancellationToken ct)
    {
        var product = await _products.GetByIdAsync(id, ct);
        return product is null
            ? NotFound(ApiResponse<ProductDto>.Fail($"Product {id} not found."))
            : Ok(ApiResponse<ProductDto>.Ok(product));
    }

    [Authorize(Policy = Policies.SellerOnly)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var created = await _products.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ProductDto>.Ok(created));
    }

    [Authorize(Policy = Policies.SellerOnly)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken ct)
    {
        var updated = await _products.UpdateAsync(id, dto, ct);
        return Ok(ApiResponse<ProductDto>.Ok(updated));
    }

    /// <summary>Hides a product from the catalogue while preserving it on historical orders (US-S11).</summary>
    [Authorize(Policy = Policies.SellerOnly)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Hide(Guid id, CancellationToken ct)
    {
        var hidden = await _products.HideAsync(id, ct);
        return hidden
            ? Ok(ApiResponse<object>.Ok(new { id }, "Product hidden."))
            : NotFound(ApiResponse<object>.Fail($"Product {id} not found."));
    }
}
