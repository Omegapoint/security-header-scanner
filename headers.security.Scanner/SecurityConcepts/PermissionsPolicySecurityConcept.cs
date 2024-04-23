using headers.security.Common.Constants;
using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;

namespace headers.security.Scanner.SecurityConcepts;

/// <summary>
/// Implementation of evaluator for Permissions-Policy header
/// RFC: https://w3c.github.io/webappsec-permissions-policy/#permissions-policy-http-header-field
/// </summary>
// ReSharper disable once UnusedType.Global
public class PermissionsPolicySecurityConcept : ISecurityConcept
{
    // Old name, warn if encountered
    public const string DeprecatedHeaderName = "Feature-Policy";
    
    public const string HeaderName = SecurityHeaderNames.PermissionsPolicy;

    public Task<ISecurityConceptResult> ExecuteAsync(ScanData scanData) => Task.FromResult(Execute(scanData));

    private ISecurityConceptResult Execute(ScanData scanData)
    {
        var rawHeaders = scanData.RawHeaders;
        
        var infos = new List<ISecurityConceptResultInfo>();
        
        var result = new SimpleSecurityConceptResult(HeaderName, infos, SecurityImpact.Info);
        
        if (rawHeaders.TryGetValue(DeprecatedHeaderName, out _))
        {
            infos.Add(SecurityConceptResultInfo.Create($"The {DeprecatedHeaderName} header name is deprecated and should not be used."));
        }
        
        if (!rawHeaders.TryGetValue(HeaderName, out var policyHeaders))
        {
            infos.Add(SecurityConceptResultInfo.Create("No policy present, consider adding one."));
            return result;
        }
        
        result.SetImpact(SecurityImpact.None);
        result.StringValue = string.Join(", ", policyHeaders);

        if (policyHeaders.Count > 1)
        {
            infos.Add(SecurityConceptResultInfo.Create("Multiple policies present."));
        }
        
        //TODO: FUTURE: if we have detailed information about the application being scanned we can do better analysis

        return result;
    }
}