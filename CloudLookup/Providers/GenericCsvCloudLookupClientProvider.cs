using System.Net;

namespace CloudLookup.Providers;

public class GenericCsvCloudLookupClientProvider(IHttpClientFactory httpClientFactory, bool header, int? column = null, params string[] targets)
    : CloudLookupClientProviderBase(httpClientFactory)
{
    public override async Task<IEnumerable<IPNetwork>> GetNetworks()
    {
        var content = await Task.WhenAll(targets.Select(FetchContent));

        var tokens = content.SelectMany(data => data.Split('\n'))
            .Skip(header ? 1 : 0)
            .SelectMany(line =>
            {
                var lineTokens = line.Split(',');
                
                return column == null
                    ? lineTokens
                    : lineTokens.Skip((int) column).Take(1);
            });

        return ExtractNetworks(tokens);
    }
}