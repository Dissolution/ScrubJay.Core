namespace ScrubJay.Functional.IMPL;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly ref struct Error<E>
#if NET9_0_OR_GREATER
    where E : allows ref struct
#endif
{
    public readonly E Value;

    public Error(E value)
    {
        Value = value;
    }

    public void Deconstruct(out E error)
    {
        error = Value;
    }

    public override string ToString()
    {
        return $"Error<{typeof(E)}>({Any.ToString(Value)})";
    }
}