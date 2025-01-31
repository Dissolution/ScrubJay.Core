global using ByteSpanReader = ScrubJay.Memory.SpanReader<byte>;

namespace ScrubJay.Memory;

public static class ByteSpanReaderExtensions
{
    public static T PeekValue<T>(this ref ByteSpanReader reader)
        where T : unmanaged
        => TryPeekValue<T>(ref reader).SomeOrThrow();

    public static Option<T> TryPeekValue<T>(this ref ByteSpanReader reader)
        where T : unmanaged
    {
        if (reader.TryPeek(Notsafe.SizeOf<T>()).HasReason(StopReason.Satisified, out var span))
        {
#if !NET8_0_OR_GREATER
            var value = Unsafe.ReadUnaligned<T>(ref Notsafe.InAsRef(in span.GetPinnableReference()));
#else
            var value = Unsafe.ReadUnaligned<T>(in span.GetPinnableReference());
#endif
            return Some(value);
        }
        return None();
    }

    public static T TakeValue<T>(this ref ByteSpanReader reader)
        where T : unmanaged
        => TryTakeValue<T>(ref reader).SomeOrThrow();

    public static Option<T> TryTakeValue<T>(this ref ByteSpanReader reader)
        where T : unmanaged
    {
        if (reader.TryTake(Notsafe.SizeOf<T>()).HasReason(StopReason.Satisified, out var span))
        {
#if !NET8_0_OR_GREATER
            var value = Unsafe.ReadUnaligned<T>(ref Notsafe.InAsRef(in span.GetPinnableReference()));
#else
            var value = Unsafe.ReadUnaligned<T>(in span.GetPinnableReference());
#endif
            return Some(value);
        }
        return None();
    }


}
