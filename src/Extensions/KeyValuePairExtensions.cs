namespace ScrubJay.Extensions;

[PublicAPI]
public static class KeyValuePairExtensions
{
#if NETFRAMEWORK || NETSTANDARD2_0
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair, out TKey key, out TValue value)
    {
        key = keyValuePair.Key;
        value = keyValuePair.Value;
    }
#endif
}