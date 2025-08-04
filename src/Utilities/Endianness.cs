namespace ScrubJay.Utilities;

[PublicAPI]
public enum Endianness
{
    Little = 0,
    Big = 1,
    System = 2,
    NonSystem = 3,
}

/// <summary>
///
/// </summary>
/// <remarks>
/// See <see cref="System.Buffers.Binary.BinaryPrimitives"/>
/// </remarks>
[PublicAPI]
public static class EndianHelper
{
    public static readonly Endianness SystemEndianness = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

    // public static ReadOnlySpan<byte> AsSpan<U>(in U value, Endianness endianness = Endianness.Little)
    //     where U : unmanaged
    // {
    //     unsafe
    //     {
    //         if (endianness == SystemEndianness)
    //         {
    //             return new ReadOnlySpan<byte>(Notsafe.InAsVoidPtr<U>(in value), sizeof(U));
    //         }
    //
    //         Span<byte> span = new Span<byte>(Notsafe.InAsVoidPtr<U>(in value), sizeof(U));
    //         span.Reverse();
    //         return span;
    //     }
    // }

    public static Endianness Flipped(this Endianness endianness)
    {
        return endianness switch
        {
            Endianness.Little => Endianness.Big,
            Endianness.Big => Endianness.Little,
            Endianness.System => Endianness.NonSystem,
            Endianness.NonSystem => Endianness.System,
            _ => throw InvalidEnumException.New(endianness),
        };
    }

    public static Endianness Resolve(this Endianness endianness)
    {
        return endianness switch
        {
            Endianness.System => SystemEndianness,
            Endianness.NonSystem => SystemEndianness == Endianness.Little ? Endianness.Big : Endianness.Little,
            _ => endianness,
        };
    }
}