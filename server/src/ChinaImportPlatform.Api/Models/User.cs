using ChinaImportPlatform.Api.Enums;

namespace ChinaImportPlatform.Api.Models;

/// <summary>A customer or the seller. Authentication is by verified mobile number.</summary>
public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    /// <summary>South African mobile number in E.164 form (e.g. +2782...). Unique.</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public UserRole Role { get; set; } = UserRole.Customer;

    public string? City { get; set; }

    public string? Suburb { get; set; }

    public string? PreferredCollectionPoint { get; set; }

    public bool PhoneVerified { get; set; }

    /// <summary>Per-channel notification preferences (US-C15). Default on.</summary>
    public bool NotifyStatusUpdates { get; set; } = true;

    public bool NotifyAnnouncements { get; set; } = true;

    public bool NotifyMessages { get; set; } = true;

    /// <summary>Soft-delete flag; orders are anonymised rather than destroyed (POPIA).</summary>
    public bool IsDeleted { get; set; }
}
