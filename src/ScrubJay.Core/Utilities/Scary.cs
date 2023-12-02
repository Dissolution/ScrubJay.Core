﻿// ReSharper disable EntityNameCapturedOnly.Global

#if NET48 || NETSTANDARD2_0
using System.Runtime.Serialization;
#endif

using System.Runtime.InteropServices;
using InlineIL;
using static InlineIL.IL;

namespace ScrubJay.Utilities;

/// <summary>
/// Similar to <see cref="System.Runtime.CompilerServices.Unsafe"/>, this helper class is full
/// of bad things you shouldn't use unless you understand what you are doing.
/// </summary>
/// <see href="https://github.com/ltrzesniewski/InlineIL.Fody/blob/master/src/InlineIL.Examples/Unsafe.cs"/>
public static unsafe class Scary
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object GetUninitializedObject(Type type)
    {
#if NET48 || NETSTANDARD2_0
        return FormatterServices.GetUninitializedObject(type);
#else
        return RuntimeHelpers.GetUninitializedObject(type);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetUninitializedObject<T>()
    {
        return (T)GetUninitializedObject(typeof(T));
    }


#region Read / Write
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Read<T>(void* source)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldobj<T>();
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Read<T>(in byte source)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldobj<T>();
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadUnaligned<T>(void* source)
    {
        Emit.Ldarg(nameof(source));
        Emit.Unaligned(1);
        Emit.Ldobj(typeof(T));
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReadUnaligned<T>(in byte source)
    {
        Emit.Ldarg(nameof(source));
        Emit.Unaligned(1);
        Emit.Ldobj(typeof(T));
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(void* destination, T value)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(value));
        Emit.Stobj<T>();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUnaligned<T>(void* destination, T value)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(value));
        Emit.Unaligned(1);
        Emit.Stobj<T>();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUnaligned<T>(ref byte destination, T value)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(value));
        Emit.Unaligned(1);
        Emit.Stobj<T>();
    }
#endregion

#region CopyTo
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(in T source, void* destination)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldobj<T>();
        Emit.Stobj<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(void* source, ref T destination)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldobj<T>();
        Emit.Stobj<T>();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<TIn, TOut>(in TIn source, ref TOut destination)
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldobj<TIn>();
        Emit.Stobj<TOut>();
    }
#endregion


