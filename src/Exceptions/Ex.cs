namespace ScrubJay.Exceptions;

[PublicAPI]
[StackTraceHidden]
public static partial class Ex
{
    public static InvalidOperationException Invalid(
        InterpolatedText info = default,
        Exception? innerException = null)
    {
        string message = info.ToStringAndDispose();
        return new InvalidOperationException(message, innerException);
    }

    public static NotImplementedException NotImplemented(
        InterpolatedText info = default,
        Exception? innerException = null)
    {
        string message = info.ToStringAndDispose();
        return new NotImplementedException(message, innerException);
    }

    public static UnreachableException Unreachable(
        InterpolatedText info = default,
        Exception? innerException = null)
    {
        string message = info.ToStringAndDispose();
        return new UnreachableException(message, innerException);
    }

    public static NotSupportedException NotSupported(
        Type? instanceType = null,
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerMemberName] string? methodName = null)
    {
        string message = TextBuilder
            .New
            .Append($"Cannot call {methodName} on {instanceType:@}")
            .IfNotEmpty(info,
                static (builder, n) => builder.Append(": ").Append(n),
                builder => builder.If(instanceType!.IsRef,
                    tb => tb.Append($": {instanceType:@} is a ref struct and cannot be boxed")))
            .ToStringAndDispose();

        return new NotSupportedException(message, innerException);
    }

    public static NotSupportedException NotSupported<T>(
        T? instance = default,
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerMemberName] string? methodName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => NotSupported(typeof(T), info, innerException, methodName);


    public static ArgException Arg(object? argument,
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return ArgException.New(argument, info, innerException, argumentName);
    }

    public static ArgException Arg<T>(object? argument,
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return ArgException.New<T>(argument, info, innerException, argumentName);
    }


    public static ArgException Arg<T>(scoped Span<T> argument,
        InterpolatedText info = default,
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

    public static ArgException Arg<T>(scoped ReadOnlySpan<T> argument,
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
#if NET9_0_OR_GREATER
        return ArgException.New<ReadOnlySpan<T>>(argument.ToArray(), info, innerException, argumentName);
#else
        return ArgException.New(argument.ToArray(), typeof(ReadOnlySpan<T>), info, innerException,
            argumentName);
#endif
    }


    public static ArgumentNullException ArgNull<T>(T? argument,
        InterpolatedText info = default,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        string message = info.ToStringAndDispose();
        return new ArgumentNullException(argumentName, message);
    }


    public static ArgumentOutOfRangeException ArgRange<T>(T? argument,
        InterpolatedText info = default,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        string message = info.ToStringAndDispose();
        return new ArgumentOutOfRangeException(argumentName, argument, message);
    }


    // enum

    public static ArgException Enum<E>(E @enum,
        InterpolatedText info = default,
        [CallerArgumentExpression(nameof(@enum))]
        string? enumName = null)
        where E : struct, Enum
    {
        return ArgException.New<E>(@enum,  info, null, enumName);
    }
}