using System.Net;

namespace headers.security.Scanner.CloudLookup;

public interface ICloudLookupRepository
{
    string GetCloud(IPAddress address);
}