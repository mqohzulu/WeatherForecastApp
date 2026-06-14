using System.Collections.Concurrent;
using System.Security.Cryptography;
using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Enums;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Services;

/// <summary>
/// Phone + OTP authentication. This is a development-grade implementation: OTPs are
/// held in memory and tokens are opaque placeholders. The production version will
/// send OTPs via an SMS provider and issue signed JWTs with refresh-token rotation
/// (US-X01). The contract (<see cref="IAuthService"/>) does not change.
/// </summary>
public class AuthService : IAuthService
{
    private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> OtpStore = new();
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);

    private readonly IUserRepository _users;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository users, ILogger<AuthService> logger)
    {
        _users = users;
        _logger = logger;
    }

    public async Task<string> RequestOtpAsync(RequestOtpDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            throw new AppException("A mobile number is required.");
        }

        var user = await _users.GetByPhoneAsync(request.PhoneNumber, ct);
        if (user is null)
        {
            user = new User
            {
                FullName = request.FullName ?? "New Customer",
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Role = UserRole.Customer
            };
            await _users.AddAsync(user, ct);
        }

        var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        OtpStore[request.PhoneNumber] = (otp, DateTime.UtcNow.Add(OtpLifetime));

        // In production this is sent by SMS; here it is logged and returned for testing only.
        _logger.LogInformation("OTP for {Phone} is {Otp} (valid 5 min)", request.PhoneNumber, otp);
        return otp;
    }

    public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto request, CancellationToken ct = default)
    {
        if (!OtpStore.TryGetValue(request.PhoneNumber, out var entry))
        {
            throw new AppException("No OTP was requested for this number.");
        }

        if (entry.Expiry < DateTime.UtcNow)
        {
            OtpStore.TryRemove(request.PhoneNumber, out _);
            throw new AppException("The OTP has expired. Please request a new one.");
        }

        if (entry.Otp != request.Otp)
        {
            throw new AppException("The OTP is incorrect.");
        }

        OtpStore.TryRemove(request.PhoneNumber, out _);

        var user = await _users.GetByPhoneAsync(request.PhoneNumber, ct)
                   ?? throw new NotFoundException("User not found.");

        if (!user.PhoneVerified)
        {
            user.PhoneVerified = true;
            await _users.UpdateAsync(user, ct);
        }

        return BuildAuthResponse(user);
    }

    public Task<AuthResponseDto> RefreshAsync(RefreshTokenDto request, CancellationToken ct = default)
    {
        // Placeholder: a production implementation validates and rotates the refresh token.
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new AppException("A refresh token is required.");
        }

        throw new AppException("Token refresh is not implemented in the JSON-backed scaffold.", 501);
    }

    public async Task<UserDto> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct)
                   ?? throw new NotFoundException("User not found.");
        return user.ToDto();
    }

    public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileDto request, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct)
                   ?? throw new NotFoundException("User not found.");

        user.FullName = request.FullName ?? user.FullName;
        user.Email = request.Email ?? user.Email;
        user.City = request.City ?? user.City;
        user.Suburb = request.Suburb ?? user.Suburb;
        user.PreferredCollectionPoint = request.PreferredCollectionPoint ?? user.PreferredCollectionPoint;

        await _users.UpdateAsync(user, ct);
        return user.ToDto();
    }

    private static AuthResponseDto BuildAuthResponse(User user) => new()
    {
        AccessToken = GenerateOpaqueToken(),
        RefreshToken = GenerateOpaqueToken(),
        AccessTokenExpiresAt = DateTime.UtcNow.Add(AccessTokenLifetime),
        User = user.ToDto()
    };

    private static string GenerateOpaqueToken() =>
        Convert.ToBase64String(Guid.NewGuid().ToByteArray());
}
