using headers.security.Common.Constants;
using headers.security.Common.Domain;
using headers.security.Common.Extensions;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.Extensions;

public static class HttpResponseMessageExtensions
{
    public static bool IsRedirectStatusCode(this HttpResponseMessage message)
    {
        return (int) message.StatusCode is >= 300 and < 400;
    }
    
    public static Uri GetAbsoluteRedirectUri(this HttpResponseMessage message, Uri baseUri)
    {
        var nextUri = message.Headers.Location;
        if (nextUri?.IsAbsoluteUri == false)
        {
            nextUri = nextUri.SetBaseUri(baseUri);
        }

        return nextUri;
    }
    
    public static bool IsResponseFromSelf(this HttpResponseMessage message) =>
        message.Headers.TryGetValues(AppConstants.XAppIdentifierHeader, out var appIdentifier) 
        && appIdentifier.Any(id => id.Equals(AppConstants.AppIdentifier));

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