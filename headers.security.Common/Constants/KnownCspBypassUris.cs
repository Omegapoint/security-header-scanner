using System.Text.RegularExpressions;

namespace headers.security.Common.Constants;

public static partial class KnownCspBypassUris
{
    public static readonly List<string> Uris = [
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

        return Uris.Any(bypassToken => cleanedToken.StartsWith("*.")
            ? bypassToken.StartsWith(cleanedToken[2..]) || bypassToken.Split('.').Last().StartsWith(cleanedToken[2..])
            : bypassToken.StartsWith(cleanedToken));
    }
    
    private static string CleanedToken(string token) => ProtocolRegex().Replace(token, "", 1).ToLowerInvariant();
    
    [GeneratedRegex(@"^http[s]{0,1}:\/\/", RegexOptions.IgnoreCase)]
    private static partial Regex ProtocolRegex();
}