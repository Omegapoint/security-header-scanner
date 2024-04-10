namespace headers.security.Common.Constants;

public static class AppConstants
{
    public const string UserAgentPrefix = "headers.security scanner";
    public static readonly Uri Referrer = new("https://securityheaders.omegapoint.se");
    public const string XAppIdentifierHeader = "X-App-Identifier";
}