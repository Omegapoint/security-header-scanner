using System.Net;
using System.Text.RegularExpressions;

namespace CloudLookup.Providers;

public partial class AzureCloudLookupClientProvider(IHttpClientFactory httpClientFactory)
    : CloudLookupClientProviderBase(httpClientFactory)
{
    private readonly Uri _downloadPage = new("https://www.microsoft.com/en-us/download/confirmation.aspx?id=56519");

    public override async Task<IEnumerable<IPNetwork>> GetNetworks()
    {
        var downloadPageHtml = await FetchContent(_downloadPage);

        var urlMatch = DataUrlRe().Match(downloadPageHtml);

        if (!urlMatch.Success || !Uri.TryCreate(urlMatch.Groups["URI"].Value, UriKind.Absolute, out var contentUri))
        {
            throw new ArgumentException("Failed to locate Azure IP ranges JSON URI.");
        }

        var content = await FetchContent(contentUri);

        var tokens = GenericJsonCloudLookupClientProvider.ExtractTokens(content);
        
        return ExtractNetworks(tokens);
    }

    [GeneratedRegex(
        @"href=""(?<URI>https?://download\.microsoft\.com/[\w\d/\-_]+\.json)""",
        RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase
    )]
    private static partial Regex DataUrlRe();
}