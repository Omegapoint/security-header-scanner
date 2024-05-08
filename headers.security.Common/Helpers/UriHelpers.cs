using System.Collections.Immutable;

namespace headers.security.Common.Helpers;

public static class UriHelpers
{
    public static ImmutableArray<string> SplitDomainParts(string domain, bool reversed = false)
    {
        if (string.IsNullOrWhiteSpace(domain))
            throw new ArgumentException("Invalid string domain supplied");

        var parts = domain.ToLowerInvariant().Split('.');

        return (reversed ? parts.Reverse() : parts).ToImmutableArray();
    }
}