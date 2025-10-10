namespace ScrubJay.Exceptions;

[PublicAPI]
[StackTraceHidden]
public static class Ex
{
    public static InvalidOperationException Invalid(ref InterpolatedTextBuilder interpolatedMessage)
    {
        string? message = interpolatedMessage.ToStringAndDispose();
        return new InvalidOperationException(message);
    }

    public static InvalidOperationException Invalid(string? message = default)
    {
        return new InvalidOperationException(message);
    }

    public static NotImplementedException NotImplemented(ref InterpolatedTextBuilder interpolatedMessage)
    {
        string? message = interpolatedMessage.ToStringAndDispose();
        return new NotImplementedException(message);
    }

    public static NotImplementedException NotImplemented(string? message = default)
    {
        return new NotImplementedException(message);
    }

    public static UnreachableException Unreachable(InterpolatedTextBuilder interpolatedMessage = default)
    {
        return new UnreachableException(interpolatedMessage.ToStringAndDispose());
    }


    public static ArgException Arg(object? argument,
        string? info = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return ArgException.New(argument, info, innerException, argumentName);
    }

    public static ArgException Arg(object? argument,
        ref InterpolatedTextBuilder interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return ArgException.New(argument, ref interpolatedInfo, innerException, argumentName);
    }

    public static ArgException Arg<T>(object? argument,
        string? info = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return ArgException.New<T>(argument, info, innerException, argumentName);
    }

    public static ArgException Arg<T>(object? argument,
        ref InterpolatedTextBuilder interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return ArgException.New<T>(argument, ref interpolatedInfo, innerException, argumentName);
    }

    public static ArgException Arg<T>(T? argument,
        string? info = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return ArgException.New<T>(argument, info, innerException, argumentName);
    }

    public static ArgException Arg<T>(T? argument,
        ref InterpolatedTextBuilder interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return ArgException.New<T>(argument, ref interpolatedInfo, innerException, argumentName);
    }

    // special ability to deal with Span<T> and ReadOnlySpan<T>
    public static ArgException Arg<T>(scoped Span<T> argument,
        string? info = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
#if NET9_0_OR_GREATER
        return ArgException.New<Span<T>>(argument.ToArray(), info, innerException, argumentName);
#else
        return ArgException.New(argument.ToArray(), typeof(Span<T>), info, innerException, argumentName);
#endif
    }

    public static ArgException Arg<T>(scoped Span<T> argument,
        ref InterpolatedTextBuilder interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
#if NET9_0_OR_GREATER
        return ArgException.New<Span<T>>(argument.ToArray(), ref interpolatedInfo, innerException, argumentName);
#else
        return ArgException.New(argument.ToArray(), typeof(Span<T>), ref interpolatedInfo, innerException, argumentName);
#endif
    }

    public static ArgException Arg<T>(scoped ReadOnlySpan<T> argument,
        string? info = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
#if NET9_0_OR_GREATER
        return ArgException.New<ReadOnlySpan<T>>(argument.ToArray(), info, innerException, argumentName);
#else
        return ArgException.New(argument.ToArray(), typeof(ReadOnlySpan<T>), info, innerException, argumentName);
#endif
    }

    public static ArgException Arg<T>(scoped ReadOnlySpan<T> argument,
        ref InterpolatedTextBuilder interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
#if NET9_0_OR_GREATER
        return ArgException.New<ReadOnlySpan<T>>(argument.ToArray(), ref interpolatedInfo, innerException, argumentName);
#else
        return ArgException.New(argument.ToArray(), typeof(ReadOnlySpan<T>), ref interpolatedInfo, innerException, argumentName);
#endif
    }


    public static ArgumentNullException ArgNull<T>(T? argument,
        ref InterpolatedTextBuilder interpolatedMessage,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        string message = interpolatedMessage.ToStringAndDispose();
        return new ArgumentNullException(argumentName, message);
    }

    public static ArgumentNullException ArgNull<T>(T? argument,
        string? message = default,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgumentNullException(argumentName, message);
    }


    public static ArgumentOutOfRangeException ArgRange<T>(T? argument,
        ref InterpolatedTextBuilder interpolatedMessage,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        string? message = interpolatedMessage.ToStringAndDispose();
        return new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static ArgumentOutOfRangeException ArgRange<T>(T? argument,
        string? message = default,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    // enum

    public static ArgException Enum<E>(E @enum,
        ref InterpolatedTextBuilder interpolatedMessage,
        [CallerArgumentExpression(nameof(@enum))]
        string? enumName = null)
        where E : struct, Enum
    {
        return ArgException.New<E>(@enum, ref interpolatedMessage, null, enumName);
    }

    public static ArgException Enum<E>(E @enum,
        string? message = null,
        [CallerArgumentExpression(nameof(@enum))]
        string? enumName = null)
        where E : struct, Enum
    {
        return ArgException.New<E>(@enum, message, null, enumName);
    }
}