using headers.security.Common.Constants;
using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using headers.security.Scanner.SecurityConcepts.Csp;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

/// <summary>
/// RFC: https://html.spec.whatwg.org/multipage/document-lifecycle.html#the-x-frame-options-header
/// </summary>
public class XFrameOptionsSecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = HeaderNames.XFrameOptions;

    private static readonly List<string> AllValidValues = ["DENY", "ALLOWALL", "SAMEORIGIN"];

    public static ISecurityConcept Create() => new XFrameOptionsSecurityConcept();

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
        var result = new SimpleSecurityConceptResult(HeaderName, infos, SecurityImpact.Low);
        
        var csp = CspParser.ExtractAll(rawHeaders, rawHttpEquivMetas);

        if (csp.HasPolicy && csp.Effective.Directives.TryGetValue(CspDirective.FrameAncestors, out _))
        {
            infos.Add(SecurityConceptResultInfo.Create("Header is obsoleted by the frame-ancestors directive of the CSP, and should be removed."));

            if (csp.IsEnforcing)
            {
                result.SetImpact(SecurityImpact.Info);
                return result;
            }
        }
        
        if (!rawHeaders.TryGetValue(HeaderName, out var headers))
        {
            return result;
        }
        
        if (headers.Count > 1)
        {
            var firstHeader = headers.First();
            if (headers.All(header => header.Equals(firstHeader, StringComparison.OrdinalIgnoreCase)))
            {
                infos.Add(SecurityConceptResultInfo.Create("Duplicate identical policies present."));
                result.StringValue = firstHeader;
            }
            else if (headers.Any(h => AllValidValues.Contains(h, StringComparer.OrdinalIgnoreCase)))
            {
                infos.Add(SecurityConceptResultInfo.Create("Multiple policies present with conflicting values, browser will treat as \"DENY\"."));
                result.StringValue = "DENY";
            }
            else
            {
                infos.Add(SecurityConceptResultInfo.Create("Multiple policies present with no valid values, browser will ignore header and allow from anywhere."));
            }
        }
        else
        {
            result.StringValue = headers.Single();
        }
        
        SetGrade(result);
        
        return result;
    }

    private void SetGrade(SimpleSecurityConceptResult result)
    {
        var effectiveValue = result.StringValue.ToLowerInvariant();
        if (effectiveValue is "deny" or "sameorigin")
        {
            result.SetImpact(SecurityImpact.None);
        }
    }
}