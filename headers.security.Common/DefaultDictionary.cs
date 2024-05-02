namespace headers.security.Common;

/// <summary>
/// A dictionary which always returns a value.
/// </summary>
/// <param name="init">The initializer which defines the default value.</param>
/// <param name="comparer">A comparer to configure key lookup behaviour.</param>
public class DefaultDictionary<TKey, TValue>(Func<TValue> init, IEqualityComparer<TKey> comparer = default)
    : Dictionary<TKey, TValue>(comparer)
{
    public new TValue this[TKey key]
    {
        get
        {
            if (!ContainsKey(key))
            {
                Add(key, init());
            }

            return base[key];
        }
        set => base[key] = value;
    }
}