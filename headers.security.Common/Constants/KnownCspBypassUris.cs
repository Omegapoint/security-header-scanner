using System.Text.RegularExpressions;

namespace headers.security.Common.Constants;

public static partial class KnownCspBypassUris
{
    // TODO: FUTURE: might be better to use full URI here and do more advanced matching, e.g. query parameters can come in any order, this is good enough for now
    public static readonly List<string> All = [
        "google.com/complete/search",
        "youtube.com/oembed"
    ];

    public static bool Matches(string token)
    {
        var cleanedToken = CleanedToken(token);
        
        if (cleanedToken == "*")
        {
            return true;
        }

        return All.Any(bypassToken => cleanedToken.StartsWith("*.")
            ? bypassToken.StartsWith(cleanedToken[2..]) || bypassToken.Split('.').Last().StartsWith(cleanedToken[2..])
            : bypassToken.StartsWith(cleanedToken));
    }
    
    private static string CleanedToken(string token) => ProtocolRegex().Replace(token, "", 1).ToLowerInvariant();
    
    [GeneratedRegex(@"^http[s]{0,1}:\/\/", RegexOptions.IgnoreCase)]
    private static partial Regex ProtocolRegex();
}