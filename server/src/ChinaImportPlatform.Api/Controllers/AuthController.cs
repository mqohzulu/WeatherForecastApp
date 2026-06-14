using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ChinaImportPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>Request an OTP for registration or sign-in.</summary>
    [HttpPost("request-otp")]
    public async Task<ActionResult<ApiResponse<object>>> RequestOtp([FromBody] RequestOtpDto dto, CancellationToken ct)
    {
        var otp = await _auth.RequestOtpAsync(dto, ct);

        // The OTP is returned only because there is no SMS provider in the scaffold.
        return Ok(ApiResponse<object>.Ok(new { devOtp = otp }, "OTP sent."));
    }

    /// <summary>Verify an OTP and receive access/refresh tokens.</summary>
    [HttpPost("verify-otp")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> VerifyOtp([FromBody] VerifyOtpDto dto, CancellationToken ct)
    {
        var result = await _auth.VerifyOtpAsync(dto, ct);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result));
    }

    /// <summary>Exchange a refresh token for a new access token.</summary>
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
}
