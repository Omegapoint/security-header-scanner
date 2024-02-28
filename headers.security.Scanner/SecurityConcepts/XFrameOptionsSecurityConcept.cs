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

    public static ISecurityConcept Create() => new XFrameOptionsSecurityConcept();

    public Task<ISecurityConceptResult> ExecuteAsync(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message) 
        => Task.FromResult(Execute(rawHeaders, rawHttpEquivMetas, message));
    
    public ISecurityConceptResult Execute(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message)
    {
        var infos = new List<SecurityConceptResultInfo>();
        var result = new SimpleSecurityConceptResult(HeaderName, infos, SecurityImpact.Low);
        
        if (!rawHeaders.TryGetValue(HeaderName, out var headers))
        {
            return result;
        }
        
        // todo: this could be optimised so it only needs to be processed once for all handlers that need it
        // todo: but unclear how to build that without things getting messy
        var cspEffective = CspParser.ExtractAll(rawHeaders, rawHttpEquivMetas).Effective;

        if (cspEffective.Directives.TryGetValue(CspDirective.FrameAncestors, out _))
        {
            // todo: this shouldn't be considering the value of default-src right? double-check
            infos.Add(SecurityConceptResultInfo.Create($"The \"{HeaderName}\" header is obsoleted by the frame-ancestors directive of the CSP, and should be removed."));
        }
        
        if (headers.Count > 1)
        {
            var firstHeader = headers.First();
            if (headers.All(header => header.Equals(firstHeader, StringComparison.OrdinalIgnoreCase)))
            {
                infos.Add(SecurityConceptResultInfo.Create($"Duplicate \"{HeaderName}\" headers present."));
                
                result.MutableValue = firstHeader;
            }
            else
            {
                infos.Add(SecurityConceptResultInfo.Create($"Multiple \"{HeaderName}\" headers present."));
                
                //todo: how to proceed?
            }
        }
        else
        {
            result.MutableValue = headers.Single();
        }
        
        SetGrade(result);
        
        return result;
    }

    private void SetGrade(SimpleSecurityConceptResult result)
    {
        var lowerCaseConfiguration = result.MutableValue.ToLowerInvariant();
        if (lowerCaseConfiguration is "deny" or "sameorigin")
        {
            result.SetImpact(SecurityImpact.None);
        }
    }
}