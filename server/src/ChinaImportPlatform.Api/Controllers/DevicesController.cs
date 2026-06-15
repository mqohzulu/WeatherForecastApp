using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/devices")]
[Produces("application/json")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _devices;

    public DevicesController(IDeviceService devices)
    {
        _devices = devices;
    }

    /// <summary>Registers this device's push token so notifications reach the active device.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<DeviceTokenDto>>> Register([FromBody] RegisterDeviceDto dto, CancellationToken ct)
    {
        var result = await _devices.RegisterAsync(dto, ct);
        return Ok(ApiResponse<DeviceTokenDto>.Ok(result, "Device registered."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Unregister(Guid id, CancellationToken ct)
    {
        var removed = await _devices.UnregisterAsync(id, ct);
        return removed
            ? Ok(ApiResponse<object>.Ok(new { id }, "Device unregistered."))
            : NotFound(ApiResponse<object>.Fail($"Device {id} not found."));
    }
}
