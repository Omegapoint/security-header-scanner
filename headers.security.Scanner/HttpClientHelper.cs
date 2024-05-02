using System.Net;
using headers.security.Common;
using headers.security.Common.Constants;
using headers.security.Common.Extensions;
using headers.security.Scanner.Extensions;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner;

public static class HttpClientHelper
{
    public static void ConfigureClient(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.EmulateChromium122();
        
        var appIdentifier = $"{AppConstants.AppIdentifier}/{ApplicationInformation.CompileDate.VersionDateString()}";
        httpClient.DefaultRequestHeaders.Add(AppConstants.XAppIdentifierHeader, appIdentifier);
        httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, AppConstants.Referrer.ToString());
        
        httpClient.Timeout = TimeSpan.FromSeconds(25);
    }

    public static HttpClientHandler ConfigureHandler(IServiceProvider _) => new()
    {
        AllowAutoRedirect = false,
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    };
}