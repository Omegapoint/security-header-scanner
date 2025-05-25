using headers.security.Common.Constants;
using headers.security.Scanner.SecurityConcepts;
using static Microsoft.Net.Http.Headers.HeaderNames;
using static headers.security.Common.Constants.Http.SecurityHeaderNames;

namespace headers.security.Api.Middlewares;

public class UseSecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers[PermissionsPolicy] = SitePermissionsPolicy;
        context.Response.Headers[ReferrerPolicy] = ReferrerPolicyValue.StrictOrigin;
        context.Response.Headers[XContentTypeOptions] = XContentTypeOptionsSecurityConcept.NoSniff;
        context.Response.Headers[ContentSecurityPolicy] = SiteCspConfiguration.Policy;

        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.Headers[CacheControl] = "no-store";
        }
        
        await next.Invoke(context);
    }

    private static string SitePermissionsPolicy =>
       string.Join(", ", PermissionsPolicyDirective.SensitiveDirectives.Select(directive => $"{directive}=()"));
}

public static class UseSecurityHeadersMiddlewareExtensions
{
    public static void UseSecurityHeaders(this IApplicationBuilder builder) 
        => builder.UseMiddleware<UseSecurityHeadersMiddleware>();
}