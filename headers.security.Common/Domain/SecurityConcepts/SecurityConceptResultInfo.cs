namespace headers.security.Common.Domain.SecurityConcepts;

public class SecurityConceptResultInfo : ISecurityConceptResultInfo
{
    public string Message { get; init; }
    public List<List<string>> FormatTokens { get; init; }

    public Uri ExternalLink { get; init; }

    public static SecurityConceptResultInfo Create(FormattableString message) => new()
    {
        Message = message.Format,
        FormatTokens = message.GetArguments()
            .Select(argument => new List<string> { argument?.ToString() ?? "" })
            .ToList()
    };

    public static SecurityConceptResultInfo Create(StringDifferentiator message) => new()
    {
        Message = message.Value
    };
}
