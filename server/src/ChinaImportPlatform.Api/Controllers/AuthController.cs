using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[EnableRateLimiting("auth")]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IWebHostEnvironment _environment;

    public AuthController(IAuthService auth, IWebHostEnvironment environment)
    {
        _auth = auth;
        _environment = environment;
    }

    /// <summary>Request an OTP for registration or sign-in.</summary>
    [AllowAnonymous]
    [HttpPost("request-otp")]
    public async Task<ActionResult<ApiResponse<object>>> RequestOtp([FromBody] RequestOtpDto dto, CancellationToken ct)
    {
        var otp = await _auth.RequestOtpAsync(dto, ct);

        // Outside Development the OTP is delivered by SMS and never returned in the response.
        object payload = _environment.IsDevelopment()
            ? new { devOtp = otp }
            : new { };
        return Ok(ApiResponse<object>.Ok(payload, "OTP sent."));
    }

    /// <summary>Verify an OTP and receive access/refresh tokens.</summary>
    [AllowAnonymous]
    [HttpPost("verify-otp")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> VerifyOtp([FromBody] VerifyOtpDto dto, CancellationToken ct)
    {
        var result = await _auth.VerifyOtpAsync(dto, ct);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result));
    }

    /// <summary>Exchange a refresh token for a new access token.</summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh([FromBody] RefreshTokenDto dto, CancellationToken ct)
    {
        var result = await _auth.RefreshAsync(dto, ct);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result));
    }

    [HttpGet("profile/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile(Guid userId, CancellationToken ct)
    {
        var user = await _auth.GetProfileAsync(userId, ct);
        return Ok(ApiResponse<UserDto>.Ok(user));
    }

    [HttpPut("profile/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile(Guid userId, [FromBody] UpdateProfileDto dto, CancellationToken ct)
    {
        var user = await _auth.UpdateProfileAsync(userId, dto, ct);
        return Ok(ApiResponse<UserDto>.Ok(user, "Profile updated."));
    }

    /// <summary>Toggle per-channel notification preferences (US-C15).</summary>
    [HttpPut("profile/{userId:guid}/notification-preferences")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateNotificationPreferences(Guid userId, [FromBody] NotificationPreferencesDto dto, CancellationToken ct)
    {
        var user = await _auth.UpdateNotificationPreferencesAsync(userId, dto, ct);
        return Ok(ApiResponse<UserDto>.Ok(user, "Preferences updated."));
    }
}
