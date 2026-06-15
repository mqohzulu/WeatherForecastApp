using ChinaImportPlatform.Api.Enums;

namespace ChinaImportPlatform.Api.Dtos;

public record TripDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CutoffDate { get; init; }
    public DateTime? DepartureDate { get; init; }
    public TripStatus Status { get; init; }
}

public record CreateTripDto
{
    public string Name { get; init; } = string.Empty;
    public DateTime CutoffDate { get; init; }
    public DateTime? DepartureDate { get; init; }
}

public record UpdateTripStatusDto
{
    public TripStatus Status { get; init; }
}
