using ChinaImportPlatform.Api.Dtos;

namespace ChinaImportPlatform.Api.IServices;

public interface ITripService
{
    Task<IReadOnlyList<TripDto>> GetAllAsync(CancellationToken ct = default);

    Task<TripDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Creates a trip and publishes an automatic cut-off announcement (US-S03).</summary>
    Task<TripDto> CreateAsync(CreateTripDto dto, CancellationToken ct = default);

    Task<TripDto> UpdateStatusAsync(Guid id, UpdateTripStatusDto dto, CancellationToken ct = default);
}
