#pragma warning disable CA1065

namespace ScrubJay.Utilities;

[PublicAPI]
public enum Endianness
{
    Little = 0,
    Big = 1,
    System = 2,
    NonSystem = 3,
}

[PublicAPI]
public static class EndiannessExtensions
{
    extension(Endianness endianness)
    {
        public Endianness Reverse()
            => endianness == Endianness.Little ? Endianness.Big : Endianness.Little;

        public Endianness Resolve()
        {
            switch (endianness)
            {
                case Endianness.Little:
                    return Endianness.Little;
                case Endianness.Big:
                    return Endianness.Big;
                case Endianness.System:
                    return BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;
                case Endianness.NonSystem:
                    return BitConverter.IsLittleEndian ? Endianness.Big : Endianness.Little;
                default:
                    throw Ex.Enum(endianness);
            }
        }

        public bool IsSwap => endianness switch
        {
            Endianness.System => false,
            Endianness.NonSystem => true,
            Endianness.Little => !BitConverter.IsLittleEndian,
            Endianness.Big => BitConverter.IsLittleEndian,
            _ => throw Ex.Enum(endianness),
        };


        public bool IsNoSwap => endianness switch
        {
            Endianness.System => true,
            Endianness.NonSystem => false,
            Endianness.Little => BitConverter.IsLittleEndian,
            Endianness.Big => !BitConverter.IsLittleEndian,
            _ => throw Ex.Enum(endianness),
        };
    }
}
