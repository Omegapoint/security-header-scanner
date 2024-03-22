using headers.security.Common.Domain;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.Extensions;

public static class HttpResponseMessageExtensions
{
    public static bool IsRedirectStatusCode(this HttpResponseMessage message)
    {
        return (int) message.StatusCode is >= 300 and < 400;
    }
    
    private static readonly List<string> FrontendDetectionContentTypes = ["text/html"];
    
    public static bool LooksLikeFrontendResponse(this HttpResponseMessage message)
    {
        return (message.Content.Headers.TryGetValues(HeaderNames.ContentType, out var contentTypes) ||
                message.Headers.TryGetValues(HeaderNames.ContentType, out contentTypes)) &&
               contentTypes.Any(c => FrontendDetectionContentTypes.Any(t => c.StartsWith(t, StringComparison.InvariantCultureIgnoreCase)));
    }
    
    public static TargetKind DetectTargetKind(this HttpResponseMessage httpResponse) => 
        httpResponse.LooksLikeFrontendResponse()
            ? TargetKind.Frontend
            : TargetKind.Api;
}