using System.Runtime.Serialization;
using headers.security.Common.Domain;

namespace headers.security.Api.Contracts;

[DataContract]
public class ScanResultsContract
{
    public static ScanResultsContract From(DateTime scanStart, ScanRequestContract req, List<ServerResult> results) =>
        new()
        {
            Request = req,
            Results = results,
            ScanStart = scanStart,
            ScanFinish = DateTime.UtcNow,
        };

    public ScanRequestContract Request { get; set; }

    public List<ServerResult> Results { get; set; }

    public DateTime ScanStart { get; set; }

    public DateTime ScanFinish { get; set; }
}