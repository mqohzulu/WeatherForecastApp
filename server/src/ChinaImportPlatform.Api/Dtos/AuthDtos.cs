using ChinaImportPlatform.Api.Enums;

namespace ChinaImportPlatform.Api.Dtos;

/// <summary>Step 1 of registration / sign-in: request an OTP for a mobile number.</summary>
public record RequestOtpDto
{
    public string PhoneNumber { get; init; } = string.Empty;
    public string? FullName { get; init; }
    public string? Email { get; init; }
}

/// <summary>Step 2: verify the OTP and receive tokens.</summary>
public record VerifyOtpDto
{
    public string PhoneNumber { get; init; } = string.Empty;
    public string Otp { get; init; } = string.Empty;
}

/// <summary>Issued on successful verification.</summary>
public record AuthResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; init; }
    public UserDto User { get; init; } = new();
}

public record RefreshTokenDto
{
    public string RefreshToken { get; init; } = string.Empty;
}

public record UserDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public UserRole Role { get; init; }
    public string? City { get; init; }
    public string? Suburb { get; init; }
    public string? PreferredCollectionPoint { get; init; }
    public bool NotifyStatusUpdates { get; init; }
    public bool NotifyAnnouncements { get; init; }
    public bool NotifyMessages { get; init; }
}

public record UpdateProfileDto
{
    public string? FullName { get; init; }
    public string? Email { get; init; }
    public string? City { get; init; }
    public string? Suburb { get; init; }
    public string? PreferredCollectionPoint { get; init; }
}

/// <summary>Per-channel notification preferences (US-C15).</summary>
public record NotificationPreferencesDto
{
    public bool NotifyStatusUpdates { get; init; } = true;
    public bool NotifyAnnouncements { get; init; } = true;
    public bool NotifyMessages { get; init; } = true;
}
