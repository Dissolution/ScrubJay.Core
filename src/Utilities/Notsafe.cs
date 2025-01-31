#pragma warning disable IDE0060, CS8500, CA1045

using InlineIL;
using static InlineIL.IL;

namespace ScrubJay.Utilities;

/// <summary>
/// Very <b>unsafe</b> methods, more dangerous than <see cref="Unsafe"/>
/// </summary>
[PublicAPI]
public static unsafe class Notsafe
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
    public static int SizeOf<T>()
        where T : struct
    {
        Emit.Sizeof<T>();
        return Return<int>();
    }


    #region Referencing
    /************************************
     * Roughly:
     * void*, T*, in T, ref T
     ************************************
     */

    // void* -> ?

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* VoidPtrAsPtr<T>(void* voidPointer)
       where T : unmanaged
    {
        Emit.Ldarg(nameof(voidPointer));
        return ReturnPointer<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T VoidPtrAsRef<T>(void* voidPointer)
    {
        Emit.Ldarg(nameof(voidPointer));
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T VoidPtrAsIn<T>(void* voidPointer)
    {
        Emit.Ldarg(nameof(voidPointer));
        return ref ReturnRef<T>();
    }

    // T* -> ?

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* PtrAsVoidPtr<T>(T* sourcePointer)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T PtrAsRef<T>(T* sourcePointer)
       where T : unmanaged
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T PtrAsIn<T>(T* sourcePointer)
       where T : unmanaged
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ref ReturnRef<T>();
    }

    // in T -> ?

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* InAsVoidPtr<T>(in T inValue)
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* InAsPtr<T>(in T inValue)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T InAsRef<T>(in T inValue)
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<T>();
    }

    // ref T -> ?

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* RefAsVoidPtr<T>(ref T inValue)
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* RefAsPtr<T>(ref T inValue)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T RefAsIn<T>(ref T inValue)
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<T>();
    }

    #endregion

    #region Casting

    // object -> ?

    /// <summary>
    /// Unbox an <see cref="object"/> to a <typeparamref name="T"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Unbox<T>(object obj)
    {
        Emit.Ldarg_0();
        Emit.Unbox_Any<T>();
        return Return<T>();
    }

    /// <summary>
    /// Gets a <c>ref </c><typeparamref name="TStruct"/> to the contents of an <see cref="object"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TStruct UnboxRef<TStruct>(object obj)
        where TStruct : struct
    {
        Emit.Ldarg_0();
        Emit.Unbox<TStruct>();
        return ref ReturnRef<TStruct>();
    }

    /// <summary>
    /// Casts an <see cref="object"/> to a <c>class</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TClass CastClass<TClass>(object obj)
        where TClass : class
    {
        Emit.Ldarg_0();
        Emit.Castclass<TClass>();
        return Return<TClass>();
    }

    /// <summary>
    /// Uses an <see cref="object"/> <b>as</b> a <typeparamref name="T"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(obj))]
    public static T As<T>(object? obj)
        where T : class?
    {
        Emit.Ldarg(nameof(obj));
        Emit.Ret();
        throw Unreachable();
    }


    /// <summary>
    /// Interpret the <typeparamref name="TIn"/> <paramref name="input"/> as a <typeparamref name="TOut"/>
    /// </summary>
    /// <remarks>
    /// Unless <typeparamref name="TIn"/> and <typeparamref name="TOut"/> have the same size and layout,
    /// exceptions can be thrown and possible memory corruption
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOut As<TIn, TOut>(TIn input)
    {
        Emit.Ldarg_0();
        return Return<TOut>();
    }

    /// <summary>
    /// Interpret the <c>ref </c><typeparamref name="TIn"/> <paramref name="input"/> as a <c>ref </c><typeparamref name="TOut"/>
    /// </summary>
    /// <remarks>
    /// Unless <typeparamref name="TIn"/> and <typeparamref name="TOut"/> have the same size and layout,
    /// exceptions can be thrown and possible memory corruption
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TOut As<TIn, TOut>(ref TIn input)
    {
        Emit.Ldarg_0();
        return ref ReturnRef<TOut>();
    }

    /// <summary>
    /// Interpret the <see cref="Span{TIn}">Span&lt;TIn&gt;</see> <paramref name="input"/> as a <see cref="Span{TOut}">Span&lt;TOut&gt;</see>
    /// </summary>
    /// <remarks>
    /// Unless <typeparamref name="TIn"/> and <typeparamref name="TOut"/> have the same size and layout,
    /// exceptions can be thrown and possible memory corruption
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<TOut> As<TIn, TOut>(Span<TIn> input)
    {
        return new Span<TOut>(
            pointer: RefAsVoidPtr(ref input.GetPinnableReference()),
            length: input.Length);
    }

    /// <summary>
    /// Interpret the <see cref="ReadOnlySpan{TIn}">ReadOnlySpan&lt;TIn&gt;</see> <paramref name="input"/> as a <see cref="ReadOnlySpan{TOut}">ReadOnlySpan&lt;TOut&gt;</see>
    /// </summary>
    /// <remarks>
    /// Unless <typeparamref name="TIn"/> and <typeparamref name="TOut"/> have the same size and layout,
    /// exceptions can be thrown and possible memory corruption
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<TOut> As<TIn, TOut>(ReadOnlySpan<TIn> input)
    {
        return new ReadOnlySpan<TOut>(
            pointer: InAsVoidPtr(in input.GetPinnableReference()),
            length: input.Length);
    }
    #endregion

    // crazy stuff below

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
        // call `new ReadOnlySpan<T>(ref T)`
        Emit.Newobj(MethodRef.Constructor(typeof(ReadOnlySpan<T>), typeof(T).MakeByRefType()));
#else
        // have to call `new ReadOnlySpan<T>(void*, int)`
        Emit.Ldc_I4_1(); // count == 1
        Emit.Newobj(MethodRef.Constructor(typeof(ReadOnlySpan<T>), typeof(void*), typeof(int)));
#endif
        Emit.Ret();
        throw Unreachable();
    }
}
