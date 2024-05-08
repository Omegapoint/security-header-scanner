namespace headers.security.Scanner.Hsts;

public class HstsPreloadService(IHstsPreloadRepository repository) : IHstsPreloadService
{
    public bool IsPreloaded(Uri target)
    {
        var entry = repository.GetPreloadEntry(target);

        // Service not initialized
        if (entry == null) return false;

        return entry.Mode != HstsPreloadMatchMode.NotMatched;
    }
}