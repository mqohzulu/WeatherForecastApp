namespace ChinaImportPlatform.Api.IServices;

/// <summary>
/// Dispatches push notifications. The current implementation logs only; a real
/// implementation will fan out to FCM (Android) and APNs (iOS) as per the plan.
/// </summary>
public interface INotificationService
{
    Task NotifyUserAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken ct = default);

    Task BroadcastAsync(string? segment, string title, string body, IDictionary<string, string>? data = null, CancellationToken ct = default);
}
