using headers.security.Common.Constants;
using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using headers.security.Scanner.SecurityConcepts.Csp;
using static headers.security.Common.Constants.ReferrerPolicy;

namespace headers.security.Scanner.SecurityConcepts;

public class ReferrerPolicySecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = SecurityHeaderNames.ReferrerPolicy;

    public static ISecurityConcept Create() => new ReferrerPolicySecurityConcept();

    public Task<ISecurityConceptResult> ExecuteAsync(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message) 
        => Task.FromResult(Execute(rawHeaders, rawHttpEquivMetas, message));
    
    public ISecurityConceptResult Execute(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message)
    {
        var infos = new List<SecurityConceptResultInfo>();
        var result = new SimpleSecurityConceptResult(HeaderName, infos, SecurityGrade.NonInfluencing);
        
        if (!rawHeaders.TryGetValue(HeaderName, out var headers))
        {
            return result;
        }
        
        // todo: this could be optimised so it only needs to be processed once for all handlers that need it
        // todo: but unclear how to build that without things getting messy
        var cspEffective = CspParser.ExtractAll(rawHeaders, rawHttpEquivMetas).Effective;

        if (cspEffective.Directives.TryGetValue(CspDirective.Referrer, out _))
        {
            // todo: this shouldn't be considering the value of default-src right? double-check
            infos.Add(SecurityConceptResultInfo.Create($"The \"{CspDirective.Referrer}\" directive of the CSP header is obsolete and should be removed."));
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
        else if (headers.Count == 1)
        {
            result.MutableValue = headers.Single();
        }
        else
        {
            // this is the default value which browsers will give when not configured
            result.MutableValue = StrictOriginWhenCrossOrigin;
        }

        SetGrade(result);
        
        return result;
    }

    private void SetGrade(SimpleSecurityConceptResult result)
    {
        var lowerCaseConfiguration = result.MutableValue.ToLowerInvariant();
        var grade = lowerCaseConfiguration switch
        {
            NoReferrer or Origin or StrictOrigin 
                => SecurityGrade.A,
            NoReferrerWhenDowngrade 
                => SecurityGrade.C,
            UnsafeUrl
                => SecurityGrade.D,
            _ => SecurityGrade.NonInfluencing
        };
        result.SetGrade(grade);
    }
}