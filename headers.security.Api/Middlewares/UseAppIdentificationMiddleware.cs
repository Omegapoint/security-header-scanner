using headers.security.Common.Constants;

namespace headers.security.Api.Middlewares;

public class UseAppIdentificationMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers[AppConstants.XAppIdentifierHeader] = AppConstants.UserAgentPrefix;
        
        await next.Invoke(context);
    }
}

public static class UseAppIdentificationMiddlewareExtensions
{
    public static void UseAppIdentification(this IApplicationBuilder builder) 
        => builder.UseMiddleware<UseAppIdentificationMiddleware>();
}