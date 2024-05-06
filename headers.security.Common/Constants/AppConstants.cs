namespace headers.security.Common.Constants;

public static class AppConstants
{
    public const string AppIdentifier = "headers.security";
    // TODO: Should probably get this value from incoming user request Host value
    public static readonly Uri Referrer = new("https://headers.omegapoint.app");
    public const string XAppIdentifierHeader = "X-App-Identifier";
}