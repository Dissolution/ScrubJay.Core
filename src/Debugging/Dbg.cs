using static InlineIL.IL;

namespace ScrubJay.Debugging;

public static class Dbg
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [StackTraceHidden]
    [DebuggerHidden]
    [DebuggerStepThrough]
    public static void Break()
    {
        Emit.Break();
    }
}
