using System.Net;
using CloudLookup;
using headers.security.Scanner.CloudLookup;
using Microsoft.Extensions.Caching.Memory;

namespace headers.security.CachedContent.Data;

public class CachingCloudLookupRepository(CloudLookupClient client, IMemoryCache cache) : ICloudLookupRepository, ICachedContentRepository
{
    public string CacheKey => "CloudLookup";
    public string ExpiryCacheKey => CacheKey + "/ExpiryKey";
    public string StateFilename => "cloudlookup";
    public Type Type => typeof(CloudLookupCollection);
    
    private static readonly MemoryCacheEntryOptions CacheEntryOptions = new()
    {
        Priority = CacheItemPriority.NeverRemove
    };

    public string GetCloud(IPAddress address)
    {
        var lookup = GetLookup();

        var cloud = lookup?.GetCloud(address);

        if (cloud == null)
        {
            return null;
        }
        
        return cloud.ToString();
    }

    private CloudLookupCollection GetLookup()
    {
        cache.TryGetValue<CloudLookupCollection>(CacheKey, out var lookup);

        return lookup;
    }

    public async Task<object> UpdateCache()
    {
        var expirationDate = cache.Get<DateTime?>(ExpiryCacheKey) ?? DateTime.UtcNow;

        var current = GetLookup();

        if (expirationDate - DateTime.UtcNow > TimeSpan.FromHours(1))
        {
            return current;
        }
        
        cache.Set(ExpiryCacheKey, DateTime.UtcNow.AddDays(7), CacheEntryOptions);

        var lookup = current != null
            ? await client.UpdateLookup(current)
            : await client.CreateLookup();

        cache.Set(CacheKey, lookup, CacheEntryOptions);

        return lookup;
    }
    
    public void RestoreState(object state)
    {
        cache.Set(CacheKey, state, CacheEntryOptions);
        cache.Set(ExpiryCacheKey, DateTime.UtcNow.AddMinutes(61), CacheEntryOptions);
    }
}