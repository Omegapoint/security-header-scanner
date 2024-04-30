using System.Net;

namespace CloudLookup.Providers;

public interface ICloudLookupClientProvider
{
    Task<IEnumerable<IPNetwork>> GetNetworks();
}