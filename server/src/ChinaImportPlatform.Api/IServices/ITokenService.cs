using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.IServices;

/// <summary>Creates signed JWT access tokens and opaque refresh tokens.</summary>
public interface ITokenService
{
    /// <summary>Builds a signed JWT for the user and returns it with its expiry.</summary>
    (string Token, DateTime ExpiresAt) CreateAccessToken(User user);

    /// <summary>Generates a cryptographically random refresh token (the raw value).</summary>
    string CreateRefreshToken();

    /// <summary>SHA-256 hash used to store/compare refresh tokens without keeping the raw value.</summary>
    string Hash(string token);
}
