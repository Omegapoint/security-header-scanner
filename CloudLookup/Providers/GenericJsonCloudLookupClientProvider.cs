using System.Net;
using System.Text.RegularExpressions;

namespace CloudLookup.Providers;

public partial class GenericJsonCloudLookupClientProvider(IHttpClientFactory httpClientFactory, params string[] targets)
    : CloudLookupClientProviderBase(httpClientFactory)
{
    public override async Task<IEnumerable<IPNetwork>> GetNetworks()
    {
        var pages = await Task.WhenAll(targets.Select(FetchContent));

        var tokens = pages.SelectMany(ExtractTokens);

        return ExtractNetworks(tokens);
    }

    public static IEnumerable<string> ExtractTokens(string content) =>
        CidrRe().Matches(content)
            .Select(m => m.Groups["IPNetwork"].Value)
            .ToHashSet();

    [GeneratedRegex(
        @"""(?<IPNetwork>[\da-f:.]+\/[\d]*)""",
        RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase
    )]
    private static partial Regex CidrRe();
}