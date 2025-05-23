using System.Net.Http.Headers;
using headers.security.Common.Constants;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.Extensions;

public static class HttpRequestHeadersExtensions
{
    // Chrome 122 is used because it doesn't by default support zstd, which isn't implemented in HttpClient as of 2024-05-02
    private const string ChromiumUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36";

    /// <summary>
    /// Clears existing headers and sets those used by Chromium version 122 running on x64 Windows
    /// </summary>
    public static void EmulateChromium(this HttpRequestHeaders headers)
    {
        headers.Clear();
        
        headers.Add(HeaderNames.UserAgent,                      ChromiumUserAgent);
        headers.Add(HeaderNames.Accept,                         "text/html");
        headers.Add(HeaderNames.AcceptEncoding,                 "gzip, deflate, br");
        headers.Add(HeaderNames.AcceptLanguage,                 "en-US,en;q=0.9");
        headers.Add(HeaderNames.CacheControl,                   "no-cache");
        headers.Add(HeaderNames.Pragma,                         "no-cache");
        headers.Add(ChromiumHeaderNames.SecChUa,                "\"Chromium\";v=\"122\", \"Not-A/Brand\";v=\"99\", \"Google Chrome\";v=\"122\"");
        headers.Add(ChromiumHeaderNames.SecChUaArch,            "\"x86\"");
        headers.Add(ChromiumHeaderNames.SecChUaBitness,         "\"64\"");
        headers.Add(ChromiumHeaderNames.SecChUaFullVersion,     "\"122.0.6261.57\"");
        headers.Add(ChromiumHeaderNames.SecChUaFullVersionList, "\"Chromium\";v=\"122.0.6261.57\", \"Not-A/Brand\";v=\"99.0.0.0\", \"Google Chrome\";v=\"122.0.6261.57\"");
        headers.Add(ChromiumHeaderNames.SecChUaMobile,          "?0");
        headers.Add(ChromiumHeaderNames.SecChUaModel,           "\"\"");
        headers.Add(ChromiumHeaderNames.SecChUaPlatform,        "\"Windows\"");
        headers.Add(ChromiumHeaderNames.SecChUaPlatformVersion, "\"10.0.0\"");
        headers.Add(ChromiumHeaderNames.SecChUaWow64,           "?0");
        headers.Add(ChromiumHeaderNames.SecFetchDest,           "document");
        headers.Add(ChromiumHeaderNames.SecFetchMode,           "navigate");
        headers.Add(ChromiumHeaderNames.SecFetchSite,           "same-origin");
        headers.Add(ChromiumHeaderNames.SecFetchUser,           "?1");
        headers.Add(HeaderNames.UpgradeInsecureRequests,        "1");
    }
}