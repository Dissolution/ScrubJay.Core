/*

The xxHash32 implementation is based on the code published by Yann Collet:
https://raw.githubusercontent.com/Cyan4973/xxHash/5c174cfa4e45a42f94082dc0d4539b39696afea1/xxhash.c

  xxHash - Fast Hash algorithm
  Copyright (C) 2012-2016, Yann Collet

  BSD 2-Clause License (http://www.opensource.org/licenses/bsd-license.php)

  Redistribution and use in source and binary forms, with or without
  modification, are permitted provided that the following conditions are
  met:

  * Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.
  * Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following disclaimer
  in the documentation and/or other materials provided with the
  distribution.

  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
  OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
  LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

  You can contact the author at :
  - xxHash homepage: http://www.xxhash.com
  - xxHash source repository : https://github.com/Cyan4973/xxHash

*/

#pragma warning disable CS0809

namespace ScrubJay.Utilities;

/// <summary>
/// A null-safe hashcode generator
/// </summary>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public ref struct DeterministicHasher
{
    #region Static

    private const uint PRIME1 = 0x9E3779B1U;
    private const uint PRIME2 = 0x85EBCA77U;
    private const uint PRIME3 = 0xC2B2AE3DU;
    private const uint PRIME4 = 0x27D4EB2FU;
    private const uint PRIME5 = 0x165667B1U;

    /// <summary>
    /// The seed for this Hasher
    /// </summary>
    private const uint SEED = 147U;

    /// <summary>
    /// The current hashcode for no value
    /// </summary>
    public static int EmptyHash { get; } = new DeterministicHasher().ToHashCode();

    /// <summary>
    /// The current hashcode for <c>null</c>
    /// </summary>
    public static int NullHash { get; } = Hash<object?>(null);

    private const uint START_HASH = SEED + PRIME5;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void StartingStates(out uint state1, out uint state2, out uint state3, out uint state4)
    {
        unchecked
        {
            state1 = SEED + PRIME1 + PRIME2;
            state2 = SEED + PRIME2;
            state3 = SEED;
            state4 = SEED - PRIME1;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint RotateLeft(uint value, int offset)
    {
#if NET6_0_OR_GREATER
        return BitOperations.RotateLeft(value, offset);
#else
        return (value << offset) | (value >> (32 - offset));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint StateAdd(uint hash, uint input)
        => RotateLeft(hash + (input * PRIME2), 13) * PRIME1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint HashAdd(uint hash, uint queuedValue)
        => RotateLeft(hash + (queuedValue * PRIME3), 17) * PRIME4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint StateToHash(uint value1, uint value2, uint value3, uint value4)
        => RotateLeft(value1, 1) + RotateLeft(value2, 7) + RotateLeft(value3, 12) + RotateLeft(value4, 18);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint HashFinalize(uint hash)
    {
        hash ^= (hash >> 15);
        hash *= PRIME2;
        hash ^= (hash >> 13);
        hash *= PRIME3;
        hash ^= (hash >> 16);
        return hash;
    }

    #endregion

    // current hasher states

    private uint _state1;
    private uint _state2;
    private uint _state3;
    private uint _state4;

    // mixing queue

    private uint _queue1;
    private uint _queue2;
    private uint _queue3;

    private uint _length;

    public void AddHash(byte u8) => AddHash((uint)u8);

    public void AddHash(sbyte i8) => AddHash((uint)i8);

    public void AddHash(short i16) => AddHash((uint)i16);

    public void AddHash(ushort u16) => AddHash((uint)u16);

    public void AddHash(int i32) => AddHash((uint)i32);

    public void AddHash(uint u32)
    {
        uint previousLength = _length++;
        uint position = previousLength % 4;

        // Cannot inline switch
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (position == 0)
        {
            _queue1 = u32;
        }
        else if (position == 1)
        {
            _queue2 = u32;
        }
        else if (position == 2)
        {
            _queue3 = u32;
        }
        else // position == 3
        {
            if (previousLength == 3)
            {
                StartingStates(out _state1, out _state2, out _state3, out _state4);
            }

            _state1 = StateAdd(_state1, _queue1);
            _state2 = StateAdd(_state2, _queue2);
            _state3 = StateAdd(_state3, _queue3);
            _state4 = StateAdd(_state4, u32);
        }
    }

    public void AddHash(long i64)
    {
        AddHash((uint)i64); // low bits
        AddHash((uint)(i64 >> 32)); // high bits
    }

    public void AddHash(ulong u64)
    {
        AddHash((uint)u64); // low bits
        AddHash((uint)(u64 >> 32)); // high bits
    }

    public void AddHash<U>(U value)
        where U : unmanaged
    {
        unsafe
        {
            var span  = new ReadOnlySpan<byte>(Notsafe.InAsVoidPtr<U>(in value), sizeof(U));
            Random r = default!;
            r.NextBytes();
        }
    }

    /// <summary>
    /// Adds the hashcodes of the items in a <see cref="Span{T}"/>
    /// </summary>
    public void AddMany<T>(scoped Span<T> values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            Add<T>(values[i]);
        }
    }

    /// <summary>
    /// Adds the hashcodes of the items in a <see cref="ReadOnlySpan{T}"/>
    /// </summary>
    public void AddMany<T>(scoped ReadOnlySpan<T> values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            Add<T>(values[i]);
        }
    }

    /// <summary>
    /// Adds the hashcodes generated by a <paramref name="comparer"/> for the given <paramref name="values"/> to this <see cref="Hasher"/>
    /// </summary>
    public void AddMany<T>(scoped ReadOnlySpan<T> values, IEqualityComparer<T>? comparer)
    {
        for (int i = 0; i < values.Length; i++)
        {
            Add<T>(values[i], comparer);
        }
    }

    /// <summary>
    /// Adds the hashcodes generated for the given <paramref name="values"/> to this <see cref="Hasher"/>
    /// </summary>
    public void AddMany<T>(params T[]? values)
    {
        if (values is null)
            return;
        for (int i = 0; i < values.Length; i++)
        {
            Add<T>(values[i]);
        }
    }

    /// <summary>
    /// Adds the hashcodes generated by a <paramref name="comparer"/> for the given <paramref name="values"/> to this <see cref="Hasher"/>
    /// </summary>
    public void AddMany<T>(T[]? values, IEqualityComparer<T>? comparer)
    {
        if (values is null)
            return;
        for (int i = 0; i < values.Length; i++)
        {
            Add<T>(values[i], comparer);
        }
    }

    /// <summary>
    /// Adds the hashcodes generated for the given <paramref name="values"/> to this <see cref="Hasher"/>
    /// </summary>
    public void AddMany<T>(IEnumerable<T>? values)
    {
        if (values is null)
            return;
        foreach (var value in values)
        {
            Add<T>(value);
        }
    }

    /// <summary>
    /// Adds the hashcodes generated by a <paramref name="comparer"/> for the given <paramref name="values"/> to this <see cref="Hasher"/>
    /// </summary>
    public void AddMany<T>(IEnumerable<T>? values, IEqualityComparer<T>? comparer)
    {
        if (values is null)
            return;
        foreach (var value in values)
        {
            Add<T>(value, comparer);
        }
    }

    /// <summary>
    /// Gets the hashcode generated by this <see cref="Hasher"/> instance
    /// </summary>
    /// <returns></returns>
    public readonly int ToHashCode()
    {
        uint length = _length;

        // position refers to the *next* queue position in this method
        // So: position == 1 means that _queue1 is populated and _queue2 would have been populated on the next call to Add()
        uint position = length % 4;

        // If the length is less than 4, _state1 to _state4 don't contain anything yet
        uint hash;
        if (length < 4)
        {
            hash = START_HASH;
        }
        else
        {
            hash = StateToHash(_state1, _state2, _state3, _state4);
        }

        // _length is incremented once per AddHash() and is therefore 4 times too small
        // (xxHash length is in bytes and we are using uints)
        hash += (length * 4);

        // Mix what remains in the queue
        if (position > 0)
        {
            hash = HashAdd(hash, _queue1);
            if (position > 1)
            {
                hash = HashAdd(hash, _queue2);
                if (position > 2)
                {
                    hash = HashAdd(hash, _queue3);
                }
            }
        }

        hash = HashFinalize(hash);
        return (int)hash;
    }

    [Obsolete("Use ToHashCode() to get the hash generated by this DeterministicHasher", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override readonly int GetHashCode() => ToHashCode();

    [Obsolete("DeterministicHasher cannot equate to any value", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override readonly bool Equals(object? obj) => false;

    public override readonly string ToString()
    {
        return $"{nameof(DeterministicHasher)} #{ToHashCode():X}";
    }
}