using ChinaImportPlatform.Api.Dtos;

namespace ChinaImportPlatform.Api.IServices;

public interface IAuthService
{
    /// <summary>Creates the user if new and issues an OTP. Returns the (dev) OTP for testing.</summary>
    Task<string> RequestOtpAsync(RequestOtpDto request, CancellationToken ct = default);

    Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto request, CancellationToken ct = default);

    Task<AuthResponseDto> RefreshAsync(RefreshTokenDto request, CancellationToken ct = default);

    Task<UserDto> GetProfileAsync(Guid userId, CancellationToken ct = default);

    Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileDto request, CancellationToken ct = default);
}
