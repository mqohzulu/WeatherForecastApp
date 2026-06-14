using ChinaImportPlatform.Api.Dtos;

namespace ChinaImportPlatform.Api.IServices;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetActiveAsync(CancellationToken ct = default);

    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);

    Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);

    Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default);
}
