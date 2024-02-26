namespace headers.security.Scanner;

public class WorkerConfiguration
{
    // ReSharper disable once InconsistentNaming
    public bool IPv6Enabled { get; set; }
    
    public bool AllowInternalTargets { get; set; }

    public int MaxHttpRequestDepth { get; set; } = 20;
}