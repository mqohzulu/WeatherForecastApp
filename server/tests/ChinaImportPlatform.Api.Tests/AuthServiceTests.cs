using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Services;
using ChinaImportPlatform.Api.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ChinaImportPlatform.Api.Tests;

public class AuthServiceTests
{
    private const string Phone = "+27820001234";

    private static (AuthService Auth, FakeRefreshTokenRepository Refresh) Build()
    {
        var jwt = new JwtOptions { SigningKey = "test-signing-key-that-is-definitely-long-enough-123456" };
        var users = new FakeUserRepository();
        var refresh = new FakeRefreshTokenRepository();
        var tokens = new TokenService(jwt);
        var auth = new AuthService(users, refresh, tokens, new NullSmsSender(), jwt, NullLogger<AuthService>.Instance);
        return (auth, refresh);
    }

    [Fact]
    public async Task VerifyOtp_WithCorrectCode_IssuesTokens()
    {
        var (auth, _) = Build();
        var otp = await auth.RequestOtpAsync(new RequestOtpDto { PhoneNumber = Phone, FullName = "Test User" });

        var result = await auth.VerifyOtpAsync(new VerifyOtpDto { PhoneNumber = Phone, Otp = otp });

        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.True(result.AccessTokenExpiresAt > DateTime.UtcNow);
        Assert.Equal(Phone, result.User.PhoneNumber);
    }

    [Fact]
    public async Task VerifyOtp_WithWrongCode_Throws()
    {
        var (auth, _) = Build();
        await auth.RequestOtpAsync(new RequestOtpDto { PhoneNumber = Phone });

        await Assert.ThrowsAsync<AppException>(() =>
            auth.VerifyOtpAsync(new VerifyOtpDto { PhoneNumber = Phone, Otp = "000000" }));
    }

    [Fact]
    public async Task Refresh_RotatesToken_AndRevokesOldOne()
    {
        var (auth, refresh) = Build();
        var otp = await auth.RequestOtpAsync(new RequestOtpDto { PhoneNumber = Phone });
        var first = await auth.VerifyOtpAsync(new VerifyOtpDto { PhoneNumber = Phone, Otp = otp });

        var second = await auth.RefreshAsync(new RefreshTokenDto { RefreshToken = first.RefreshToken });

        Assert.NotEqual(first.RefreshToken, second.RefreshToken);

        // The originally issued refresh token must no longer be usable.
        await Assert.ThrowsAsync<AppException>(() =>
            auth.RefreshAsync(new RefreshTokenDto { RefreshToken = first.RefreshToken }));
    }
}
