namespace ScrubJay.Validation.Demanding;

[PublicAPI]
public class DemandException : Exception
{
    public static DemandException New<T>(
        ValidatingValue<T> captured,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var message = TextBuilder.New
            .Append($"Invalid {captured.ValueType:@} variable \"{captured.ValueName}\" `{captured.Value:@}`")
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(ref info))
            .ToStringAndDispose();
        return new(message, innerException);
    }

    public DemandException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

[StackTraceHidden]
public static class Demand
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValidatingValue<T> That<T>(T value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return new ValidatingValue<T>(value, valueName);
    }

    public static ValidatingValue<T> Capture<T>(T value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var trace = new StackTrace();
        var frames = trace.GetFrames();
        // skip this frame for Capture
        Debugger.Break();


        throw Ex.NotImplemented();
    }


    extension<T>(ValidatingValue<T> captured)
        where T : class
    {
        public void IsNotNull()
        {
            if (captured.Value is null)
                throw DemandException.New(captured, $"was null");
        }
    }

#if NET9_0_OR_GREATER
    extension<T>(ValidatingValue<T> captured)
        where T : allows ref struct
    {

    }
#endif
}