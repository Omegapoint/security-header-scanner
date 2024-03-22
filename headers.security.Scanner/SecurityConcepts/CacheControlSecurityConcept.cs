using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

public class CacheControlSecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = HeaderNames.CacheControl;

    public static ISecurityConcept Create() => new CacheControlSecurityConcept();

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
        if (crawlerConf.GetTargetKind(message) == TargetKind.Frontend)
        {
            // cache-control configuration is not relevant from a security header perspective for pure frontends
            return null;
        }
        
        var infos = new List<ISecurityConceptResultInfo>();
        var result = new SimpleSecurityConceptResult(HeaderName, infos);

        rawHeaders.TryGetValue(HeaderName, out var headers);
        
        var firstHeader = headers?.FirstOrDefault()?.ToLowerInvariant().Trim();
        
        if (firstHeader != "no-store")
        {
            infos.Add(SecurityConceptResultInfo.Create("APIs may return sensitive information and should not be cached."));
            result.SetImpact(SecurityImpact.Medium);
        }
        
        return result;
    }
}