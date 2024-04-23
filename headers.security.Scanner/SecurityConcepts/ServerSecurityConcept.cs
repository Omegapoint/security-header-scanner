using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace headers.security.Scanner.SecurityConcepts;

// ReSharper disable once UnusedType.Global
public partial class ServerSecurityConcept : ISecurityConcept
{
    public static readonly string HeaderName = HeaderNames.Server;

    private const string MessageVersion = 
        "The Server header reveals server software vendor and detailed version information, " +
        "this allows automation of CVE susceptibility scanning and should be disabled. " +
        "Consider removing the header altogether.";

    public Task<ISecurityConceptResult> ExecuteAsync(ScanData scanData) => Task.FromResult(Execute(scanData));

    private ISecurityConceptResult Execute(ScanData scanData)
    {
        var rawHeaders = scanData.RawHeaders;
        
        var infos = new List<ISecurityConceptResultInfo>();
        var result = new SimpleSecurityConceptResult(HeaderName, infos);
        
        if (!rawHeaders.TryGetValue(HeaderName, out var headers) || string.IsNullOrWhiteSpace(headers.First()))
        {
            return null;
        }

        result.StringValue = string.Join(' ', headers);

        if (headers.Any(header => VersionRegex().IsMatch(header)))
        {
            result.SetImpact(SecurityImpact.Info);
            infos.Add(SecurityConceptResultInfo.Create(MessageVersion));
        }
        
        return result;
    }
    
    [GeneratedRegex(@"[\d]+\.[\d]+")]
    private static partial Regex VersionRegex();
}