namespace headers.security.Common.Domain.SecurityConcepts;

public class CspNonceSecurityConceptResultInfo : ISecurityConceptResultInfo
{
    public string Message { get; init; }
    public List<List<string>> FormatTokens { get; init; }
    public Uri ExternalLink { get; init; } = new("https://w3c.github.io/webappsec-csp/#security-nonces");

    public static CspNonceSecurityConceptResultInfo Reuse(HashSet<string> nonces)
    {
        var noncePart = nonces.Count > 1 ? "nonces seem" : "nonce seems";
            
        return new()
        {
            Message = $"The following {noncePart} to be re-used: {{0}}. A nonce that is re-used can be predicted by an attacker and renders the CSP useless.",
            FormatTokens = [nonces.ToList()]
        };
    }

    public static CspNonceSecurityConceptResultInfo LowComplexity(string nonce) =>
        new()
        {
            Message = "The nonce value {0} is of lower than recommended complexity, nonces should be at least 16 characters long.",
            FormatTokens = [[nonce]]
        };
}