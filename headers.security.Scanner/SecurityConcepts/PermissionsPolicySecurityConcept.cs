using headers.security.Common.Constants;
using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;

namespace headers.security.Scanner.SecurityConcepts;

/// <summary>
/// Implementation of evaluator for Permissions-Policy header
/// RFC: https://w3c.github.io/webappsec-permissions-policy/#permissions-policy-http-header-field
/// </summary>
public class PermissionsPolicySecurityConcept : ISecurityConcept
{
    // Old name, warn if encountered
    public const string DeprecatedHeaderName = "Feature-Policy";
    
    public const string HeaderName = SecurityHeaderNames.PermissionsPolicy;

    public static ISecurityConcept Create() => new PermissionsPolicySecurityConcept();

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
        
        var result = new SimpleSecurityConceptResult(HeaderName, infos, SecurityImpact.Info);
        
        if (rawHeaders.TryGetValue(DeprecatedHeaderName, out _))
        {
            infos.Add(SecurityConceptResultInfo.Create($"The \"{DeprecatedHeaderName}\" header name is deprecated and should not be used."));
        }
        
        if (!rawHeaders.TryGetValue(HeaderName, out var policyHeaders))
        {
            return result;
        }
        
        result.SetImpact(SecurityImpact.None);
        result.StringValue = string.Join(", ", policyHeaders);

        if (policyHeaders.Count > 1)
        {
            infos.Add(SecurityConceptResultInfo.Create("Multiple policies present."));
        }
        
        //TODO: if we have detailed information about the application being scanned we can do better analysis, future work

        return result;
    }
}