#region To
    /* XToY indicates a direct cast between X -> Y that does not perform any allocations
     * These methods are Scary/Unsafe because they perform no checking of any kind
     *
     * ___Inputs___
     * void*
     * unmanaged T*
     * in T
     * ref T
     * Span<T>
     * ReadOnlySpan<T>
     *
     * ___Outputs___
     * void*
     * unmanaged T*
     * ref T
     * Span<T>
     * ReadOnlySpan<T>
     */

    // void* ->

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* VoidPointerToPointer<T>(void* voidPtr)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(voidPtr));
        Emit.Conv_I();
        return ReturnPointer<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T VoidPointerToRef<T>(void* voidPtr)
    {
        Emit.Ldarg(nameof(voidPtr));
        Emit.Conv_I();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> VoidPointerToSpan<T>(void* voidPtr, int length)
    {
        return new Span<T>(voidPtr, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> VoidPointerToReadOnlySpan<T>(void* voidPtr, int length)
    {
        return new ReadOnlySpan<T>(voidPtr, length);
    }

    // unmanaged T* ->

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* PointerToVoidPointer<T>(T* valuePtr)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(valuePtr));
        Emit.Conv_U();
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T PointerToRef<T>(T* valuePtr)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(valuePtr));
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> PointerToSpan<T>(T* valuePtr, int length)
        where T : unmanaged
    {
        return new Span<T>(valuePtr, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> PointerToReadOnlySpan<T>(T* valuePtr, int length)
        where T : unmanaged
    {
        return new ReadOnlySpan<T>(valuePtr, length);
    }

    // in T -> 
    // These are all _really_ dangerous!

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* InToVoidPointer<T>(in T inValue)
    {
        Emit.Ldarg(nameof(inValue));
        Emit.Conv_U();
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* InToPointer<T>(in T inValue)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T InToRef<T>(in T inValue)
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> InToSpan<T>(in T inValue, int length)
    {
#if NET48 || NETSTANDARD2_0
        return new Span<T>(InToVoidPointer<T>(in inValue), length);
#else
        return MemoryMarshal.CreateSpan(ref InToRef<T>(in inValue), length);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> InToReadOnlySpan<T>(in T inValue, int length)
    {
#if NET48 || NETSTANDARD2_0
        return new ReadOnlySpan<T>(InToVoidPointer<T>(in inValue), length);
#elif NET8_0_OR_GREATER
        return MemoryMarshal.CreateReadOnlySpan(in inValue, length);
#else
        return MemoryMarshal.CreateReadOnlySpan(ref InToRef(in inValue), length);
#endif
    }

    // ref T ->

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* RefToVoidPointer<T>(ref T refValue)
    {
        Emit.Ldarg(nameof(refValue));
        Emit.Conv_U();
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* RefToPointer<T>(ref T refValue)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(refValue));
        return ReturnPointer<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> RefToSpan<T>(ref T refValue, int length)
    {
#if NET48 || NETSTANDARD2_0
        return new Span<T>(RefToVoidPointer<T>(ref refValue), length);
#else
        return MemoryMarshal.CreateSpan<T>(ref refValue, length);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> RefToReadOnlySpan<T>(ref T refValue, int length)
    {
#if NET48 || NETSTANDARD2_0
        return new ReadOnlySpan<T>(RefToVoidPointer<T>(ref refValue), length);
#else
        return MemoryMarshal.CreateReadOnlySpan<T>(ref refValue, length);
#endif
    }

    // Span<T> ->

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* SpanToVoidPointer<T>(Span<T> span)
    {
        return RefToVoidPointer(ref span.GetPinnableReference());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* SpanToPointer<T>(Span<T> span)
        where T : unmanaged
    {
        return RefToPointer(ref span.GetPinnableReference());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T SpanToRef<T>(Span<T> span)
    {
        return ref span.GetPinnableReference();
    }

    /*
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> SpanToReadOnlySpan<T>(Span<T> span)
    {
        return span;
    }
    */

    // ReadOnlySpan<T> ->
    // These are all _really_ dangerous!

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* ReadOnlySpanToVoidPointer<T>(ReadOnlySpan<T> span)
    {
        return RefToVoidPointer<T>(ref ReadOnlySpanToRef<T>(span));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* ReadOnlySpanToPointer<T>(ReadOnlySpan<T> span)
        where T : unmanaged
    {
        return RefToPointer<T>(ref ReadOnlySpanToRef(span));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T ReadOnlySpanToRef<T>(ReadOnlySpan<T> readOnlySpan)
    {
        return ref MemoryMarshal.GetReference(readOnlySpan);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> ReadOnlySpanToSpan<T>(ReadOnlySpan<T> readOnlySpan)
    {
        ref T first = ref MemoryMarshal.GetReference(readOnlySpan);
#if NET48 || NETSTANDARD2_0
        return new Span<T>(RefToVoidPointer<T>(ref first), readOnlySpan.Length);
#else
        return MemoryMarshal.CreateSpan<T>(ref first, readOnlySpan.Length);
#endif
    }

    // out T ->
    // These are all _really_ stupid + dangerous!
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T OutToRef<T>(out T outValue)
    {
        Emit.Ldarg(nameof(outValue));
        Emit.Ret();
        throw Unreachable();
    }
#endregion

#region As / Cast / Box / Unbox
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> UnmanagedToByteSpan<T>(ref T value)
        where T : unmanaged
    {
        return MemoryMarshal.AsBytes(RefToSpan(ref value, 1));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<TOut> CastSpan<TIn, TOut>(Span<TIn> inSpan)
        where TIn : struct
        where TOut : struct
    {
        return MemoryMarshal.Cast<TIn, TOut>(inSpan);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOut DirectCast<TIn, TOut>(TIn value)
    {
        Emit.Ldarg(nameof(value));
        return Return<TOut>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TOut DirectCast<TIn, TOut>(ref TIn refValue)
    {
        Emit.Ldarg(nameof(refValue));
        return ref ReturnRef<TOut>();
    }


#region Unbox
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Unbox<T>([DisallowNull] object obj)
    {
        Emit.Ldarg(nameof(obj));
        Emit.Unbox_Any<T>();
        return Return<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T UnboxRef<T>([DisallowNull] object obj)
    {
        Emit.Ldarg(nameof(obj));
        Emit.Unbox<T>();
        return ref ReturnRef<T>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(obj))]
    public static T? CastClass<T>(object? obj)
        where T : class
    {
        Emit.Ldarg(nameof(obj));
        Emit.Castclass<T>();
        return Return<T?>();
    }
#endregion
#endregion


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SkipInit<T>(out T value)
    {
        Emit.Ret();
        throw Unreachable();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOf<T>()
    {
        Emit.Sizeof<T>();
        return Return<int>();
    }

    /// <summary>
    /// Methods related to working with byte blocks
    /// </summary>
    public static class Block
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(void* sourcePtr, void* destPtr, uint byteCount)
        {
            Emit.Ldarg(nameof(destPtr));
            Emit.Ldarg(nameof(sourcePtr));
            Emit.Ldarg(nameof(byteCount));
            Emit.Cpblk();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(in byte sourceByte, ref byte destByte, int byteCount)
        {
            Emit.Ldarg(nameof(destByte));
            Emit.Ldarg(nameof(sourceByte));
            Emit.Ldarg(nameof(byteCount));
            Emit.Cpblk();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(ReadOnlySpan<byte> source, Span<byte> dest)
        {
            CopyTo(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(ReadOnlySpan<byte> source, byte[] dest)
        {
            CopyTo(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(Span<byte> source, Span<byte> dest)
        {
            CopyTo(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(Span<byte> source, byte[] dest)
        {
            CopyTo(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(byte[] source, Span<byte> dest)
        {
            CopyTo(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(byte[] source, byte[] dest)
        {
            CopyTo(in source.GetPinnableReference(),
                ref dest.GetPinnableReference(),
                source.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnalignedCopyTo(void* sourcePtr, void* destPtr, uint byteCount)
        {
            Emit.Ldarg(nameof(destPtr));
            Emit.Ldarg(nameof(sourcePtr));
            Emit.Ldarg(nameof(byteCount));
            Emit.Unaligned(1);
            Emit.Cpblk();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnalignedCopyTo(in byte sourceByte, ref byte destByte, int byteCount)
        {
            Emit.Ldarg(nameof(destByte));
            Emit.Ldarg(nameof(sourceByte));
            Emit.Ldarg(nameof(byteCount));
            Emit.Unaligned(1);
            Emit.Cpblk();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(void* startPtr, byte value, uint byteCount)
        {
            Emit.Ldarg(nameof(startPtr));
            Emit.Ldarg(nameof(value));
            Emit.Ldarg(nameof(byteCount));
            Emit.Initblk();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(ref byte startByte, byte value, int byteCount)
        {
            Emit.Ldarg(nameof(startByte));
            Emit.Ldarg(nameof(value));
            Emit.Ldarg(nameof(byteCount));
            Emit.Initblk();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnalignedInit(void* startPtr, byte value, uint byteCount)
        {
            Emit.Ldarg(nameof(startPtr));
            Emit.Ldarg(nameof(value));
            Emit.Ldarg(nameof(byteCount));
            Emit.Unaligned(1);
            Emit.Initblk();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnalignedInit(ref byte startByte, byte value, int byteCount)
        {
            Emit.Ldarg(nameof(startByte));
            Emit.Ldarg(nameof(value));
            Emit.Ldarg(nameof(byteCount));
            Emit.Unaligned(1);
            Emit.Initblk();
        }

        /// <summary>
        /// Makes an exact copy of the given <see cref="byte"/> array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] CopyBytes(byte[] bytes)
        {
            DeclareLocals(
                new LocalVar("len", typeof(int)),
                new LocalVar("newArray", typeof(byte[])));
            Emit.Ldarg(nameof(bytes));
            Emit.Ldlen();
            Emit.Stloc("len");
            Emit.Ldloc("len");
            Emit.Newarr<byte>();
            Emit.Stloc("newArray");
            Emit.Ldloca("newArray");
            Emit.Ldarga(nameof(bytes));
            Emit.Ldloc("len");
            Emit.Cpblk();
            Emit.Ldloc("newArray");
            return Return<byte[]>();
        }
    }

    /// <summary>
    /// Methods on <c>unmanaged</c> values
    /// </summary>
    public static class Unmanaged
    {
        public static T Clone<T>(in T value)
            where T : unmanaged
        {
            int size = sizeof(T);
            Span<byte> buffer = stackalloc byte[size];
            Block.CopyTo(
                InToVoidPointer(in value), 
                RefToVoidPointer(ref buffer.GetPinnableReference()), 
                (uint)size);
            return Unsafe.ReadUnaligned<T>(ref buffer.GetPinnableReference());
        }

        public static byte[] ToBytes<T>(in T value)
            where T : unmanaged
        {
            int size = sizeof(T);
            Span<byte> buffer = stackalloc byte[size];
            Block.CopyTo(
                InToVoidPointer(in value), 
                RefToVoidPointer(ref buffer.GetPinnableReference()), 
                (uint)size);
            return buffer.ToArray();
        }
    }
    

#region Offsets
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* OffsetBy<T>(void* source, int elementOffset)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Conv_I();
        Emit.Mul();
        Emit.Add();
        return ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* OffsetBy<T>(T* source, int elementOffset)
        where T : unmanaged
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Conv_I();
        Emit.Mul();
        Emit.Add();
        return ReturnPointer<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T OffsetBy<T>(ref T source, int elementOffset)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Conv_I();
        Emit.Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T OffsetBy<T>(ref T source, nint elementOffset)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* OffsetByBytes(void* source, int byteOffset)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(byteOffset));
        Emit.Add();
        return ReturnPointer();
    }
#endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreSame<T>(ref T left, ref T right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Ceq();
        return Return<bool>();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAddressGreaterThan<T>(ref T left, ref T right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Cgt_Un();
        return Return<bool>();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAddressLessThan<T>(ref T left, ref T right)
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Clt_Un();
        return Return<bool>();
    }


    /// <summary>
    /// Is the <c>ref</c> <typeparamref name="T"/> <paramref name="source"/> a reference to <c>null</c>?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(ref T source)
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        Emit.Ceq();
        return Return<bool>();
    }

    /// <summary>
    /// Gets a <c>ref (</c><typeparamref name="T"/><c>)null</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T NullRef<T>()
    {
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// Gets a <c>((</c><typeparamref name="T"/><c>)null)*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* NullPointer<T>()
        where T : unmanaged
    {
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        return ReturnPointer<T>();
    }
    
    /// <summary>
    /// Gets a <c>void*</c> to <c>null</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* NullPointer()
    {
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        return ReturnPointer();
    }
}