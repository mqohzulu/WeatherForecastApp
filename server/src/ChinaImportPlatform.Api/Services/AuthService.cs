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
/// Phone + OTP authentication issuing signed JWT access tokens with rotating refresh
/// tokens (US-X01). OTP delivery goes through <see cref="ISmsSender"/>; in the scaffold
/// the OTP is held in memory and logged. The OTP store should move to a distributed
/// cache (e.g. Redis/ElastiCache) once the API runs on more than one instance.
/// </summary>
public class AuthService : IAuthService
{
    private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> OtpStore = new();
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(5);

    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly ITokenService _tokens;
    private readonly ISmsSender _sms;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        ITokenService tokens,
        ISmsSender sms,
        JwtOptions jwtOptions,
        ILogger<AuthService> logger)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _tokens = tokens;
        _sms = sms;
        _jwtOptions = jwtOptions;
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

        await _sms.SendOtpAsync(request.PhoneNumber, otp, ct);
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

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResponseDto> RefreshAsync(RefreshTokenDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new AppException("A refresh token is required.");
        }

        var hash = _tokens.Hash(request.RefreshToken);
        var stored = await _refreshTokens.GetByHashAsync(hash, ct);

        if (stored is null || !stored.IsActive)
        {
            throw new AppException("The refresh token is invalid or expired.", 401);
        }

        var user = await _users.GetByIdAsync(stored.UserId, ct)
                   ?? throw new NotFoundException("User not found.");

        // Rotate: revoke the presented token before issuing a new pair.
        var response = await IssueTokensAsync(user, ct);
        stored.RevokedAt = DateTime.UtcNow;
        stored.ReplacedByTokenHash = _tokens.Hash(response.RefreshToken);
        await _refreshTokens.UpdateAsync(stored, ct);

        return response;
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

    public async Task<UserDto> UpdateNotificationPreferencesAsync(Guid userId, NotificationPreferencesDto prefs, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct)
                   ?? throw new NotFoundException("User not found.");

        user.NotifyStatusUpdates = prefs.NotifyStatusUpdates;
        user.NotifyAnnouncements = prefs.NotifyAnnouncements;
        user.NotifyMessages = prefs.NotifyMessages;

        await _users.UpdateAsync(user, ct);
        return user.ToDto();
    }

    private async Task<AuthResponseDto> IssueTokensAsync(User user, CancellationToken ct)
    {
        var (accessToken, expiresAt) = _tokens.CreateAccessToken(user);
        var rawRefresh = _tokens.CreateRefreshToken();

        await _refreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokens.Hash(rawRefresh),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
        }, ct);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = rawRefresh,
            AccessTokenExpiresAt = expiresAt,
            User = user.ToDto()
        };
    }
}
