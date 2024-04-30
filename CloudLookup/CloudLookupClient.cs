using System.Net;
using CloudLookup.Providers;
using static CloudLookup.Constants;

namespace CloudLookup;

public class CloudLookupClient(IHttpClientFactory httpClientFactory)
{
    private readonly Dictionary<Cloud, ICloudLookupClientProvider> _providers = new()
    {
        {
            Cloud.Azure,
            new AzureCloudLookupClientProvider(httpClientFactory)
        },
        {
            Cloud.AWS,
            new GenericJsonCloudLookupClientProvider(httpClientFactory, AWSEndpoint)
        },
        {
            Cloud.GCP,
            new GenericJsonCloudLookupClientProvider(httpClientFactory, GCPEndpoint)
        },
        {
            Cloud.Oracle,
            new GenericJsonCloudLookupClientProvider(httpClientFactory, OracleEndpoint)
        },
        {
            Cloud.Cloudflare,
            new GenericTextCloudLookupClientProvider(httpClientFactory, CloudflareIPv4Endpoint, CloudflareIPv6Endpoint)
        },
        {
            Cloud.DigitalOcean,
            new GenericCsvCloudLookupClientProvider(httpClientFactory, header: false, column: 0, DigitalOceanEndpoint)
        },
    };

    public async Task<CloudLookupCollection> CreateLookup()
    {
        var tuples = await GetNetworks();

        return new CloudLookupCollection(tuples);
    }

    public async Task<CloudLookupCollection> UpdateLookup(CloudLookupCollection current)
    {
        var latest = await GetNetworks();

        var data = new List<(Cloud Key, IEnumerable<IPNetwork> Networks)>();
        
        foreach (var (cloud, networks) in latest)
        {
            data.Add((cloud, networks ?? current.GetNetworks(cloud)));
        }

        return new CloudLookupCollection(data);
    }

    public async Task<(Cloud Key, IEnumerable<IPNetwork> Networks)[]> GetNetworks() =>
        await Task.WhenAll(
            _providers.Select(async kvp =>
            {
                try
                {
                    return (kvp.Key, await kvp.Value.GetNetworks());
                }
                catch (Exception)
                {
                    return (kvp.Key, null);
                }
            })
        );
}