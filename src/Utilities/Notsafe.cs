#pragma warning disable IDE0060

using InlineIL;
using static InlineIL.IL;

namespace ScrubJay.Utilities;

/// <summary>
/// Very <b>unsafe</b> methods
/// </summary>
[PublicAPI]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Unbox<T>(object obj)
    {
        Emit.Ldarg_0();
        Emit.Unbox_Any<T>();
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T UnboxRef<T>(object obj)
        where T : struct
    {
        Emit.Ldarg_0();
        Emit.Unbox<T>();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TClass CastClass<TClass>(object obj)
    {
        Emit.Ldarg_0();
        Emit.Castclass<TClass>();
        return Return<TClass>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOut DirectCast<TIn, TOut>(TIn input)
    {
        Emit.Ldarg_0();
        return Return<TOut>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(object obj)
    {
        if (typeof(T).IsValueType)
        {
            return FastUnboxToSpan<T>(obj);
        }
        return new ReadOnlySpan<T>([(T)obj]);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ReadOnlySpan<T> FastUnboxToSpan<T>(object obj)
    {
        Emit.Ldarg(nameof(obj)); // load the object
        Emit.Unbox<T>(); // unbox it to a T*
#if NET7_0_OR_GREATER
        Emit.Newobj(MethodRef.Constructor(typeof(ReadOnlySpan<T>), typeof(T).MakeByRefType()));
#else
        Emit.Ldc_I4_1(); // count == 1
        Emit.Newobj(MethodRef.Constructor(typeof(ReadOnlySpan<T>), typeof(void*), typeof(int)));
#endif
        Emit.Ret();
        throw Unreachable();
    }
}