using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

// ReSharper disable once UnusedType.Global
public class CacheControlSecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = HeaderNames.CacheControl;

    public Task<ISecurityConceptResult> ExecuteAsync(ScanData scanData) => Task.FromResult(Execute(scanData));

    private ISecurityConceptResult Execute(ScanData scanData)
    {
        var rawHeaders = scanData.RawHeaders;
        
        if (scanData.TargetType == TargetKind.Frontend)
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