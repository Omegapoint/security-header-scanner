using CloudLookup.Extensions;
using headers.security.CachedContent.Services;
using headers.security.Scanner.CloudLookup;
using headers.security.Scanner.Hsts;
using Microsoft.Extensions.DependencyInjection;

namespace headers.security.CachedContent.Extensions;

public static class ServiceCollectionExtensions
{
    private static void AddHstsPreloadFeature(this IServiceCollection services)
    {
        services.AddSingleton<HstsPreloadClient>();
        services.AddSingleton<CachingHstsPreloadRepository>();
        services.AddSingleton<IHstsPreloadRepository>(s => s.GetService<CachingHstsPreloadRepository>());
        services.AddSingleton<ICachedContentRepository>(s => s.GetService<CachingHstsPreloadRepository>());
        services.AddSingleton<IHstsPreloadService, HstsPreloadService>();
    }
    
    private static void AddCloudLookupFeature(this IServiceCollection services)
    {
        services.AddCloudLookupClient();
        services.AddSingleton<CachingCloudLookupRepository>();
        services.AddSingleton<ICloudLookupRepository>(s => s.GetService<CachingCloudLookupRepository>());
        services.AddSingleton<ICachedContentRepository>(s => s.GetService<CachingCloudLookupRepository>());
        services.AddSingleton<CloudLookupService>();
    }
    
    public static void AddCachedContent(this IServiceCollection services, bool useBackgroundService = true)
    {
        services.AddHttpClient("Integration");
        
        services.AddHstsPreloadFeature();
        services.AddCloudLookupFeature();
        
        if (useBackgroundService)
        {
            services.AddHostedService<CachedContentBackgroundUpdateService>();
        }
    }
}