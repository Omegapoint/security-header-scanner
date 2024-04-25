using headers.security.Scanner.Hsts.Contracts;

namespace headers.security.Scanner.Hsts;

public interface IHstsPreloadRepository
{
    PreloadPolicy GetPreloadEntry(Uri target);
}