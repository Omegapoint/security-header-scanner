using headers.security.Scanner;
using Microsoft.Extensions.Caching.Memory;

namespace headers.security.Frontend.Caching;

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public class CachingCrawler(Crawler crawler, IMemoryCache cache) : ICrawler
{
    public async Task<List<CrawlerResponse>> Crawl(Uri uri, CrawlerConfiguration crawlerConf)
    {
        return await cache.GetOrCreateAsync(
            new { uri, crawlerConf.FollowRedirects },
            cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(1);
                return crawler.Crawl(uri, crawlerConf);
            });
    }
}