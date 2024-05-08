using headers.security.Scanner.Hsts;
using headers.security.Scanner.Hsts.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace headers.security.CachedContent;

public class CachingHstsPreloadRepository(HstsPreloadClient client, IMemoryCache cache) : IHstsPreloadRepository, ICachedContentRepository
{
    public string CacheKey => "HSTSPreloadTree";
    public string ExpiryCacheKey => CacheKey + "/ExpiryTime";
    public string StateFilename => "hstspreload";
    public Type Type => typeof(PreloadPolicyNode);
    
    private static readonly MemoryCacheEntryOptions CacheEntryOptions = new()
    {
        Priority = CacheItemPriority.NeverRemove
    };
    
    public PreloadMatch GetPreloadEntry(Uri target) => GetTree()?.GetOrDefault(target);

    private PreloadPolicyNode GetTree()
    {
        cache.TryGetValue<PreloadPolicyNode>(CacheKey, out var tree);

        return tree;
    }

    public async Task<object> UpdateCache()
    {
        var expirationDate = cache.Get<DateTime?>(ExpiryCacheKey) ?? DateTime.UtcNow;

        if (expirationDate - DateTime.UtcNow > TimeSpan.FromHours(1))
        {
            return GetTree();
        }
        
        cache.Set(ExpiryCacheKey, DateTime.UtcNow.AddHours(24), CacheEntryOptions);

        var entries = await client.GetPreloadEntries();
        var tree = PreloadPolicyNode.Create(entries);

        cache.Set(CacheKey, tree, CacheEntryOptions);

        return tree;
    }

    public void RestoreState(object state)
    {
        cache.Set(CacheKey, state, CacheEntryOptions);
        cache.Set(ExpiryCacheKey, DateTime.UtcNow.AddMinutes(61), CacheEntryOptions);
    }
}