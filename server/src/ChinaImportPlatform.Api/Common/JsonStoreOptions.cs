namespace ChinaImportPlatform.Api.Common;

/// <summary>
/// Configures where the JSON-backed repositories read and write their data.
/// Bound from the "JsonStore" configuration section.
/// </summary>
public class JsonStoreOptions
{
    public const string SectionName = "JsonStore";

    /// <summary>Absolute or content-root-relative directory containing the seed JSON files.</summary>
    public string DataPath { get; set; } = string.Empty;

    /// <summary>
    /// When true, mutations are written back to the JSON files so changes survive
    /// across requests within a run. Set false to keep the seed files pristine.
    /// </summary>
    public bool PersistChanges { get; set; } = true;
}
