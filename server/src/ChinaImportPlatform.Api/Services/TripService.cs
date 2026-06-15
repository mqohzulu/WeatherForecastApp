using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Enums;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _trips;
    private readonly IAnnouncementRepository _announcements;
    private readonly INotificationService _notifications;

    public TripService(ITripRepository trips, IAnnouncementRepository announcements, INotificationService notifications)
    {
        _trips = trips;
        _announcements = announcements;
        _notifications = notifications;
    }

    public async Task<IReadOnlyList<TripDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _trips.GetAllAsync(ct))
            .OrderByDescending(t => t.CutoffDate)
            .Select(t => t.ToDto())
            .ToList();

    public async Task<TripDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var trip = await _trips.GetByIdAsync(id, ct);
        return trip?.ToDto();
    }

    public async Task<TripDto> CreateAsync(CreateTripDto dto, CancellationToken ct = default)
    {
        var trip = new Trip
        {
            Name = dto.Name,
            CutoffDate = dto.CutoffDate,
            DepartureDate = dto.DepartureDate,
            Status = TripStatus.Open
        };

        await _trips.AddAsync(trip, ct);

        // Creating a trip publishes an automatic cut-off announcement (US-S03).
        var announcement = new Announcement
        {
            Title = $"New sourcing trip: {trip.Name}",
            Body = $"Order cut-off is {trip.CutoffDate:dd MMM yyyy}. Orders placed after the cut-off roll into the next trip.",
            CutoffDate = trip.CutoffDate,
            Segment = "all",
            PublishedAt = DateTime.UtcNow
        };
        await _announcements.AddAsync(announcement, ct);
        await _notifications.BroadcastAsync("all", announcement.Title, announcement.Body, null, ct);

        return trip.ToDto();
    }

    public async Task<TripDto> UpdateStatusAsync(Guid id, UpdateTripStatusDto dto, CancellationToken ct = default)
    {
        var trip = await _trips.GetByIdAsync(id, ct)
                   ?? throw new NotFoundException($"Trip {id} not found.");

        trip.Status = dto.Status;
        await _trips.UpdateAsync(trip, ct);
        return trip.ToDto();
    }
}
