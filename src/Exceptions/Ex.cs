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


    public static ArgumentException Arg<T>(T? argument,
        ref InterpolatedTextBuilder interpolatedMessage,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        string message = interpolatedMessage.ToStringAndDispose();
        return new ArgumentException(message, argumentName);
    }

    public static ArgumentException Arg<T>(T? argument,
        string? message = default,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return new ArgumentException(message, argumentName);
    }


    public static ArgumentException ArgNull<T>(T? argument,
        ref InterpolatedTextBuilder interpolatedMessage,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        string message = interpolatedMessage.ToStringAndDispose();
        return new ArgumentNullException(argumentName, message);
    }

    public static ArgumentException ArgNull<T>(T? argument,
        string? message = default,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgumentNullException(argumentName, message);
    }


    public static ArgumentException ArgRange<T>(T? argument,
        ref InterpolatedTextBuilder interpolatedMessage,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        string? message = interpolatedMessage.ToStringAndDispose();
        return new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static ArgumentException ArgRange<T>(T? argument,
        string? message = default,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgumentOutOfRangeException(argumentName, argument, message);
    }
}