using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace CloudLookup.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class CloudLookupActualDataFixture
{
    internal (Cloud, IEnumerable<IPNetwork>)[] Networks { get; }

    public CloudLookupActualDataFixture()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        
        var serviceProvider = services.BuildServiceProvider();
        var client = new CloudLookupClient(serviceProvider.GetService<IHttpClientFactory>());

        Networks = client.GetNetworks()
            .GetAwaiter()
            .GetResult();
    }
}