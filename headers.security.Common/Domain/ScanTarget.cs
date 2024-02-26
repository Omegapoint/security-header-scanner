using System.Globalization;

namespace headers.security.Common.Domain;

public class ScanTarget
{
    private static readonly IdnMapping IdnMapping = new();
    
    public string UtfDomain { get; set; }
    
    public string AsciiDomain { get; set; }
    
    public string Path { get; set; }
    
    public int Port { get; set; }
    
    public string Scheme { get; set; }
    
    public bool IsDefaultPort { get; set; }

    public static ScanTarget From(Uri uri) => new()
    {
        UtfDomain = IdnMapping.GetUnicode(uri.Host),
        AsciiDomain = uri.IdnHost,
        Path = uri.PathAndQuery,
        Port = uri.Port,
        Scheme = uri.Scheme,
        IsDefaultPort = uri.IsDefaultPort
    };
}