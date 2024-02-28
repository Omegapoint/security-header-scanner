using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace headers.security.Scanner.SecurityConcepts;

public partial class ServerSecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = HeaderNames.Server;

    public static ISecurityConcept Create() => new ServerSecurityConcept();

    private const string MessageVersion = 
        "The Server header reveals server software vendor and detailed version information, " +
        "this allows automation of CVE susceptibility scanning and should be disabled. " +
        "Consider removing the header altogether.";

    // private const string MessageVendor =
    //     "The Server header reveals server software vendor, consider removing";

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

        result.MutableValue = string.Join(' ', headers);

        if (headers.Any(header => VersionRegex().IsMatch(header)))
        {
            infos.Add(SecurityConceptResultInfo.Create(MessageVersion));
        }
        // else
        // {
        //     infos.Add(SecurityConceptResultInfo.Create(MessageVendor));
        // }

        
        return result;
    }
    
    [GeneratedRegex(@"[\d]+\.[\d]+")]
    private static partial Regex VersionRegex();
}