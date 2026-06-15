namespace ChinaImportPlatform.Api.Enums;

/// <summary>Per-line status so individual items can be rejected or sourced independently.</summary>
public enum LineStatus
{
    Pending = 0,
    Quoted = 1,
    Accepted = 2,
    Rejected = 3,
    Sourced = 4
}
