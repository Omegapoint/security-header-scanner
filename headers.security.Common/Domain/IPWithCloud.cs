using System.Net;
using System.Text.Json.Serialization;

namespace headers.security.Common.Domain;

public record IPWithCloud
{
    [JsonPropertyName("ip")]
    public string IP { get; set; }
    
    public string Cloud { get; set; }

    public static IPWithCloud Create(string ip, string cloud) => new() { IP = ip, Cloud = cloud };
    public static IPWithCloud Create(IPAddress ip, string cloud) => new() { IP = ip.ToString(), Cloud = cloud };
}