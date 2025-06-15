using ScrubJay.Text.Rendering;

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
        TextBuffer buffer = stackalloc char[256];
        buffer.Write(typeof(T).Render());
        buffer.Write(" `");
        buffer.Write(instance);
        buffer.Write("` is read-only");
        if (info is not null)
        {
            buffer.Write(": ");
            buffer.Write(info);
        }

        string message = buffer.ToStringAndDispose();
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
        => throw InvalidEnumException.Create<E>(@enum, info, enumName);

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
        TextBuffer buffer = stackalloc char[256];
        buffer.Write(callerName);
        buffer.Write(" is not supported");
        if (info is not null)
        {
            buffer.Write(": ");
            buffer.Write(info);
        }

        string message = buffer.ToStringAndDispose();
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
        TextBuffer buffer = stackalloc char[256];
        buffer.Write("Key `");
        buffer.Write(key);
        buffer.Write("` was not found");
        if (info is not null)
        {
            buffer.Write(": ");
            buffer.Write(info);
        }

        string message = buffer.ToStringAndDispose();
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
        TextBuffer buffer = stackalloc char[256];
        buffer.Write("Duplicate Key `");
        buffer.Write(key);
        buffer.Write("` was found");
        if (info is not null)
        {
            buffer.Write(": ");
            buffer.Write(info);
        }

        string message = buffer.ToStringAndDispose();
        throw new ArgumentException(message, keyName);
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