namespace headers.security.Scanner.Hsts;

public class HstsPreloadService(IHstsPreloadRepository repository)
{
    public bool IsPreloaded(Uri target)
    {
        var entry = repository.GetPreloadEntry(target);

        // TODO: Not sure this is correct, maybe mode == force-https?
        return entry != null;
    }
}