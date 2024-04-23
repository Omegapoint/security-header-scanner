using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

/// <summary>
/// Implementation of scanner for the HTTP Strict Transport Security concept
/// RFC: https://datatracker.ietf.org/doc/html/rfc6797
/// </summary>
// ReSharper disable once UnusedType.Global
public class StrictTransportSecurityConcept : ISecurityConcept
{
    public const string HandlerName = "HTTP Strict Transport Security";
    
    // These will be compared to a lowercased string as spec specifies case-insensitive directive names
    private const string IncludeSubdomainsToken = "includesubdomains";
    private const string PreloadToken = "preload";
    private const string MaxAgeToken = "max-age";
    
    private const int ShortPolicyCutoff = 15768000; // 6 months

    public Task<ISecurityConceptResult> ExecuteAsync(ScanData scanData) => Task.FromResult(Execute(scanData));

    private ISecurityConceptResult Execute(ScanData scanData)
    {
        var rawHeaders = scanData.RawHeaders;
        
        var infos = new List<ISecurityConceptResultInfo>();
        
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

        if (maxAge < ShortPolicyCutoff)
        {
            infos.Add(SecurityConceptResultInfo.Create("The HSTS policy has a short lifespan, it is recommended to set it to at least 1 year."));
        }
        
        var includeSubdomains = tokens.Contains(IncludeSubdomainsToken);
        var preload = tokens.Contains(PreloadToken);

        // TODO: FUTURE: verify preload status? add info regarding preload
        
        return new StrictTransportSecurityConceptResult(infos, maxAge, includeSubdomains, preload);
    }

    private static int? ExtractMaxAge(IEnumerable<string> tokens)
    {
        var maxAgeToken = tokens.FirstOrDefault(t => t.StartsWith(MaxAgeToken));
        var maxAgeValue = maxAgeToken?.Split('=').LastOrDefault();

        if (int.TryParse(maxAgeValue, out var maxAge))
        {
            return maxAge;
        }

        return null;
    }
}

public class StrictTransportSecurityConceptResult(
    List<ISecurityConceptResultInfo> infos,
    int? maxAge,
    bool includeSubdomains,
    bool preload)
    : AbstractSecurityConceptResult(infos)
{
    private StrictTransportSecurityConceptResult() : this([SecurityConceptResultInfo.Create("Header missing.")], null, false, false)
    {
    }
    
    public static ISecurityConceptResult Missing => new StrictTransportSecurityConceptResult();
    
    private const int OneYear = 31_104_000;
    private const int HalfYear = 15_552_000;

    private int? MaxAge { get; } = maxAge;
    private bool IncludeSubdomains { get; } = includeSubdomains;
    private bool Preload { get; } = preload;

    public override string HandlerName => StrictTransportSecurityConcept.HandlerName;
    public override string HeaderName => HeaderNames.StrictTransportSecurity;

    public override SecurityImpact Impact => GetImpact();

    public override object ProcessedValue => new
    {
        MaxAge, IncludeSubdomains, Preload
    };

    private SecurityImpact GetImpact()
    {
        if (MaxAge is null)
        {
            return SecurityImpact.Medium;
        }
        
        if (MaxAge >= OneYear)
        {
            return SecurityImpact.None;
        }
        else if (MaxAge >= HalfYear)
        {
            return SecurityImpact.Low;
        }
        
        return SecurityImpact.Medium;
    }
}