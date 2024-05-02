using System.Net.Http.Headers;
using headers.security.Common.Constants;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.Extensions;

public static class HttpRequestHeadersExtensions
{
    // Chrome 122 is used because it doesn't by default support zstd, which isn't implemented in HttpClient as of 2024-05-02
    private const string Chromium122UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36";
    private const string Chromium122Accept =
        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7";

    /// <summary>
    /// Clears existing headers and sets those used by Chromium version 122 running on x64 Windows
    /// </summary>
    public static void EmulateChromium122(this HttpRequestHeaders headers)
    {
        headers.Clear();
        
        headers.Add(HeaderNames.Accept, Chromium122Accept);
        headers.Add(HeaderNames.AcceptEncoding, "gzip, deflate, br");
        headers.Add(HeaderNames.AcceptLanguage, "en-US,en;q=0.9");
        headers.Add(HeaderNames.CacheControl, "no-cache");
        headers.Add(HeaderNames.Connection, "keep-alive");
        headers.Add(HeaderNames.Pragma, "no-cache");
        headers.Add(ChromiumHeaderNames.SecFetchDest, "document");
        headers.Add(ChromiumHeaderNames.SecFetchMode, "navigate");
        headers.Add(ChromiumHeaderNames.SecFetchSite, "none");
        headers.Add(ChromiumHeaderNames.SecFetchUser, "?1");
        headers.Add(HeaderNames.UpgradeInsecureRequests, "1");
        headers.Add(HeaderNames.UserAgent, Chromium122UserAgent);
        headers.Add(ChromiumHeaderNames.SecChUa, "\"Chromium\";v=\"124\", \"Not-A.Brand\";v=\"99\"");
        headers.Add(ChromiumHeaderNames.SecChUaMobile, "?0");
        headers.Add(ChromiumHeaderNames.SecChUaPlatform, "\"Windows\"");
    }
}