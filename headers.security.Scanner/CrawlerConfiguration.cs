using System.Runtime.Serialization;

namespace headers.security.Scanner;

// TODO: add configuration support for things like authorization (should perform test both with and without)
// TODO: should also support setting method verb to use, per line if using file
[DataContract]
public class CrawlerConfiguration
{
    public bool FollowRedirects { get; set; }
    
    public CancellationToken CancellationToken { get; set; }
}