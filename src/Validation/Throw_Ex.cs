#pragma warning disable CA1716

namespace ScrubJay.Validation;

static partial class Throw
{
    [DoesNotReturn]
    public static void IsReadOnly<T>(
        T? instance,
        string? info = null,
        [CallerArgumentExpression(nameof(instance))]
        string? instanceName = null)
    {
        string message = TextBuilder.New
            .Append($"{typeof(T):@} `{instanceName}` is read-only")
            .IfNotNull(info, static (tb, nfo) => tb.Append($": {nfo}"))
            .ToStringAndDispose();
        throw new NotSupportedException(message);
    }

    [DoesNotReturn]
    public static R IsReadOnly<T, R>(
        T? instance,
        string? info = null,
        [CallerArgumentExpression(nameof(instance))]
        string? instanceName = null)
    {
        IsReadOnly<T>(instance, info, instanceName);
        return default!;
    }


    [DoesNotReturn]
    public static void InvalidEnum<E>(
        E @enum,
        string? info = null,
        [CallerArgumentExpression(nameof(@enum))]
        string? enumName = null)
        where E : struct, Enum
        => throw Ex.Enum(@enum, info, enumName);

    [DoesNotReturn]
    public static R InvalidEnum<E, R>(
        E @enum,
        string? info = null,
        [CallerArgumentExpression(nameof(@enum))]
        string? enumName = null)
        where E : struct, Enum
    {
        InvalidEnum<E>(@enum, info, enumName);
        return default!;
    }

    [DoesNotReturn]
    public static void NotSupported(
        string? info = null,
        [CallerMemberName] string? callerName = null)
    {
        string message = TextBuilder.New
            .Append($"{callerName} is not supported")
            .IfNotNull(info, static (tb, nfo) => tb.Append($": {nfo}"))
            .ToStringAndDispose();

        throw new NotSupportedException(message);
    }

    [DoesNotReturn]
    public static R NotSupported<R>(
        string? info = null,
        [CallerMemberName] string? callerName = null)
    {
        NotSupported(info, callerName);
        return default!;
    }

    [DoesNotReturn]
    public static void KeyNotFound<K>(
        K key,
        string? info = null)
    {
        string message = TextBuilder.New
            .Append($"Key `{key}` was not found")
            .IfNotNull(info, static (tb, nfo) => tb.Append($": {nfo}"))
            .ToStringAndDispose();
        throw new KeyNotFoundException(message);
    }

    [DoesNotReturn]
    public static R KeyNotFound<K, R>(
        K key,
        string? info = null)
    {
        KeyNotFound<K>(key, info);
        return default!;
    }

    [DoesNotReturn]
    public static void DuplicateKeyException<K>(
        K key,
        string? info = null,
        [CallerMemberName] string? keyName = null)
    {
        string message = TextBuilder.New
            .Append($"Duplicate Key `{key}` was found")
            .IfNotNull(info, static (tb, nfo) => tb.Append($": {nfo}"))
            .ToStringAndDispose();
        throw Ex.Arg(key, message, null, keyName);
    }

    [DoesNotReturn]
    public static R DuplicateKeyException<K, R>(
        K key,
        string? info = null,
        [CallerMemberName] string? keyName = null)
    {
        DuplicateKeyException<K>(key, info, keyName);
        return default!;
    }
}