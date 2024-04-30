using System.Net;

namespace CloudLookup.Providers;

public abstract class CloudLookupClientProviderBase(IHttpClientFactory httpClientFactory) : ICloudLookupClientProvider
{
    public abstract Task<IEnumerable<IPNetwork>> GetNetworks();
    
    protected async Task<string> FetchContent(Uri uri)
    {
        using var httpClient = httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to fetch data");
        }

        return await response.Content.ReadAsStringAsync();
    }

    protected HashSet<IPNetwork> ExtractNetworks(IEnumerable<string> tokens)
    {
        var result = new HashSet<IPNetwork>();

        foreach (var token in tokens)
        {
            // Longest possible IPv6 CIDR is 43 characters long
            if (token.Length > 43) continue;
            
            if (IPNetwork.TryParse(token, out var network))
            {
                result.Add(network);
            }
        }
        
        return result;
    }
}