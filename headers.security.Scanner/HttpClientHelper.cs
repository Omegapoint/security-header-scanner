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
        // TODO: read version from assembly
        httpClient.DefaultRequestHeaders.Add(
            HeaderNames.UserAgent, 
            $"{AppConstants.UserAgentPrefix} v{ApplicationInformation.CompileDate.SimpleDateString()}"
        );
        httpClient.Timeout = TimeSpan.FromSeconds(2);
    }
}