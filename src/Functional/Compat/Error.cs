// Prefix generic type parameter with T, Rename type, Should override Equals
#pragma warning disable CA1715, CA1716, CA1815

namespace ScrubJay.Functional.Compat;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
#if NET9_0_OR_GREATER
public readonly ref struct Error<E>
    where E : allows ref struct
#else
public readonly struct Error<E>
#endif
{
    internal readonly E _value;

    public Error(E value)
    {
        _value = value;
    }
}
