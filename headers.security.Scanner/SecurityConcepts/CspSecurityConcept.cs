using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using headers.security.Scanner.SecurityConcepts.Csp;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

public class CspSecurityConcept : ISecurityConcept
{
    public const string HandlerName = "Content Security Policy";

    public static ISecurityConcept Create() => new CspSecurityConcept();

    public Task<ISecurityConceptResult> ExecuteAsync(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message) 
        => Task.FromResult(Execute(rawHeaders, rawHttpEquivMetas, message));
    
    public ISecurityConceptResult Execute(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message)
    {
        var infos = new List<SecurityConceptResultInfo>();

        var configuration = CspParser.ExtractAll(rawHeaders, rawHttpEquivMetas);

        if (configuration.All.Count > 1)
        {
            infos.Add(SecurityConceptResultInfo.Create("Multiple Content Security policies present."));
        }

        if (configuration.All.Count == 0 && configuration.AllNonEnforcing.Count >= 1)
        {
            infos.Add(SecurityConceptResultInfo.Create("Content Security Policy is not enforcing."));
        }

        return new CspSecurityConceptResult(configuration, infos);
    }
}

public class CspSecurityConceptResult(CspConfiguration configuration, List<SecurityConceptResultInfo> infos) : AbstractSecurityConceptResult(infos)
{
    public override string HandlerName => CspSecurityConcept.HandlerName;
    public override string HeaderName => HeaderNames.ContentSecurityPolicy;

    public override SecurityImpact Impact => GetImpact();
    public override CspConfiguration ProcessedValue => configuration;
    
    public bool NonceReuse { get; set; }

    private SecurityImpact GetImpact()
    {
        if (!configuration.HasPolicy)
        {
            return SecurityImpact.Medium;
        }

        if (NonceReuse)
        {
            return SecurityImpact.Critical;
        }
        
        //TODO: more complex logic for grading?

        return SecurityImpact.None;
    }
}