namespace ScrubJay.Validation;

[PublicAPI]
public class DemandException : Exception
{
    public static DemandException New<T>(
        ValidatingValue<T> captured,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
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
    public static ValidatingValue<T> That<T>(T? value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return new ValidatingValue<T>(value, valueName);
    }

    /// <summary>
    /// Extensions that validate a <see cref="ValidatingValue{T}"/>
    /// </summary>
    /// <param name="captured"></param>
    /// <typeparam name="T"></typeparam>
    extension<T>(ValidatingValue<T> captured)
    {
        public void IsEqualTo(T? other)
        {
            if (!EqualityComparer<T>.Default.Equals(captured.Value!, other!))
            {
                throw DemandException.New(captured, $"was not equal to `{other:@}`");
            }
        }

        public void IsGreaterThan(T? other)
        {
            int c = Comparer<T>.Default.Compare(captured.Value!, other!);
            if (c <= 0)
            {
                throw DemandException.New(captured, $"was not greater than `{other:@}`");
            }
        }

        public void IsGreaterThanOrEqualTo(T? other)
        {
            int c = Comparer<T>.Default.Compare(captured.Value!, other!);
            if (c < 0)
            {
                throw DemandException.New(captured, $"was not greater than or equal to `{other:@}`");
            }
        }
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