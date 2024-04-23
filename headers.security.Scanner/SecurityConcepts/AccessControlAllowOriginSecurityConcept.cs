using headers.security.Common.Constants;
using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

// ReSharper disable once UnusedType.Global
public class AccessControlAllowOriginSecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = HeaderNames.AccessControlAllowOrigin;

    private const string Warning = "The policy allows cross-origin requests without restrictions, this is generally only advisable for content distribution networks.";
    private static readonly string WarningReferrer = $"The policy includes the value of the {HeaderNames.Referer} header in the request, effectively allowing cross-origin requests without restrictions.";

    public Task<ISecurityConceptResult> ExecuteAsync(
        CrawlerConfiguration crawlerConf,
        RawHeaders rawHeaders,
        RawHeaders rawHttpEquivMetas,
        HttpResponseMessage message) 
        => Task.FromResult(Execute(crawlerConf, rawHeaders, rawHttpEquivMetas, message));

    private ISecurityConceptResult Execute(
        CrawlerConfiguration crawlerConf,
        RawHeaders rawHeaders,
        RawHeaders rawHttpEquivMetas,
        HttpResponseMessage message)
    {
        var infos = new List<ISecurityConceptResultInfo>();
        var result = new SimpleSecurityConceptResult(HeaderName, infos);
        
        if (!rawHeaders.TryGetValue(HeaderName, out var headers) || string.IsNullOrWhiteSpace(headers.First()))
        {
            return null;
        }
        
        var firstHeader = headers.First();
        
        if (firstHeader.Trim() == "*")
        {
            infos.Add(SecurityConceptResultInfo.Create(Warning));
        }

        if (firstHeader.Trim().Contains(AppConstants.Referrer.Host, StringComparison.OrdinalIgnoreCase))
        {
            infos.Add(SecurityConceptResultInfo.Create(WarningReferrer));
        }
        
        return result;
    }
}