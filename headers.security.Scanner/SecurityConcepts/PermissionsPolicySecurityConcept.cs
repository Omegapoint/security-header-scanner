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

    public Task<ISecurityConceptResult> ExecuteAsync(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message) 
        => Task.FromResult(Execute(rawHeaders, rawHttpEquivMetas, message));
    
    public ISecurityConceptResult Execute(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message)
    {
        var infos = new List<SecurityConceptResultInfo>();
        
        var result = new SimpleSecurityConceptResult(HeaderName, infos);
        
        if (rawHeaders.TryGetValue(DeprecatedHeaderName, out _))
        {
            infos.Add(SecurityConceptResultInfo.Create($"The \"{DeprecatedHeaderName}\" header name is deprecated and should not be used."));
        }
        
        if (!rawHeaders.TryGetValue(HeaderName, out var policyHeaders))
        {
            //TODO: how bad is not declaring permissions-policy?
            result.SetGrade(SecurityGrade.B);
            
            return result;
        }
        
        result.SetGrade(SecurityGrade.NonInfluencing);
        //TODO: how handle multiple? merge like with CSP?
        result.MutableValue = string.Join(", ", policyHeaders);

        if (policyHeaders.Count > 1)
        {
            infos.Add(SecurityConceptResultInfo.Create($"Multiple \"{HeaderName}\" headers present."));
        }
        
        //TODO: do some analysis of the declared header

        return result;
    }
}