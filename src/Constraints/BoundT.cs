// CA1000: Do not declare static members on generic types
#pragma warning disable CA1000

namespace ScrubJay.Constraints;

[PublicAPI]
public static class Bound
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bound<T> Inclusive<T>(T value) => Bound<T>.Inclusive(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bound<T> Exclusive<T>(T value) => Bound<T>.Exclusive(value);
}


[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly record struct Bound<T>(T Value, bool IsInclusive)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bound<T> Inclusive(T value) => new(value, true);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bound<T> Exclusive(T value) => new(value, false);
}
