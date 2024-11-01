using static InlineIL.IL;

namespace ScrubJay.Utilities;

public static class Notsafe
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>([NotNullWhen(false)] ref readonly T source)
    {
        Emit.Ldarg_0();
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNonNullRef<T>([NotNullWhen(true)] ref readonly T source)
    {
        Emit.Ldarg_0();
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        Emit.Beq_S("is_null");
        Emit.Ldc_I4_1();
        Emit.Ret();
        MarkLabel("is_null");
        Emit.Ldc_I4_0();
        Emit.Ret();
        throw Unreachable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T NullRef<T>()
    {
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        return ref ReturnRef<T>();
    }
}