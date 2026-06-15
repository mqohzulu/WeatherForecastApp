using ChinaImportPlatform.Api.IServices;

namespace ChinaImportPlatform.Api.Services;

/// <summary>
/// Development notification sink. Swap for an FCM/APNs implementation in production;
/// the rest of the application depends only on <see cref="INotificationService"/>.
/// </summary>
public class LoggingNotificationService : INotificationService
{
    private readonly ILogger<LoggingNotificationService> _logger;

    public LoggingNotificationService(ILogger<LoggingNotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyUserAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken ct = default)
    {
        _logger.LogInformation("PUSH -> user {UserId}: {Title} | {Body}", userId, title, body);
        return Task.CompletedTask;
    }

    public Task BroadcastAsync(string? segment, string title, string body, IDictionary<string, string>? data = null, CancellationToken ct = default)
    {
        _logger.LogInformation("BROADCAST -> segment {Segment}: {Title} | {Body}", segment ?? "all", title, body);
        return Task.CompletedTask;
    }
}
