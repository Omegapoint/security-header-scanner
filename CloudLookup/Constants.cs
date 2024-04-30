// ReSharper disable InconsistentNaming
namespace CloudLookup;

public static class Constants
{
    public const string AWSEndpoint = "https://ip-ranges.amazonaws.com/ip-ranges.json";
    public const string GCPEndpoint = "https://www.gstatic.com/ipranges/cloud.json";
    public const string OracleEndpoint = "https://docs.cloud.oracle.com/en-us/iaas/tools/public_ip_ranges.json";
    public const string CloudflareIPv4Endpoint = "https://www.cloudflare.com/ips-v4";
    public const string CloudflareIPv6Endpoint = "https://www.cloudflare.com/ips-v6";
    
    public const string DigitalOceanEndpoint = "https://digitalocean.com/geo/google.csv";
}