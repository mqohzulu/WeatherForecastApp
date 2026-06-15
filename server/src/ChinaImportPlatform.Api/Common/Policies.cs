namespace ChinaImportPlatform.Api.Common;

/// <summary>Authorization policy names used across the API.</summary>
public static class Policies
{
    /// <summary>Restricts an endpoint to the seller/admin role.</summary>
    public const string SellerOnly = "SellerOnly";
}
