using System.Net;

namespace CloudLookup.Providers;

public class GenericTextCloudLookupClientProvider(IHttpClientFactory httpClientFactory, params string[] targets)
    : CloudLookupClientProviderBase(httpClientFactory)
{
    public override async Task<IEnumerable<IPNetwork>> GetNetworks()
    {
        var content = await Task.WhenAll(targets.Select(FetchContent));

        var tokens = content.SelectMany(data => data.Split('\n'));

        return ExtractNetworks(tokens);
    }
}