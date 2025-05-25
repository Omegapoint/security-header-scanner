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

        var matches = DataUrlRe().Matches(downloadPageHtml);
        var urls = matches
            .Select(match => Uri.TryCreate(match.Groups["URI"].Value, UriKind.Absolute, out var uri) 
                ? uri 
                : null)
            .Where(u => u != null)
            .ToList();

        if (!urls.Any())
        {
            throw new ArgumentException("Failed to locate Azure IP ranges JSON URI.");
        }

        var contentUri = GetLatest(urls);
        var content = await FetchContent(contentUri);

        var tokens = GenericJsonCloudLookupClientProvider.ExtractTokens(content);
        
        return ExtractNetworks(tokens);
    }

    private Uri GetLatest(List<Uri> uris)
    {
        var latest = uris.First();
        foreach (var uri in uris.Skip(1))
        {
            if (IsNewer(uri, latest))
            {
                latest = uri;
            } 
            
        }

        return latest;
    }

    private bool IsNewer(Uri a, Uri b) => string.Compare(a.Segments.Last(), b.Segments.Last(), StringComparison.OrdinalIgnoreCase) > 0;

    [GeneratedRegex(
        @"""(?<URI>https?://download\.microsoft\.com/[\w\d/\-_]+\.json)""",
        RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase
    )]
    private static partial Regex DataUrlRe();
}