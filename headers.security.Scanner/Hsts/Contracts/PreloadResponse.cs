using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace headers.security.Scanner.Hsts.Contracts;

public class PreloadResponse
{
    public List<PreloadEntry> Entries { get; init; }
}

public class PreloadEntry(string domain) : PreloadPolicy
{
    [JsonPropertyName("name")]
    public string Domain { get; } = domain;
    
    public ImmutableArray<string> TreePath = domain?.ToLowerInvariant().Split('.').Reverse().ToImmutableArray() ?? [];
}

public class PreloadPolicy
{
    public string Policy { get; init; }
    [JsonPropertyName("include_subdomains")]
    public bool IncludeSubdomains { get; init; }
    public string Mode { get; init; }
}

public class PreloadMatch
{
    public PreloadPolicy Policy { get; init; }
    
    public HstsPreloadMatchMode Mode { get; init; }

    public static PreloadMatch Subdomain(PreloadPolicy policy) => new()
        { Policy = policy, Mode = HstsPreloadMatchMode.Subdomain };

    public static PreloadMatch Exact(PreloadPolicy policy) => new()
        { Policy = policy, Mode = HstsPreloadMatchMode.Exact };

    public static PreloadMatch NotMatched() => new()
        { Mode = HstsPreloadMatchMode.NotMatched };
}