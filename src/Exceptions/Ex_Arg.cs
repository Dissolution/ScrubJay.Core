namespace ScrubJay.Exceptions;

partial class Ex
{
    public static ArgumentException Arg<T>(
        T? argument,
        //[HandlesResourceDisposal]
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Type argumentType = Type.GetType<T>(argument);
        var message = TextBuilder.New
            .Append($"Invalid {argumentType:@} argument \"{argumentName}\" `")
            .Render(argument)
            .Append('`')
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(info))
            .ToStringAndDispose();
        return new ArgumentException(message, innerException);
    }

    public static ArgumentException Arg(
        object? argument,
        //[HandlesResourceDisposal]
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        Type argumentType = Type.GetType(argument);
        var message = TextBuilder.New
            .Append($"Invalid {argumentType:@} argument \"{argumentName}\" `")
            .Render(argument)
            .Append('`')
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(info))
            .ToStringAndDispose();
        return new ArgumentException(message, innerException);
    }

    public static ArgumentException Arg(
        object? argument,
        Type argumentType,
        //[HandlesResourceDisposal]
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        var message = TextBuilder.New
            .Append($"Invalid {argumentType:@} argument \"{argumentName}\" `")
            .Render(argument)
            .Append('`')
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(info))
            .ToStringAndDispose();
        return new ArgumentException(message, innerException);
    }

    public static ArgumentException Arg<T>(
        object? argument,
        //[HandlesResourceDisposal]
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var message = TextBuilder.New
            .Append($"Invalid {typeof(T):@} argument \"{argumentName}\" `")
            .Render(argument)
            .Append('`')
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(info))
            .ToStringAndDispose();
        return new ArgumentException(message, innerException);
    }

    public static ArgumentException Arg<T>(scoped Span<T> argument,
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
#if NET9_0_OR_GREATER
        return Arg<Span<T>>(argument.ToArray(), info, innerException, argumentName);
#else
        return Arg(argument.ToArray(), typeof(Span<T>), info, innerException, argumentName);
#endif
    }

    public static ArgumentException Arg<T>(scoped ReadOnlySpan<T> argument,
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
#if NET9_0_OR_GREATER
        return Arg<ReadOnlySpan<T>>(argument.ToArray(), info, innerException, argumentName);
#else
        return Arg(argument.ToArray(), typeof(ReadOnlySpan<T>), info, innerException,
            argumentName);
#endif
    }
}