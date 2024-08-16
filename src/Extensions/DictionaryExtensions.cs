namespace ScrubJay.Extensions;

public static class DictionaryExtensions
{
#if NET48_OR_GREATER || NETSTANDARD2_0
    public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
            return false;
        dictionary[key] = value;
        return true;
    }
#endif
}