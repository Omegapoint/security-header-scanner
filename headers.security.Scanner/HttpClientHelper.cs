using System.Diagnostics;
using System.Net;
using System.Net.Security;
using headers.security.Common;
using headers.security.Common.Constants;
using headers.security.Common.Extensions;
using headers.security.Scanner.Configuration;
using headers.security.Scanner.Extensions;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner;

public static class HttpClientHelper
{
    public static void ConfigureClient(HttpClient httpClient) => ConfigureClient(httpClient, null);
    
    public static void ConfigureClient(HttpClient httpClient, HttpClientConfiguration configuration)
    {
        httpClient.DefaultRequestHeaders.EmulateChromium();
        
        var appIdentifier = $"{AppConstants.AppIdentifier}/{ApplicationInformation.CompileDate.VersionDateString()}";
        httpClient.DefaultRequestHeaders.Add(HeaderNames.DNT, "1");
        httpClient.DefaultRequestHeaders.Add(AppConstants.XAppIdentifierHeader, appIdentifier);
        
        if (!string.IsNullOrEmpty(configuration?.Referrer))
        {
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, configuration.Referrer);
        }
        
        httpClient.Timeout = TimeSpan.FromSeconds(25);
    }

    public static SocketsHttpHandler ConfigureHandler(IServiceProvider _) => new()
    {
        AllowAutoRedirect = false,
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
        ActivityHeadersPropagator = DistributedContextPropagator.CreateNoOutputPropagator(),
        SslOptions = new SslClientAuthenticationOptions
        {
            RemoteCertificateValidationCallback = delegate { return true; }
        }
    };
}