namespace ChinaImportPlatform.Api.Dtos;

/// <summary>Registers the current device's push token (US-C15 prerequisite).</summary>
public record RegisterDeviceDto
{
    public Guid UserId { get; init; }
    public string FcmToken { get; init; } = string.Empty;

    /// <summary>"android" or "ios".</summary>
    public string Platform { get; init; } = "android";
}

public record DeviceTokenDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string FcmToken { get; init; } = string.Empty;
    public string Platform { get; init; } = "android";
}
