using headers.security.Scanner.Hsts;
using headers.security.Scanner.Hsts.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace headers.security.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class HstsPreloadActualDataFixture
{
    internal List<PreloadEntry> PreloadEntries { get; }

    public HstsPreloadActualDataFixture()
    {
        // if (HstsPreloadActualDataTests.SkipReason != null)
        // {
        //     return;
        // }
        
        var services = new ServiceCollection();
        services.AddHttpClient();
        
        var serviceProvider = services.BuildServiceProvider();
        var client = new HstsPreloadClient(serviceProvider.GetService<IHttpClientFactory>());

        PreloadEntries = client.GetPreloadEntries()
            .GetAwaiter()
            .GetResult();
    }
}