using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

/// <summary>
/// RFC: https://fetch.spec.whatwg.org/#x-content-type-options-header
/// </summary>
// ReSharper disable once UnusedType.Global
public class XContentTypeOptionsSecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = HeaderNames.XContentTypeOptions;

    public const string NoSniff = "nosniff";

    public Task<ISecurityConceptResult> ExecuteAsync(ScanData scanData) => Task.FromResult(Execute(scanData));

    private ISecurityConceptResult Execute(ScanData scanData)
    {
        var rawHeaders = scanData.RawHeaders;
        
        var infos = new List<ISecurityConceptResultInfo>();
        var result = new SimpleSecurityConceptResult(HeaderName, infos, SecurityImpact.Low);
        
        if (!rawHeaders.TryGetValue(HeaderName, out var headers))
        {
            infos.Add(SecurityConceptResultInfo.Create($"Header missing."));
            return result;
        }
        
        if (headers.Count > 1)
        {
            infos.Add(SecurityConceptResultInfo.Create($"Multiple {HeaderName} headers present."));
            if (headers.All(header => string.Equals(header, headers.First(), StringComparison.OrdinalIgnoreCase)))
            {
                result.StringValue = headers.First();
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
        var lowerCaseConfiguration = result.StringValue.ToLowerInvariant();
        if (lowerCaseConfiguration is NoSniff)
        {
            result.SetImpact(SecurityImpact.None);
        }
        else
        {
            result.Infos.Add(SecurityConceptResultInfo.Create($"{HeaderName} found but its value \"{result.StringValue}\" differs from the expected value \"{NoSniff}\"."));
        }
    }
}