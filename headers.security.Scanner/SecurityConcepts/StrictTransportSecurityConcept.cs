using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using headers.security.Common.Extensions;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

/// <summary>
/// Implementation of scanner for the HTTP Strict Transport Security concept
/// RFC: https://datatracker.ietf.org/doc/html/rfc6797
/// </summary>
public class StrictTransportSecurityConcept : ISecurityConcept
{
    public const string HandlerName = "HTTP Strict Transport Security";
    
    // These will be compared to a lowercased string as spec specifies case-insensitive directive names
    private const string IncludeSubdomains = "includesubdomains";
    private const string Preload = "preload";
    
    public static ISecurityConcept Create() => new StrictTransportSecurityConcept();

    public Task<ISecurityConceptResult> ExecuteAsync(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message) 
        => Task.FromResult(Execute(rawHeaders, rawHttpEquivMetas, message));
    
    public ISecurityConceptResult Execute(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message)
    {
        var infos = new List<SecurityConceptResultInfo>();
        
        if (!rawHeaders.TryGetValue(HeaderNames.StrictTransportSecurity, out var hstsHeaders))
        {
            return StrictTransportSecurityConceptResult.Missing;
        }
        
        if (hstsHeaders.Count > 1)
        {
            infos.Add(SecurityConceptResultInfo.Create("Multiple HSTS headers present, using first."));
        }

        var tokens = hstsHeaders.First()
            .ToLowerInvariant()
            .Split(';')
            .Select(t => t.Trim())
            .ToList();

        var maxAge = ExtractMaxAge(tokens);
        var includeSubdomains = tokens.Any(t => t.Equals(IncludeSubdomains));
        var preload = tokens.Any(t => t.Equals(Preload));

        // TODO: verify preload status?
        // TODO: add info regarding preload
        
        return new StrictTransportSecurityConceptResult(infos, maxAge, includeSubdomains, preload);
    }

    private static int? ExtractMaxAge(IEnumerable<string> tokens)
    {
        var maxAgeToken = tokens.FirstOrDefault(t => t.StartsWith("max-age"));
        var maxAgeValue = maxAgeToken?.Split('=').LastOrDefault();

        if (int.TryParse(maxAgeValue, out var maxAge))
        {
            return maxAge;
        }

        return null;
    }
}

/// <summary>
/// TODO: document why 1 year
/// </summary>
public class StrictTransportSecurityConceptResult : AbstractSecurityConceptResult
{
    private static readonly int OneYear = TimeSpan.FromDays(365).Seconds;
    private static readonly int HalfYear = TimeSpan.FromDays(180).Seconds;
    
    public static ISecurityConceptResult Missing => new StrictTransportSecurityConceptResult();

    private int? MaxAge { get; init; }
    private bool IncludeSubdomains { get; init; }
    private bool Preload { get; init; }

    public override string HandlerName => StrictTransportSecurityConcept.HandlerName;
    public override string HeaderName => HeaderNames.StrictTransportSecurity;

    public override SecurityGrade Grade => GetGrade();

    public override object ProcessedValue => new
    {
        MaxAge, IncludeSubdomains, Preload
    };

    private StrictTransportSecurityConceptResult() : base([SecurityConceptResultInfo.Create("HSTS header missing.")])
    {
    }

    public StrictTransportSecurityConceptResult(List<SecurityConceptResultInfo> infos, int? maxAge, bool includeSubdomains, bool preload) : base(infos)
    {
        MaxAge = maxAge;
        IncludeSubdomains = includeSubdomains;
        Preload = preload;
    }

    private SecurityGrade GetGrade()
    {
        if (MaxAge is null)
        {
            return SecurityGrade.F;
        }
        
        if (MaxAge >= OneYear)
        {
            return SecurityGrade.A.Lowered(!IncludeSubdomains);
        }
        else if (MaxAge >= HalfYear)
        {
            return SecurityGrade.B.Lowered(!IncludeSubdomains);
        }
        
        // TODO: decide how this is handled

        return SecurityGrade.F;
    }
}