namespace headers.security.Common;

public class DefaultDictionary<TKey, TValue>(Func<TValue> init, IEqualityComparer<TKey> comparer = default)
    : Dictionary<TKey, TValue>(comparer)
{
    public new TValue this[TKey k] {
        get {
            if (!ContainsKey(k))
                Add(k,init());
            return base[k];
        }
        set => base[k] = value;
    }	
}