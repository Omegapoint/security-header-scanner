using headers.security.Common.Constants.Http;
using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using static headers.security.Common.Constants.ReferrerPolicyValue;

namespace headers.security.Scanner.SecurityConcepts;

/// <summary>
/// Specification: https://w3c.github.io/webappsec-referrer-policy/#referrer-policy-header
/// </summary>
// ReSharper disable once UnusedType.Global
public class ReferrerPolicySecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = SecurityHeaderNames.ReferrerPolicy;
    
    // https://w3c.github.io/webappsec-referrer-policy/#referrer-policy-delivery-meta
    public static readonly string MetaName = "referrer";

    public Task<ISecurityConceptResult> ExecuteAsync(ScanData scanData) => Task.FromResult(Execute(scanData));

    private ISecurityConceptResult Execute(ScanData scanData)
    {
        var rawHeaders = scanData.RawHeaders;
        var rawHttpEquivMetas = scanData.RawHttpEquivMetas;
        
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
        
        ParseValue(values, result);

        SetImpact(result);
        
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

    private void SetImpact(SimpleSecurityConceptResult result)
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