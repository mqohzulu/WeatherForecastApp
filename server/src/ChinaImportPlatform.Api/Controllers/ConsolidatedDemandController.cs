using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Enums;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.SellerOnly)]
[Route("api/v1/consolidated-demand")]
[Produces("application/json")]
public class ConsolidatedDemandController : ControllerBase
{
    private readonly IConsolidatedDemandService _demand;

    public ConsolidatedDemandController(IConsolidatedDemandService demand)
    {
        _demand = demand;
    }

    /// <summary>
    /// The consolidated buying list: order lines aggregated by product and variant for a
    /// trip window, expandable to the contributing customers (US-S02).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ConsolidatedDemandLineDto>>>> Get(
        [FromQuery] Guid? tripId,
        [FromQuery] OrderStatus? minStatus,
        [FromQuery] Guid? categoryId,
        CancellationToken ct = default)
    {
        var result = await _demand.GetAsync(tripId, minStatus, categoryId, ct);
        return Ok(ApiResponse<IReadOnlyList<ConsolidatedDemandLineDto>>.Ok(result));
    }
}
