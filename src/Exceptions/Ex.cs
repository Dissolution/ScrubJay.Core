namespace ScrubJay.Exceptions;

[PublicAPI]
[StackTraceHidden]
public static partial class Ex
{
    public static InvalidOperationException Invalid(
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
    {
        string message = info.ToStringAndClear();
        return new InvalidOperationException(message, innerException);
    }

    public static NotImplementedException NotImplemented(
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
    {
        string message = info.ToStringAndClear();
        return new NotImplementedException(message, innerException);
    }

    public static UnreachableException Unreachable(
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
    {
        string message = info.ToStringAndClear();
        return new UnreachableException(message, innerException);
    }

    public static NotSupportedException NotSupported(
        Type? instanceType = null,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerMemberName] string? methodName = null)
    {
        string message = TextBuilder
            .New
            .Append($"Cannot call {methodName} on {instanceType:@}")
            .IfNotEmpty(info,
                static (builder, n) => builder.Append(": ").Append(ref n),
                builder => builder.If(instanceType!.IsRef,
                    tb => tb.Append($": {instanceType:@} is a ref struct and cannot be boxed")))
            .ToStringAndDispose();

        return new NotSupportedException(message, innerException);
    }

    public static NotSupportedException NotSupported<T>(
        T? instance = default,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerMemberName] string? methodName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => NotSupported(typeof(T), info, innerException, methodName);








    public static ArgumentNullException ArgNull<T>(T? argument,
        InterpolatedTextBuilder info = default,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        string message = info.ToStringAndClear();
        return new ArgumentNullException(argumentName, message);
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