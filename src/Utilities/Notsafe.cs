#pragma warning disable IDE0060
#pragma warning disable CS8500

using InlineIL;
using static InlineIL.IL;

// ReSharper disable EntityNameCapturedOnly.Local

namespace ScrubJay.Utilities;

/// <summary>
/// <b>Very unsafe</b> methods, often more dangerous than <see cref="Unsafe"/><br/>
/// Many of the methods in here have no validation,
/// and often direct il manipulation for pure speed has been defined
/// </summary>
/// <seealso href="https://github.com/ltrzesniewski/InlineIL.Fody/blob/master/src/InlineIL.Examples/UnsafeNet9.cs"/>
[PublicAPI]
public static unsafe class Notsafe
{
    /// <summary>
    /// <b>Very unsafe</b> methods that operate on <c>unmanaged</c> values
    /// </summary>
    public static class Unmanaged
    {
#region CopyTo

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyUnmanagedBlock<U>(in U source, ref U destination, int count)
            where U : unmanaged
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(count));
            Emit.Sizeof<U>();
            Emit.Mul();
            Emit.Cpblk();
        }

        /* Source types: `T[]`, `Span<T>`, `ReadOnlySpan<T>`
         * Destination types: `T[]`, `Span<T>`
         */


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(U[] source, U[] destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
#if NET5_0_OR_GREATER
                in MemoryMarshal.GetArrayDataReference<U>(source),
                ref MemoryMarshal.GetArrayDataReference<U>(destination),
#else
                in MemoryMarshal.GetReference<U>(source),
                ref MemoryMarshal.GetReference<U>(destination),
#endif
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(U[] source, Span<U> destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
#if NET5_0_OR_GREATER
                in MemoryMarshal.GetArrayDataReference<U>(source),
#else
                in MemoryMarshal.GetReference<U>(source),
#endif
                ref MemoryMarshal.GetReference<U>(destination),
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(Span<U> source, U[] destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
                in MemoryMarshal.GetReference<U>(source),
#if NET5_0_OR_GREATER
                ref MemoryMarshal.GetArrayDataReference<U>(destination),
#else
                ref MemoryMarshal.GetReference<U>(destination),
#endif
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(Span<U> source, Span<U> destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
                in MemoryMarshal.GetReference<U>(source),
                ref MemoryMarshal.GetReference<U>(destination),
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(ReadOnlySpan<U> source, U[] destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
                in MemoryMarshal.GetReference<U>(source),
#if NET5_0_OR_GREATER
                ref MemoryMarshal.GetArrayDataReference<U>(destination),
#else
                ref MemoryMarshal.GetReference<U>(destination),
#endif
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(ReadOnlySpan<U> source, Span<U> destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
                in MemoryMarshal.GetReference<U>(source),
                ref MemoryMarshal.GetReference<U>(destination),
                count);

        /* And the below is just crazy */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyBlock<I, O>(in I source, ref O destination, int count)
            where I : unmanaged
            where O : unmanaged
        {
            Debug.Assert(SizeOf<I>() == SizeOf<O>());
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(count));
            Emit.Sizeof<I>();
            Emit.Mul();
            Emit.Cpblk();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<I, O>(ReadOnlySpan<I> source, Span<O> destination, int count)
            where I : unmanaged
            where O : unmanaged
        {
            Debug.Assert(SizeOf<I>() == SizeOf<O>());
            CopyBlock<I, O>(
                in MemoryMarshal.GetReference<I>(source),
                ref MemoryMarshal.GetReference<O>(destination),
                count);
        }

#endregion


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero<U>(U value)
            where U : unmanaged
        {
            Emit.Ldarg(nameof(value));
            Emit.Ldc_I4_0();
            Emit.Ceq();
            return Return<bool>();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>([NotNullWhen(false)] ref readonly T source)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg_0();
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNonNullRef<T>([NotNullWhen(true)] ref readonly T source)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldc_I4_0();
        Emit.Ldarg_0();
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        Emit.Ceq();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T NullRef<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOf<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return sizeof(T);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SkipInit<T>(out T value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ret();
        throw Unreachable();
    }


#region Changing reference types

    /************************************
     * Roughly:
     * void*, T*, in T, ref T
     ************************************
     */

    /// <summary>
    /// Returns the <typeparamref name="I"/> <paramref name="input"/> as a <typeparamref name="O"/> with no type checking
    /// </summary>
    /// <remarks>
    /// If <typeparamref name="I"/> and <typeparamref name="O"/> do not have the same size and layout,
    /// memory corruption, undefined behavior, and Exceptions may occur
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static O As<I, O>(I input)
#if NET9_0_OR_GREATER
        where I : allows ref struct
        where O : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(input));
        return Return<O>();
    }

    // void* -> ?

    /// <summary>
    /// <c>void* -> T*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* VoidPtrAsPtr<T>(void* voidPointer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(voidPointer));
        return ReturnPointer<T>();
    }

    /// <summary>
    /// <c>void* -> ref T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T VoidPtrAsRef<T>(void* voidPointer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(voidPointer));
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// <c>void* -> in T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T VoidPtrAsIn<T>(void* voidPointer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(voidPointer));
        return ref ReturnRef<T>();
    }

    // T* -> ?

    /// <summary>
    /// <c>T* -> void*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* PtrAsVoidPtr<T>(T* sourcePointer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ReturnPointer();
    }

    /// <summary>
    /// <c>I* -> O*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static O* PtrAsPtr<I, O>(I* sourcePointer)
#if NET9_0_OR_GREATER
        where I : allows ref struct
        where O : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ReturnPointer<O>();
    }

    /// <summary>
    /// <c>T* -> ref T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T PtrAsRef<T>(T* sourcePointer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// <c>I* -> ref O</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref O PtrAsRef<I, O>(I* sourcePointer)
