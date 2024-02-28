namespace headers.security.Common.Extensions;

public static class HttpResponseMessageExtensions
{
    public static bool IsRedirectStatusCode(this HttpResponseMessage message)
    {
        return (int) message.StatusCode is >= 300 and < 400;
    }
}