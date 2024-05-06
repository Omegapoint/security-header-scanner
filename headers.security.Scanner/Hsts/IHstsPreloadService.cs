namespace headers.security.Scanner.Hsts;

public interface IHstsPreloadService
{
    bool IsPreloaded(Uri target);
}