namespace headers.security.Tests;

public static class EnumerableExtensions
{
    public static IEnumerable<object[]> FirstMemberAsArray<T>(this IEnumerable<T> source)
        where T : IEnumerable<object> =>
        source.Select(r => new[] { r.First() });
}