using ChinaImportPlatform.Api.Dtos;

namespace ChinaImportPlatform.Api.IServices;

public interface IAnnouncementService
{
    Task<IReadOnlyList<AnnouncementDto>> GetPublishedAsync(CancellationToken ct = default);

    Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto dto, CancellationToken ct = default);
}
