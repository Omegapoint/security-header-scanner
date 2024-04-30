using System.Net;

namespace CloudLookup.Providers;

public class GenericCsvCloudLookupClientProvider(IHttpClientFactory httpClientFactory, string target, bool header, int? column = null)
    : CloudLookupClientProviderBase(httpClientFactory)
{
    private readonly Uri _target = new(target);

    public override async Task<IEnumerable<IPNetwork>> GetNetworks()
    {
        var content = await FetchContent(_target);

        var tokens = content
            .Split('\n')
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