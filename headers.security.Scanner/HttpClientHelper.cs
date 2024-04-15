using headers.security.Common;
using headers.security.Common.Constants;
using headers.security.Common.Extensions;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner;

public static class HttpClientHelper
{
    public static void ConfigureClient(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add(
            HeaderNames.UserAgent, 
            $"{AppConstants.UserAgentPrefix} v{ApplicationInformation.CompileDate.VersionDateString()}"
        );
        httpClient.DefaultRequestHeaders.Add(
            HeaderNames.Referer,
            AppConstants.Referrer.ToString());
        httpClient.DefaultRequestHeaders.Add(
            HeaderNames.Accept,
            "*/*");
        httpClient.Timeout = TimeSpan.FromSeconds(25);
    }
}