using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/announcements")]
[Produces("application/json")]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementService _announcements;

    public AnnouncementsController(IAnnouncementService announcements)
    {
        _announcements = announcements;
    }

    /// <summary>The customer-facing announcements feed (US-C16).</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AnnouncementDto>>>> Get(CancellationToken ct)
    {
        var result = await _announcements.GetPublishedAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<AnnouncementDto>>.Ok(result));
    }

    /// <summary>Send a broadcast to all customers or a segment (US-S08).</summary>
    [Authorize(Policy = Policies.SellerOnly)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AnnouncementDto>>> Create([FromBody] CreateAnnouncementDto dto, CancellationToken ct)
    {
        var created = await _announcements.CreateAsync(dto, ct);
        return Ok(ApiResponse<AnnouncementDto>.Ok(created));
    }
}
