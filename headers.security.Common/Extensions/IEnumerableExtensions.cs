using System.Runtime.InteropServices;

namespace headers.security.Common.Extensions;

public static class EnumerableExtensions
{
    public static DefaultDictionary<T, int> ToCounter<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = default) 
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(source);

        var dictionary = new DefaultDictionary<T, int>(() => 0, comparer);
        foreach (var item in source)
        {
            CollectionsMarshal.GetValueRefOrAddDefault(dictionary, item, out _)++;
        }
        return dictionary;
    }
}