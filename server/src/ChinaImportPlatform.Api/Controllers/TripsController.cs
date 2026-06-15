using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/trips")]
[Produces("application/json")]
public class TripsController : ControllerBase
{
    private readonly ITripService _trips;

    public TripsController(ITripService trips)
    {
        _trips = trips;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TripDto>>>> Get(CancellationToken ct)
    {
        var trips = await _trips.GetAllAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<TripDto>>.Ok(trips));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TripDto>>> GetById(Guid id, CancellationToken ct)
    {
        var trip = await _trips.GetByIdAsync(id, ct);
        return trip is null
            ? NotFound(ApiResponse<TripDto>.Fail($"Trip {id} not found."))
            : Ok(ApiResponse<TripDto>.Ok(trip));
    }

    /// <summary>Creates a trip and publishes the cut-off announcement (US-S03).</summary>
    [Authorize(Policy = Policies.SellerOnly)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TripDto>>> Create([FromBody] CreateTripDto dto, CancellationToken ct)
    {
        var created = await _trips.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<TripDto>.Ok(created));
    }

    [Authorize(Policy = Policies.SellerOnly)]
    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<TripDto>>> UpdateStatus(Guid id, [FromBody] UpdateTripStatusDto dto, CancellationToken ct)
    {
        var updated = await _trips.UpdateStatusAsync(id, dto, ct);
        return Ok(ApiResponse<TripDto>.Ok(updated));
    }
}
