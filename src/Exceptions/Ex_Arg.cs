namespace ScrubJay.Exceptions;

partial class Ex
{
    public static ArgumentException Arg<T>(
        T? argument,
        //[HandlesResourceDisposal]
        InterpolatedTextBuilder info = default,
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
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(ref info))
            .ToStringAndDispose();
        return new ArgumentException(message, innerException);
    }

    public static ArgumentException Arg(
        object? argument,
        [HandlesResourceDisposal]
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        Type argumentType = Type.GetType(argument);
        var message = TextBuilder.New
            .Append($"Invalid {argumentType:@} argument \"{argumentName}\" `")
            .Render(argument)
            .Append('`')
            .IfNotEmpty(info, static (tb, nfo) => tb.Append(": ").Append(ref nfo))
            .ToStringAndDispose();
        return new ArgumentException(message, innerException);
    }

    public static ArgumentException Arg(
        object? argument,
        Type argumentType,
        //[HandlesResourceDisposal]
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        var message = TextBuilder.New
            .Append($"Invalid {argumentType:@} argument \"{argumentName}\" `")
            .Render(argument)
            .Append('`')
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(ref info))
            .ToStringAndDispose();
        return new ArgumentException(message, innerException);
    }

    public static ArgumentException Arg<T>(
        object? argument,
        //[HandlesResourceDisposal]
        InterpolatedTextBuilder info = default,
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
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(ref info))
            .ToStringAndDispose();
        return new ArgumentException(message, innerException);
    }

    public static ArgumentException Arg<T>(scoped Span<T> argument,
        InterpolatedTextBuilder info = default,
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
        InterpolatedTextBuilder info = default,
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