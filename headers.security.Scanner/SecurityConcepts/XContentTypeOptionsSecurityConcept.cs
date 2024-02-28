using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

/// <summary>
/// RFC: https://fetch.spec.whatwg.org/#x-content-type-options-header
/// </summary>
public class XContentTypeOptionsSecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = HeaderNames.XContentTypeOptions;

    public static ISecurityConcept Create() => new XContentTypeOptionsSecurityConcept();

    public Task<ISecurityConceptResult> ExecuteAsync(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message) 
        => Task.FromResult(Execute(rawHeaders, rawHttpEquivMetas, message));
    
    public ISecurityConceptResult Execute(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message)
    {
        var infos = new List<SecurityConceptResultInfo>();
        var result = new SimpleSecurityConceptResult(HeaderName, infos);
        
        if (!rawHeaders.TryGetValue(HeaderName, out var headers))
        {
            return result;
        }
        
        if (headers.Count > 1)
        {
            infos.Add(SecurityConceptResultInfo.Create($"Multiple {HeaderName} headers present."));
            if (headers.All(header => header.ToLowerInvariant() == headers.First()))
            {
                result.MutableValue = headers.First();
            }
        }
        else
        {
            result.MutableValue = headers.Single();
        }

        if (!string.IsNullOrWhiteSpace(result.MutableValue))
        {
            SetGrade(result);
        }
        
        return result;
    }

    private void SetGrade(SimpleSecurityConceptResult result)
    {
        var lowerCaseConfiguration = result.MutableValue.ToLowerInvariant();
        if (lowerCaseConfiguration is "nosniff")
        {
            result.SetGrade(SecurityGrade.A);
        }
    }
}