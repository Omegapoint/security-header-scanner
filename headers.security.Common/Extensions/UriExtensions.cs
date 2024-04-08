using System.Net;

namespace headers.security.Common.Extensions;

public static class UriExtensions
{
    // https://www.ietf.org/archive/id/draft-chapin-rfc2606bis-00.html#rfc.section.2
    private static readonly List<string> InternalHosts = ["localhost"];
    private static readonly List<string> ForbiddenTlds =
        ["corp", "domain", "example", "home", "host", "invalid", "lan", "local", "localdomain"];
    
    public static bool IsInternal(this Uri uri)
    {
        var host = uri.Host.ToLowerInvariant();
        
        if (InternalHosts.Contains(host))
        {
            return true;
        }

        if (IPAddress.TryParse(host, out var hostIp) && hostIp.IsPrivate())
        {
            return true;
        }

        var tokens = host.Split('.');
        switch (tokens.Length)
        {
            case 2:
                var singleDotTld = tokens.Last();
                if (ForbiddenTlds.Contains(singleDotTld))
                {
                    return true;
                }
                break;
            case 1:
                return true;
        }

        return false;
    }

    public static Uri IdnNormalized(this Uri uri)
    {
        var uriBuilder = new UriBuilder(uri)
        {
            Host = uri.IdnHost
        };

        if (uriBuilder.Uri.IsDefaultPort)
        {
            uriBuilder.Port = -1;
        }
        
        return uriBuilder.Uri;
    }

    private static Uri WithIpHost(this Uri uri, IPAddress ipAddress) =>
        new UriBuilder(uri)
        {
            Host = ipAddress.ToString()
        }.Uri;

    public static HttpRequestMessage MakeRequestToIp(this Uri uri, IPAddress ipAddress, HttpMethod method = null) =>
        new(method ?? HttpMethod.Get, uri.WithIpHost(ipAddress))
        {
            Headers =
            {
                Host = uri.IdnHost
            }
        };

    public static Uri SetBaseUri(this Uri relativeUri, Uri baseUri)
    {
        if (Uri.TryCreate(baseUri, relativeUri, out var absoluteUri))
        {
            return absoluteUri;
        }

        return relativeUri.IsAbsoluteUri ? relativeUri : null;
    }
}