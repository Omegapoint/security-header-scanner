using headers.security.Common.Constants;
using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using headers.security.Scanner.SecurityConcepts.Csp;
using static headers.security.Common.Constants.ReferrerPolicy;

namespace headers.security.Scanner.SecurityConcepts;

/// <summary>
/// Specification: https://w3c.github.io/webappsec-referrer-policy/#referrer-policy-header
/// </summary>
public class ReferrerPolicySecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = SecurityHeaderNames.ReferrerPolicy;
    
    // https://w3c.github.io/webappsec-referrer-policy/#referrer-policy-delivery-meta
    public static readonly string MetaName = "referrer";

    public static ISecurityConcept Create() => new ReferrerPolicySecurityConcept();

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
        var values = new List<string>();
        
        if (rawHeaders.TryGetValue(HeaderName, out var headers))
        {
            values.AddRange(headers);
        }
        
        if (rawHttpEquivMetas.TryGetValue(MetaName, out var httpEquivMetas))
        {
            values.AddRange(httpEquivMetas);
        }

        if (values.Count == 0)
        {
            infos.Add(SecurityConceptResultInfo.Create("No policy present, consider adding one."));
            return result;
        }
        
        var csp = CspParser.ExtractAll(rawHeaders, rawHttpEquivMetas);

        // todo: move to CSP instead, check for referrer policy and pop if exists
        if (csp.HasPolicy && csp.Effective.Directives.TryGetValue(CspDirective.Referrer, out _))
        {
            infos.Add(SecurityConceptResultInfo.Create($"The \"{CspDirective.Referrer}\" directive of the CSP header is obsolete and should be removed."));
        }
        
        ParseValue(values, result);

        SetGrade(result);
        
        return result;
    }

    private static void ParseValue(IReadOnlyCollection<string> values, SimpleSecurityConceptResult result)
    {
        if (values.Count > 1)
        {
            // https://w3c.github.io/webappsec-referrer-policy/#example-3966e12b
            result.StringValue = values.Last();

            var message = values.All(header => header.Equals(result.StringValue, StringComparison.OrdinalIgnoreCase))
                ? "Duplicate policies present."
                : "Multiple policies present, using last.";

            result.Infos.Add(SecurityConceptResultInfo.Create(message));
        }
        else if (values.Count == 1)
        {
            result.StringValue = values.Single();
        }
        else
        {
            // this is the default value which browsers will give when not configured according to spec
            // https://w3c.github.io/webappsec-referrer-policy/#default-referrer-policy
            result.StringValue = StrictOriginWhenCrossOrigin;
        }
    }

    private void SetGrade(SimpleSecurityConceptResult result)
    {
        var lowerCaseConfiguration = result.StringValue.ToLowerInvariant();
        var grade = lowerCaseConfiguration switch
        {
            NoReferrer or Origin or StrictOrigin 
                => SecurityImpact.None,
            NoReferrerWhenDowngrade 
                => SecurityImpact.Low,
            UnsafeUrl
                => SecurityImpact.Medium,
            _ => SecurityImpact.None
        };
        result.SetImpact(grade);
    }
}