namespace ScrubJay.Validation;

/// <summary>
/// A static helper class for creating <see cref="Exception">Exceptions</see>
/// using <see cref="InterpolatedTextBuilder"/> to generate messages
/// </summary>
[PublicAPI]
[StackTraceHidden]
public static partial class Ex
{
    private static TextBuilder AppendInfo(
        this TextBuilder builder,
        [InterpolatedStringHandlerArgument(nameof(builder))]
        ref InterpolatedTextBuilder info)
    {
        if (info.Length > 0)
        {
            return builder.Append(": ").Append(ref info);
        }

        return builder;
    }

    /// <summary>
    /// Get a new <see cref="InvalidOperationException"/>
    /// </summary>
    public static InvalidOperationException Invalid(
        InterpolatedTextBuilder message = default,
        Exception? innerException = null)
    {
        return new InvalidOperationException(
            message.ToStringAndClear(),
            innerException);
    }

    /// <summary>
    /// Get a new <see cref="NotImplementedException"/>
    /// </summary>
    public static NotImplementedException NotImplemented(
        InterpolatedTextBuilder message = default,
        Exception? innerException = null)
    {
        return new NotImplementedException(
            message.ToStringAndClear(),
            innerException);
    }

    /// <summary>
    /// Get a new <see cref="UnreachableException"/>
    /// </summary>
    public static UnreachableException Unreachable(
        InterpolatedTextBuilder message = default,
        Exception? innerException = null)
    {
        return new UnreachableException(
            message.ToStringAndClear(),
            innerException);
    }






    public static ArgumentOutOfRangeException ArgRange<T>(T? argument,
        [HandlesResourceDisposal]
        InterpolatedTextBuilder info = default,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        string message = info.ToStringAndClear();
        return new ArgumentOutOfRangeException(argumentName, argument, message);
    }



}