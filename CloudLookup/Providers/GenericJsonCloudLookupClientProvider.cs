using System.Net;
using System.Text.RegularExpressions;

namespace CloudLookup.Providers;

public partial class GenericJsonCloudLookupClientProvider(IHttpClientFactory httpClientFactory, string target)
    : CloudLookupClientProviderBase(httpClientFactory)
{
    private readonly Uri _target = new(target);

    public override async Task<IEnumerable<IPNetwork>> GetNetworks()
    {
        var content = await FetchContent(_target);

        var tokens = ExtractTokens(content);

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