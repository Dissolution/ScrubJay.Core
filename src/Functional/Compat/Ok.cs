// Prefix generic type parameter with T, Rename type, Should override Equals
#pragma warning disable CA1715, CA1716, CA1815

namespace ScrubJay.Functional.Compat;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
#if NET9_0_OR_GREATER
public readonly ref struct Ok<T>
    where T : allows ref struct
#else
public readonly struct Ok<T>
#endif
{
    public readonly T Value;

    public Ok(T value)
    {
        Value = value;
    }

    public void Deconstruct(out T value)
    {
        value = this.Value;
    }
}
