using ChinaImportPlatform.Api.Enums;

namespace ChinaImportPlatform.Api.Models;

/// <summary>A sourcing trip; groups orders into a window for the consolidated demand list.</summary>
public class Trip : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public DateTime CutoffDate { get; set; }

    public DateTime? DepartureDate { get; set; }

    public TripStatus Status { get; set; } = TripStatus.Planned;
}
