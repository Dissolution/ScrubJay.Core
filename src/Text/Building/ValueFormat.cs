namespace ScrubJay.Text;

[PublicAPI]
public readonly ref struct ValueFormat<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    public static implicit operator ValueFormat<T>(T value) => new(value);

    public readonly T Value;

    public readonly text Format;

    public ValueFormat(T value)
    {
        Value = value;
        Format = default;
    }

    public ValueFormat(T value, text format)
    {
        Value = value;
        Format = format;
    }

    public override string ToString()
    {
        return Build($"{Value}:{Format}");
    }
}