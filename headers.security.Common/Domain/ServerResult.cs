using System.Text.Json.Serialization;

namespace headers.security.Common.Domain;

public class ServerResult
{
    public ScanTarget RequestTarget { get; set; }
    
    public ScanTarget FinalTarget { get; set; }
    
    [JsonPropertyName("ips")]
    // ReSharper disable once InconsistentNaming
    public List<IPWithCloud> IPs { get; set; }

    public ScanResult Result { get; set; }
    
    public SecurityGrade Grade { get; set; }
    
    public ScanError Error { get; set; }
    
    public DateTime FetchedAt { get; set; }
}