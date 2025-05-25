namespace headers.security.CachedContent.Data;

public interface ICachedContentRepository
{
    string CacheKey { get; }
    string ExpiryCacheKey { get; }
    string StateFilename { get; }
    Type Type { get; }

    void RestoreState(object state);
    Task<object> UpdateCache();
}