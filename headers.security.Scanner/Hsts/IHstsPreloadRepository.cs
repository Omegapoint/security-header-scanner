using headers.security.Scanner.Hsts.Contracts;

namespace headers.security.Scanner.Hsts;

public interface IHstsPreloadRepository
{
    PreloadMatch GetPreloadEntry(Uri target);
}