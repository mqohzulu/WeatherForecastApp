namespace ChinaImportPlatform.Api.Models;

/// <summary>
/// A persisted refresh token. Only the SHA-256 hash of the token is stored, so a leak of
/// the data store does not expose usable tokens. Tokens are rotated on every use (US-X01).
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    /// <summary>Hash of the token that replaced this one when it was rotated.</summary>
    public string? ReplacedByTokenHash { get; set; }

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
