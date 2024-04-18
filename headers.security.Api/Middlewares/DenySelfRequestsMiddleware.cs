using System.Net;
using headers.security.Common;
using headers.security.Common.Constants;
using headers.security.Common.Domain;
using Microsoft.Net.Http.Headers;

namespace headers.security.Api.Middlewares;

/// <summary>
/// In case any other attempts to stop expensive scan loops fail,
/// deny any incoming request with our known User-Agent
/// </summary>
public class DenySelfRequestsMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderNames.UserAgent, out var userAgentValues) 
            && userAgentValues.Any(v => v.StartsWith(AppConstants.UserAgentPrefix)))
        {
            context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
            context.Response.Headers[HeaderNames.CacheControl] = "no-store";
            await context.Response.WriteAsJsonAsync(new ScanError
            {
                Message = ErrorMessages.SelfScan,
                Origin = ErrorOrigin.SystemLimitation
            });
            return;
        }
        
        await next.Invoke(context);
    }
}

public static class DenySelfRequestsMiddlewareExtensions
{
    public static void DenySelfRequests(this IApplicationBuilder builder) 
        => builder.UseMiddleware<DenySelfRequestsMiddleware>();
}