using headers.security.Common;
using headers.security.Common.Domain;

namespace headers.security.Scanner;

public abstract class ScannerException(string message) : ArgumentException(message)
{
    private readonly string _message = message;
    
    public abstract ErrorOrigin Origin { get; }

    public ScanError ToContract() => new()
    {
        Message = _message,
        Origin = Origin
    };
}

public class GenericScannerException(string message, ErrorOrigin origin) : ScannerException(message)
{
    public override ErrorOrigin Origin { get; } = origin;
};

public class DnsResolutionException(string message) : ScannerException(message)
{
    public override ErrorOrigin Origin => ErrorOrigin.Target;
};

public class InternalTargetException(string message) : ScannerException(message)
{
    public override ErrorOrigin Origin => ErrorOrigin.Target;
}