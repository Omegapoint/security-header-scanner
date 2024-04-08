using System.Text.RegularExpressions;

namespace headers.security.Common.Constants;

public static partial class KnownCspBypassUris
{
    // TODO: FUTURE: might be better to use full URI here and do more advanced matching, e.g. query parameters can come in any order, this is good enough for now
    private static readonly List<string> All = [
        "google.com/complete/search",
        "youtube.com/oembed"
    ];

    public static bool Matches(string token)
    {
        if (token == "*")
        {
            return true;
        }
        
        var cleanedToken = CleanedToken(token);

        return All.Any(bypassToken => cleanedToken.StartsWith("*.")
            ? bypassToken.EndsWith(cleanedToken[2..])
            : bypassToken.StartsWith(cleanedToken));
    }
    
    private static string CleanedToken(string token) => ProtocolRegex().Replace(token, "").ToLowerInvariant();
    
    [GeneratedRegex(@"http[s]{0,1}:\/\/")]
    private static partial Regex ProtocolRegex();
}