using System.Text.Json;
using System.Text.Json.Serialization;
using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

/// <summary>
/// A thread-safe, JSON-file backed repository. Each concrete repository binds to a
/// single seed file (e.g. "orders.json"), loads it lazily into an in-memory cache,
/// and optionally writes mutations back to disk.
///
/// This exists so the API runs end-to-end with realistic test data before a real
/// database is connected. To move to PostgreSQL, implement the same repository
/// interfaces against <c>AppDbContext</c> and re-register them in <c>Program.cs</c>.
/// </summary>
public abstract class JsonRepository<T> : IRepository<T> where T : BaseEntity
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly string _filePath;
    private readonly bool _persistChanges;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private List<T>? _cache;

    protected JsonRepository(JsonStoreOptions options, string fileName)
    {
        _filePath = Path.Combine(options.DataPath, fileName);
        _persistChanges = options.PersistChanges;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            return (await GetCacheAsync(ct)).ToList();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            return (await GetCacheAsync(ct)).FirstOrDefault(e => e.Id == id);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<T>> FindAsync(Func<T, bool> predicate, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            return (await GetCacheAsync(ct)).Where(predicate).ToList();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            var cache = await GetCacheAsync(ct);
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            entity.CreatedAt = entity.CreatedAt == default ? DateTime.UtcNow : entity.CreatedAt;
            cache.Add(entity);
            await PersistAsync(cache, ct);
            return entity;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> UpdateAsync(T entity, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            var cache = await GetCacheAsync(ct);
            var index = cache.FindIndex(e => e.Id == entity.Id);
            if (index < 0)
            {
                return false;
            }

            entity.UpdatedAt = DateTime.UtcNow;
            cache[index] = entity;
            await PersistAsync(cache, ct);
            return true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            var cache = await GetCacheAsync(ct);
            var removed = cache.RemoveAll(e => e.Id == id) > 0;
            if (removed)
            {
                await PersistAsync(cache, ct);
            }

            return removed;
        }
        finally
        {
            _gate.Release();
        }
    }

    // Caller must hold the gate.
    private async Task<List<T>> GetCacheAsync(CancellationToken ct)
    {
        if (_cache != null)
        {
            return _cache;
        }

        if (File.Exists(_filePath))
        {
            await using var stream = File.OpenRead(_filePath);
            _cache = await JsonSerializer.DeserializeAsync<List<T>>(stream, SerializerOptions, ct)
                     ?? new List<T>();
        }
        else
        {
            _cache = new List<T>();
        }

        return _cache;
    }

    // Caller must hold the gate.
    private async Task PersistAsync(List<T> cache, CancellationToken ct)
    {
        if (!_persistChanges)
        {
            return;
        }

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, cache, SerializerOptions, ct);
    }
}