#if NET9_0_OR_GREATER
        where I : allows ref struct
        where O : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ref ReturnRef<O>();
    }

    /// <summary>
    /// <c>T* -> in T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T PtrAsIn<T>(T* sourcePointer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// <c>I* -> in O</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly O PtrAsIn<I, O>(I* sourcePointer)
#if NET9_0_OR_GREATER
        where I : allows ref struct
        where O : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ref ReturnRef<O>();
    }

    // in T -> ?

    /// <summary>
    /// <c>in T -> void*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* InAsVoidPtr<T>(in T inValue)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer();
    }

    /// <summary>
    /// <c>in T -> T*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* InAsPtr<T>(in T inValue)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer<T>();
    }

    /// <summary>
    /// <c>in I -> O*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static O* InAsPtr<I, O>(in I inValue)
#if NET9_0_OR_GREATER
        where I : allows ref struct
        where O : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer<O>();
    }

    /// <summary>
    /// <c>in I -> in O</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly O InAsIn<I, O>(in I inValue)
#if NET9_0_OR_GREATER
        where I : allows ref struct
        where O : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<O>();
    }

    /// <summary>
    /// <c>in T -> ref T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T InAsRef<T>(in T inValue)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// <c>in I -> ref O</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref O InAsRef<I, O>(in I inValue)
#if NET9_0_OR_GREATER
        where I : allows ref struct
        where O : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<O>();
    }


    // ref T -> ?

    /// <summary>
    /// <c>ref T -> void*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* RefAsVoidPtr<T>(ref T inValue)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        Emit.Conv_U();
        return ReturnPointer();
    }

    /// <summary>
    /// <c>ref T -> T*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* RefAsPtr<T>(ref T inValue)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer<T>();
    }

    /// <summary>
    /// <c>ref I -> O*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static O* RefAsPtr<I, O>(ref I inValue)
#if NET9_0_OR_GREATER
        where I : allows ref struct
        where O : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer<O>();
    }

    /// <summary>
    /// <c>ref T -> in T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T RefAsIn<T>(ref T inValue)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// <c>ref I -> in O</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly O RefAsIn<I, O>(ref I inValue)
#if NET9_0_OR_GREATER
        where I : allows ref struct
        where O : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<O>();
    }

    /// <summary>
    /// <c>ref I -> ref O</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref O RefAsRef<I, O>(ref I inValue)
#if NET9_0_OR_GREATER
        where I : allows ref struct
        where O : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<O>();
    }

    // out T -> ?

    // like why? just pure IL evil

    public static ref T OutAsRef<T>(out T outValue)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(outValue));
        Emit.Ret();
        throw Unreachable();
    }

#endregion

#region Casting

    // object -> ?

    public static object Box<T>(T value)
    {
        Emit.Ldarg(nameof(value));
        Emit.Box<T>();
        return Return<object>();
    }

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
    /// Gets a <c>ref </c><typeparamref name="T"/> to the contents of an <see cref="object"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T UnboxRef<T>(object obj)
    {
        Emit.Ldarg_0();
        Emit.Unbox<T>();
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// Casts an <see cref="object"/> to a <c>class</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T CastClass<T>(object obj)
    {
        Emit.Ldarg_0();
        Emit.Castclass<T>();
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(o))]
    public static T As<T>(object? o)
        where T : class?
    {
        Emit.Ldarg(nameof(o));
        return Return<T>();
    }


    /// <summary>
    /// Interpret the <see cref="Span{TIn}">Span&lt;TIn&gt;</see> <paramref name="input"/> as a <see cref="Span{TOut}">Span&lt;TOut&gt;</see>
    /// </summary>
    /// <remarks>
    /// Unless <typeparamref name="I"/> and <typeparamref name="O"/> have the same size and layout,
    /// exceptions can be thrown and possible memory corruption
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<O> SpanAs<I, O>(Span<I> input)
    {
        return new Span<O>(
            pointer: RefAsVoidPtr(ref input.GetPinnableReference()),
            length: input.Length);
    }

    /// <summary>
    /// Interpret the <see cref="ReadOnlySpan{TIn}">ReadOnlySpan&lt;TIn&gt;</see> <paramref name="input"/> as a <see cref="ReadOnlySpan{TOut}">ReadOnlySpan&lt;TOut&gt;</see>
    /// </summary>
    /// <remarks>
    /// Unless <typeparamref name="I"/> and <typeparamref name="O"/> have the same size and layout,
    /// exceptions can be thrown and possible memory corruption
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<O> SpanAs<I, O>(ReadOnlySpan<I> input)
    {
        return new ReadOnlySpan<O>(
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsWritable<T>(ReadOnlySpan<T> readOnlySpan)
    {
        return new Span<T>(InAsVoidPtr(in readOnlySpan.GetPinnableReference()), readOnlySpan.Length);
    }

#region Pointer Offsetting

#region Add

    /// <summary>
    /// Adds an element offset to the given reference.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* Add<T>(void* source, nuint elementOffset)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Conv_I();
        Emit.Mul();
        Emit.Add();
        return ReturnPointer();
    }

    /// <summary>
    /// Adds an element offset to the given reference.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Add<T>(ref T source, nint elementOffset)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// Adds an element offset to the given reference.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Add<T>(ref T source, nuint elementOffset)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// Adds a byte offset to the given reference.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AddByteOffset<T>(ref T source, nuint byteOffset)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(byteOffset));
        Emit.Add();
        return ref ReturnRef<T>();
    }

#endregion

#endregion


#region Comparison

    /// <summary>
    /// Determines whether the specified references point to the same location.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreSame<T>([AllowNull] ref readonly T left, [AllowNull] ref readonly T right)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Ceq();
        return Return<bool>();
    }

#endregion
}