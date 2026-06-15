namespace ChinaImportPlatform.Api.Models;

/// <summary>A push-notification token; one active token per user per platform.</summary>
public class DeviceToken : BaseEntity
{
    public Guid UserId { get; set; }

    public string FcmToken { get; set; } = string.Empty;

    /// <summary>"android" or "ios".</summary>
    public string Platform { get; set; } = "android";
}
