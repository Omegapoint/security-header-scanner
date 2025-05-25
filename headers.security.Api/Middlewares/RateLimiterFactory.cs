using headers.security.Common.Domain;
using Microsoft.AspNetCore.RateLimiting;

namespace headers.security.Api.Middlewares;

public static class RateLimiterFactory
{
    public const string PolicyName = "fixed";

    public static void ConfigureOptions(RateLimiterOptions options)
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.OnRejected = async (context, cancellationToken) =>
        {
            await context.HttpContext.Response.WriteAsJsonAsync(new ScanError
            {
                Message = "Too many requests, please wait a while before performing further scans.",
                Origin = ErrorOrigin.Client
            }, cancellationToken);
        };
        options.AddFixedWindowLimiter(PolicyName, limiter =>
        {
            limiter.PermitLimit = 4;
            limiter.Window = TimeSpan.FromSeconds(12);
        });
    }
}