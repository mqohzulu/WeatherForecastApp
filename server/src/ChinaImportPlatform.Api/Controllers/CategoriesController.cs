using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/categories")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categories;

    public CategoriesController(ICategoryService categories)
    {
        _categories = categories;
    }

    /// <summary>Lists categories. By default only active categories with products are returned.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CategoryDto>>>> Get([FromQuery] bool includeInactive = false, CancellationToken ct = default)
    {
        var result = includeInactive
            ? await _categories.GetAllAsync(ct)
            : await _categories.GetActiveAsync(ct);

        return Ok(ApiResponse<IReadOnlyList<CategoryDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(Guid id, CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(id, ct);
        return category is null
            ? NotFound(ApiResponse<CategoryDto>.Fail($"Category {id} not found."))
            : Ok(ApiResponse<CategoryDto>.Ok(category));
    }

    [Authorize(Policy = Policies.SellerOnly)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var created = await _categories.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<CategoryDto>.Ok(created));
    }

    [Authorize(Policy = Policies.SellerOnly)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(Guid id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
    {
        var updated = await _categories.UpdateAsync(id, dto, ct);
        return Ok(ApiResponse<CategoryDto>.Ok(updated));
    }
}
