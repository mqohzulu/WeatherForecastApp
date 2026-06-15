using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceTokenRepository _devices;

    public DeviceService(IDeviceTokenRepository devices)
    {
        _devices = devices;
    }

    public async Task<DeviceTokenDto> RegisterAsync(RegisterDeviceDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.FcmToken))
        {
            throw new AppException("A push token is required.");
        }

        // Keep one active token per user per platform: drop any existing ones first.
        var existing = await _devices.GetByUserAsync(dto.UserId, ct);
        foreach (var token in existing.Where(t => t.Platform == dto.Platform))
        {
            await _devices.DeleteAsync(token.Id, ct);
        }

        var device = new DeviceToken
        {
            UserId = dto.UserId,
            FcmToken = dto.FcmToken,
            Platform = dto.Platform
        };
        await _devices.AddAsync(device, ct);

        return new DeviceTokenDto
        {
            Id = device.Id,
            UserId = device.UserId,
            FcmToken = device.FcmToken,
            Platform = device.Platform
        };
    }

    public Task<bool> UnregisterAsync(Guid id, CancellationToken ct = default) =>
        _devices.DeleteAsync(id, ct);
}
