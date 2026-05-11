using Microsoft.Extensions.Caching.Memory;
using ProshoreHouseBroker.Application.DTOs;
using ProshoreHouseBroker.Application.Interfaces;
using ProshoreHouseBroker.Application.Services;

namespace ProshoreHouseBroker.Infrastructure.Caching;

public class CachedPropertyService : IPropertyService
{
    private const string CacheVersionKey = "properties:version";
    private readonly IMemoryCache _memoryCache;
    private readonly PropertyService _propertyService;

    public CachedPropertyService(IMemoryCache memoryCache, PropertyService propertyService)
    {
        _memoryCache = memoryCache;
        _propertyService = propertyService;
    }

    public async Task<Guid> CreateAsync(CreateListingRequestDto request, Guid brokerId, CancellationToken cancellationToken = default)
    {
        var id = await _propertyService.CreateAsync(request, brokerId, cancellationToken);
        BumpVersion();
        return id;
    }

    public async Task<PropertyResponseDto?> GetByIdAsync(Guid id, Guid? requesterId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"properties:detail:{id}:{requesterId}:{GetVersion()}";

        if (_memoryCache.TryGetValue(cacheKey, out PropertyResponseDto? cached))
        {
            return cached;
        }

        var result = await _propertyService.GetByIdAsync(id, requesterId, cancellationToken);
        _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }

    public async Task<PropertySearchResultDto> SearchAsync(PropertySearchRequestDto request, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"properties:search:{request.Location}:{request.MinPrice}:{request.MaxPrice}:{request.PropertyType}:{request.Page}:{request.PageSize}:{GetVersion()}";

        if (_memoryCache.TryGetValue(cacheKey, out PropertySearchResultDto? cached) && cached is not null)
        {
            return cached;
        }

        var result = await _propertyService.SearchAsync(request, cancellationToken);
        _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }

    public async Task UpdateAsync(Guid id, CreateListingRequestDto request, Guid brokerId, CancellationToken cancellationToken = default)
    {
        await _propertyService.UpdateAsync(id, request, brokerId, cancellationToken);
        BumpVersion();
    }

    public async Task DeleteAsync(Guid id, Guid brokerId, CancellationToken cancellationToken = default)
    {
        await _propertyService.DeleteAsync(id, brokerId, cancellationToken);
        BumpVersion();
    }

    private int GetVersion()
    {
        if (_memoryCache.TryGetValue(CacheVersionKey, out int version))
        {
            return version;
        }

        _memoryCache.Set(CacheVersionKey, 1);
        return 1;
    }

    private void BumpVersion()
    {
        _memoryCache.Set(CacheVersionKey, GetVersion() + 1);
    }
}
