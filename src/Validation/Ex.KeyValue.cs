namespace ScrubJay.Validation;

partial class Ex
{
    public static KeyNotFoundException KeyNotFound<K>(
        K key,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
#if NET9_0_OR_GREATER
    where K : allows ref struct
#endif
    {
        string message = TextBuilder.New
            .Append($"Key ({typeof(K):@}) `{key}` was not found")
            .AppendInfo(ref info)
            .ToStringAndDispose();
        return new KeyNotFoundException(message, innerException);
    }

    public static ArgumentException DuplicateKey<K>(
        K key,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(key))]
        string? keyName = null)
#if NET9_0_OR_GREATER
    where K : allows ref struct
#endif
    {
        string message = TextBuilder.New
            .Append($"Key \"{keyName}\" ({typeof(K):@}) `{key}` was not found")
            .AppendInfo(ref info)
            .ToStringAndDispose();
        return new ArgumentException(message, innerException);
    }
}