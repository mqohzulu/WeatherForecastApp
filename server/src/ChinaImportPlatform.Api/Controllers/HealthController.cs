using ChinaImportPlatform.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("health")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    /// <summary>Liveness probe used by the load balancer and CI smoke tests.</summary>
    [HttpGet]
    public ActionResult<ApiResponse<object>> Get() =>
        Ok(ApiResponse<object>.Ok(new { status = "healthy", utc = DateTime.UtcNow }));
}
