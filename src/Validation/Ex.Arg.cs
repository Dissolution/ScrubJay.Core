

namespace ScrubJay.Validation;

partial class Ex
{
    internal static string GetArgExceptionMessage<T>(
        T? argument,
        string? argumentName,
        InterpolatedTextBuilder info,
        InterpolatedTextBuilder fallback = default)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        using var builder = new TextBuilder();
        builder.Append($"Argument \"{argumentName}\" ({typeof(T):@}) `{argument:@}`: ");
        if (info.Length > 0)
        {
            builder.Append(ref info);
        }
        else if (fallback.Length > 0)
        {
            builder.Append(ref fallback);
        }
        else
        {
            builder.Append(" is invalid");
        }

        return builder.ToString();
    }

    internal static string GetArgExceptionMessage(
        Type? argumentType,
        string? argumentString,
        string? argumentName,
        InterpolatedTextBuilder info,
        InterpolatedTextBuilder fallback)
    {
        using var builder = new TextBuilder();
        builder.Append($"Argument \"{argumentName}\" ({argumentType:@}) `{argumentString}`: ");
        if (info.Length > 0)
        {
            builder.Append(ref info);
        }
        else if (fallback.Length > 0)
        {
            builder.Append(ref fallback);
        }
        else
        {
            builder.Append(" is invalid");
        }

        return builder.ToString();
    }

    public static ArgumentException Argument<T>(
        T? argument,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        string message = GetArgExceptionMessage<T>(argument, argumentName, info);
        return new ArgumentException(message, innerException);
    }


    public static ArgumentException Argument(
        object? argument,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        string message = GetArgExceptionMessage(argument?.GetType(), argument?.ToString(), argumentName, info);
        return new ArgumentException(message, innerException);
    }

    public static ArgumentException Argument(
        Type? argumentType,
        string? argumentString,
        string? argumentName,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
    {
        var message = GetArgExceptionMessage(argumentType, argumentString, argumentName, info);
        return new ArgumentException(message, innerException);
    }


    public static ArgumentException Argument<T>(
        scoped Span<T> argument,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
#if NET9_0_OR_GREATER
        return Argument<Span<T>>(argument, info, innerException, argumentName);
#else
        return Argument(typeof(Span<T>), argument.Render(), argumentName, info, innerException);
#endif
    }

    public static ArgumentException Argument<T>(scoped ReadOnlySpan<T> argument,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
#if NET9_0_OR_GREATER
        return Argument<ReadOnlySpan<T>>(argument, info, innerException, argumentName);
#else
        return Argument(typeof(Span<T>), argument.Render(), argumentName, info, innerException);
#endif
    }
}