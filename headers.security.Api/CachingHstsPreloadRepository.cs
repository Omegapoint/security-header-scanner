using headers.security.Scanner.Hsts;
using headers.security.Scanner.Hsts.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace headers.security.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public class CachingHstsPreloadRepository(HstsPreloadClient client, IMemoryCache cache) : IHstsPreloadRepository
{
    private static readonly MemoryCacheEntryOptions CacheEntryOptions = new()
    {
        Priority = CacheItemPriority.NeverRemove
    };

    public const string CacheKey = "HSTSPreloadTree";
    public const string ExpiryCacheKey = "HSTSPreloadTree/ExpiryTime";
    
    public PreloadPolicy GetPreloadEntry(Uri target) => GetTree()?.GetOrDefault(target?.Host);

    private PreloadPolicyNode GetTree()
    {
        cache.TryGetValue<PreloadPolicyNode>(CacheKey, out var tree);

        return tree;
    }

    public async Task<PreloadPolicyNode> UpdatePreloadCache()
    {
        var expirationDate = cache.Get<DateTime?>(ExpiryCacheKey);

        if (expirationDate != null && expirationDate - DateTime.UtcNow > TimeSpan.FromHours(1))
        {
            return GetTree();
        }
        
        cache.Set(ExpiryCacheKey, DateTime.UtcNow.AddHours(24), CacheEntryOptions);

        var entries = await client.GetPreloadEntries();
        var tree = PreloadPolicyNode.Create(entries);

        cache.Set(CacheKey, tree, CacheEntryOptions);

        return tree;
    }

    public void RestoreState(PreloadPolicyNode tree)
    {
        cache.Set(CacheKey, tree, CacheEntryOptions);
        cache.Set(ExpiryCacheKey, DateTime.UtcNow.AddMinutes(61), CacheEntryOptions);
    }
}