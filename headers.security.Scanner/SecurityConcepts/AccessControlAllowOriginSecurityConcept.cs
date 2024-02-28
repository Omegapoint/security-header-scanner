using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

public class AccessControlAllowOriginSecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = HeaderNames.AccessControlAllowOrigin;

    public static ISecurityConcept Create() => new AccessControlAllowOriginSecurityConcept();

    private const string Warning = "The Access-Control-Allow-Origin header allows cross-origin requests without restrictions, this is generally only advisable for content distribution networks.";

    public Task<ISecurityConceptResult> ExecuteAsync(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message) 
        => Task.FromResult(Execute(rawHeaders, rawHttpEquivMetas, message));
    
    public ISecurityConceptResult Execute(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage httpMessage)
    {
        var infos = new List<SecurityConceptResultInfo>();
        var result = new SimpleSecurityConceptResult(HeaderName, infos);

        result.SetGrade(SecurityGrade.NonInfluencing);
        
        if (!rawHeaders.TryGetValue(HeaderName, out var headers) || string.IsNullOrWhiteSpace(headers.First()))
        {
            return null;
        }
        
        var firstHeader = headers.First();
        
        if (firstHeader.Trim() == "*")
        {
            infos.Add(SecurityConceptResultInfo.Create(Warning));
        }
        
        return result;
    }
}