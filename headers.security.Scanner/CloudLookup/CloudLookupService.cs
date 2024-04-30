using System.Net;

namespace headers.security.Scanner.CloudLookup;

public class CloudLookupService(ICloudLookupRepository repository)
{
    public string GetCloud(IPAddress address) => repository.GetCloud(address);
}