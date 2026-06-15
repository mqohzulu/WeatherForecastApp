namespace ChinaImportPlatform.Api.Common;

/// <summary>
/// JWT signing/validation settings, bound from the "Jwt" configuration section.
/// In production the signing key MUST come from AWS Secrets Manager, never source control.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "china-import-platform";

    public string Audience { get; set; } = "china-import-platform-app";

    /// <summary>Symmetric signing key (HS256). Must be at least 32 characters.</summary>
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>Access-token lifetime. Short-lived per US-X01.</summary>
    public int AccessTokenMinutes { get; set; } = 15;

    /// <summary>Refresh-token lifetime in days.</summary>
    public int RefreshTokenDays { get; set; } = 30;
}
