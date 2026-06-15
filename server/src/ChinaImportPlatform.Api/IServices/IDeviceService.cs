using ChinaImportPlatform.Api.Dtos;

namespace ChinaImportPlatform.Api.IServices;

public interface IDeviceService
{
    /// <summary>Registers or replaces the user's push token for a platform (one active per platform).</summary>
    Task<DeviceTokenDto> RegisterAsync(RegisterDeviceDto dto, CancellationToken ct = default);

    Task<bool> UnregisterAsync(Guid id, CancellationToken ct = default);
}
