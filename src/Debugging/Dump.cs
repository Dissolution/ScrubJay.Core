using InlineIL;

namespace ScrubJay.Debugging;

[StackTraceHidden]
internal static class Dump
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Break()
    {
        IL.Emit.Break();
    }
}