namespace headers.security.Common.Domain.SecurityConcepts;

public class SecurityConceptResultInfo : ISecurityConceptResultInfo
{
    public string Message { get; init; }
    public List<List<string>> FormatTokens { get; init; }

    public Uri ExternalLink { get; init; }

    public static SecurityConceptResultInfo Create(string message, Uri externalLink = null) => new()
    {
        Message = message,
        ExternalLink = externalLink
    };
}