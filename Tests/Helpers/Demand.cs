using System.Runtime.CompilerServices;
using ScrubJay.Exceptions;
using ScrubJay.Text;

namespace ScrubJay.Tests.Helpers;

public class DemandException : Exception
{
    public static DemandException New<T>(
        CapturedValue<T> captured,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
    {
        var message = TextBuilder.New
            .Append($"Invalid {captured.ValueType:@} variable \"{captured.ValueName}\" `")
            .Render(captured.Value)
            .Append('`')
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(info))
            .ToStringAndDispose();
        return new(message, innerException);
    }

    public static DemandException New(
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
    {
        var message = TextBuilder.New
            .Append("Failure was demanded")
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(info))
            .ToStringAndDispose();
        return new(message, innerException);
    }

    protected DemandException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

[StackTraceHidden]
public static class Demand
{
    public static CapturedValue<T> That<T>(T? value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return new CapturedValue<T>(value, valueName);
    }

    [DoesNotReturn]
    public static void Fail(
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
    {
        throw DemandException.New(info, innerException);
    }

    internal static ArgumentException GetEx<T>(
        CapturedValue<T> captured,
        InterpolatedTextBuilder info)
    {
        return Ex.Arg<T>(captured.Value, info, null, captured.ValueName);
    }

    extension<T>(CapturedValue<T> captured)
    {
        [AssertionMethod]
        public void IsEqualTo(T? other)
        {
            if (!EqualityComparer<T>.Default.Equals(captured.Value!, other!))
            {
                throw GetEx(captured, $"was not equal to {other}");
            }
        }

        [AssertionMethod]
        public void IsGreaterThan(T? other)
        {
            int c = Comparer<T>.Default.Compare(captured.Value!, other!);
            if (c <= 0)
            {
                throw GetEx(captured, $"was not greater than {other}");
            }
        }

        [AssertionMethod]
        public void IsGreaterThanOrEqualTo(T? other)
        {
            int c = Comparer<T>.Default.Compare(captured.Value!, other!);
            if (c < 0)
            {
                throw GetEx(captured, $"was not greater than or equal to {other}");
            }
        }
    }


    extension<T>(CapturedValue<T> captured)
        where T : class
    {
        [AssertionMethod]
        public void IsNotNull()
        {
            if (captured.Value is null)
                throw GetEx(captured, $"was null");
        }
    }
}

public ref struct CapturedValue<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    public readonly T? Value;

    public readonly string? ValueName;

    public Type ValueType
    {
#if NET9_0_OR_GREATER
        get => typeof(T);
#else
        get => Value?.GetType() ?? typeof(T);
#endif
    }

    public string ValueString
    {
#if NET9_0_OR_GREATER
        get => Value.Stringify();
#else
        get => Value?.ToString() ?? string.Empty;
#endif
    }

    internal CapturedValue(T? value, string? valueName)
    {
        Value = value;
        ValueName = valueName;
    }
}