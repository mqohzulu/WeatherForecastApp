using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Services;

public class AnnouncementService : IAnnouncementService
{
    private readonly IAnnouncementRepository _announcements;
    private readonly INotificationService _notifications;

    public AnnouncementService(IAnnouncementRepository announcements, INotificationService notifications)
    {
        _announcements = announcements;
        _notifications = notifications;
    }

    public async Task<IReadOnlyList<AnnouncementDto>> GetPublishedAsync(CancellationToken ct = default) =>
        (await _announcements.GetPublishedAsync(ct)).Select(a => a.ToDto()).ToList();

    public async Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto dto, CancellationToken ct = default)
    {
        var announcement = new Announcement
        {
            Title = dto.Title,
            Body = dto.Body,
            ImageS3Key = dto.ImageS3Key,
            CutoffDate = dto.CutoffDate,
            Segment = dto.Segment ?? "all",
            PublishedAt = dto.PublishNow ? DateTime.UtcNow : null
        };

        await _announcements.AddAsync(announcement, ct);

        if (dto.PublishNow)
        {
            await _notifications.BroadcastAsync(announcement.Segment, announcement.Title, announcement.Body, null, ct);
        }

        return announcement.ToDto();
    }
}
