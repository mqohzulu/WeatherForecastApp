namespace ChinaImportPlatform.Api.Enums;

/// <summary>Lifecycle of a sourcing trip used to group orders for consolidation.</summary>
public enum TripStatus
{
    Planned = 0,
    Open = 1,
    CutOff = 2,
    Sourcing = 3,
    Completed = 4
}
