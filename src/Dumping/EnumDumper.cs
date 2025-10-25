// namespace ScrubJay.Dumping;
//
// // Serilog Structured Data
// // https://github.com/serilog/serilog/wiki/Structured-Data
//
// // Scalar Types
// // Booleans - bool
// // Numerics - byte, short, ushort, int, uint, long, ulong, float, double, decimal
// // Strings - string, byte[]
// // Temporals - DateTime, DateTimeOffset, TimeSpan
// // Others - Guid, Uri
// // Nullables - nullable versions of any of the types above
//
// // Collections
// // IEnumerable
// // Dictionary<K,V> where K is one of the Scalar Types above
//
// // Defaults to ToString!
//
// // @ - Destructure
// // $ - Stringify
//
// public sealed class EnumDumper : IValueDumper<Enum>, IHasDefault<EnumDumper>
// {
//     public static EnumDumper Default { get; } = new();
//
//     public void DumpTo(TextBuilder builder, Enum value, DumpMode mode = DumpMode.Default)
//     {
//         throw new NotImplementedException();
//     }
//
//     public void DumpTo<E>(TextBuilder builder, E value, DumpMode mode = DumpMode.Default)
//         where E : struct, Enum
//     {
//         // todo: change to dump
//         EnumInfo.RenderTo(builder, value);
//     }
// }