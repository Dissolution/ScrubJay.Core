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

using System.ComponentModel;
using System.Security.Cryptography;

namespace ScrubJay.Utilities;

/// <summary>
/// Hasher is a hashcode generator for use in many dotnet environments<br />
/// </summary>
public ref struct Hasher
{
    private const uint PRIME1 = 0x9E3779B1U;
    private const uint PRIME2 = 0x85EBCA77U;
    private const uint PRIME3 = 0xC2B2AE3DU;
    private const uint PRIME4 = 0x27D4EB2FU;
    private const uint PRIME5 = 0x165667B1U;

    /// <summary>
    /// The seed for this Hasher
    /// </summary>
    private static readonly uint _seed = CreateSeed();
    /// <summary>
    /// The default hashcode for no value
    /// </summary>
    private static readonly int _emptyHash = new Hasher().ToHashCode();
    /// <summary>
    /// The default hashcode for <c>null</c>
    /// </summary>
    private static readonly int _nullHash = GetHashCode<object?>(null);
    
    private static uint CreateSeed()
    {
#if NET48_OR_GREATER || NETSTANDARD2_0
        using var rng = RandomNumberGenerator.Create();
        byte[] bytes = new byte[sizeof(uint)];
        rng.GetBytes(bytes);
        return BitConverter.ToUInt32(bytes, 0);
#else
        Span<byte> bytes = stackalloc byte[sizeof(uint)];
        RandomNumberGenerator.Fill(bytes);
        return Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(bytes));
#endif
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
    private static uint StartingHash()
    {
        return _seed + PRIME5;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void StartingStates(out uint state1, out uint state2, out uint state3, out uint state4)
    {
        state1 = _seed + PRIME1 + PRIME2;
        state2 = _seed + PRIME2;
        state3 = _seed;
        state4 = _seed - PRIME1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint StateAdd(uint hash, uint input)
    {
        return RotateLeft(hash + (input * PRIME2), 13) * PRIME1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint HashAdd(uint hash, uint queuedValue)
    {
        return RotateLeft(hash + (queuedValue * PRIME3), 17) * PRIME4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint StateToHash(uint value1, uint value2, uint value3, uint value4)
    {
        return RotateLeft(value1, 1) + RotateLeft(value2, 7) + RotateLeft(value3, 12) + RotateLeft(value4, 18);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint HashFinalize(uint hash)
    {
        hash ^= hash >> 15;
        hash *= PRIME2;
        hash ^= hash >> 13;
        hash *= PRIME3;
        hash ^= hash >> 16;
        return hash;
    }


    /// <summary>
    /// Gets the hashcode for a single <paramref name="value"/>
    /// </summary>
    public static int GetHashCode<T>(T? value)
    {
        var hc1 = (uint)(value?.GetHashCode() ?? 0);

        uint hash = StartingHash();
        hash += 4;

        hash = HashAdd(hash, hc1);
        
        hash = HashFinalize(hash);
        return (int)hash;
    }

    /// <summary>
    /// Gets a combined hashcode for the given values
    /// </summary>
    public static int Combine<T1, T2>(T1? value1, T2? value2)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);

        uint hash = StartingHash();
        hash += 8;

        hash = HashAdd(hash, hc1);
        hash = HashAdd(hash, hc2);
        
        hash = HashFinalize(hash);
        return (int)hash;
    }

    /// <summary>
    /// Gets a combined hashcode for the given values
    /// </summary>
    public static int Combine<T1, T2, T3>(T1? value1, T2? value2, T3? value3)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);

        uint hash = StartingHash();
        hash += 12;

        hash = HashAdd(hash, hc1);
        hash = HashAdd(hash, hc2);
        hash = HashAdd(hash, hc3);

        hash = HashFinalize(hash);
        return (int)hash;
    }

    /// <summary>
    /// Gets a combined hashcode for the given values
    /// </summary>
    public static int Combine<T1, T2, T3, T4>(T1? value1, T2? value2, T3? value3, T4? value4)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);
        var hc4 = (uint)(value4?.GetHashCode() ?? 0);

        StartingStates(out uint state1, out uint state2, out uint state3, out uint state4);

        state1 = StateAdd(state1, hc1);
        state2 = StateAdd(state2, hc2);
        state3 = StateAdd(state3, hc3);
        state4 = StateAdd(state4, hc4);

        uint hash = StateToHash(state1, state2, state3, state4);
        hash += 16;

        hash = HashFinalize(hash);
        return (int)hash;
    }

    /// <summary>
    /// Gets a combined hashcode for the given values
    /// </summary>
    public static int Combine<T1, T2, T3, T4, T5>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);
        var hc4 = (uint)(value4?.GetHashCode() ?? 0);
        var hc5 = (uint)(value5?.GetHashCode() ?? 0);

        StartingStates(out uint state1, out uint state2, out uint state3, out uint state4);

        state1 = StateAdd(state1, hc1);
        state2 = StateAdd(state2, hc2);
        state3 = StateAdd(state3, hc3);
        state4 = StateAdd(state4, hc4);

        uint hash = StateToHash(state1, state2, state3, state4);
        hash += 20;

        hash = HashAdd(hash, hc5);

        hash = HashFinalize(hash);
        return (int)hash;
    }

    /// <summary>
    /// Gets a combined hashcode for the given values
    /// </summary>
    public static int Combine<T1, T2, T3, T4, T5, T6>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);
        var hc4 = (uint)(value4?.GetHashCode() ?? 0);
        var hc5 = (uint)(value5?.GetHashCode() ?? 0);
        var hc6 = (uint)(value6?.GetHashCode() ?? 0);

        StartingStates(out uint state1, out uint state2, out uint state3, out uint state4);

        state1 = StateAdd(state1, hc1);
        state2 = StateAdd(state2, hc2);
        state3 = StateAdd(state3, hc3);
        state4 = StateAdd(state4, hc4);

        uint hash = StateToHash(state1, state2, state3, state4);
        hash += 24;

        hash = HashAdd(hash, hc5);
        hash = HashAdd(hash, hc6);

        hash = HashFinalize(hash);
        return (int)hash;
    }

    /// <summary>
    /// Gets a combined hashcode for the given values
    /// </summary>
    public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, T7? value7)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);
        var hc4 = (uint)(value4?.GetHashCode() ?? 0);
        var hc5 = (uint)(value5?.GetHashCode() ?? 0);
        var hc6 = (uint)(value6?.GetHashCode() ?? 0);
        var hc7 = (uint)(value7?.GetHashCode() ?? 0);

        StartingStates(out uint state1, out uint state2, out uint state3, out uint state4);

        state1 = StateAdd(state1, hc1);
        state2 = StateAdd(state2, hc2);
        state3 = StateAdd(state3, hc3);
        state4 = StateAdd(state4, hc4);

        uint hash = StateToHash(state1, state2, state3, state4);
        hash += 28;

        hash = HashAdd(hash, hc5);
        hash = HashAdd(hash, hc6);
        hash = HashAdd(hash, hc7);

        hash = HashFinalize(hash);
        return (int)hash;
    }

    /// <summary>
    /// Gets a combined hashcode for the given values
    /// </summary>
    public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, T7? value7,
        T8? value8)
    {
        var hc1 = (uint)(value1?.GetHashCode() ?? 0);
        var hc2 = (uint)(value2?.GetHashCode() ?? 0);
        var hc3 = (uint)(value3?.GetHashCode() ?? 0);
        var hc4 = (uint)(value4?.GetHashCode() ?? 0);
        var hc5 = (uint)(value5?.GetHashCode() ?? 0);
        var hc6 = (uint)(value6?.GetHashCode() ?? 0);
        var hc7 = (uint)(value7?.GetHashCode() ?? 0);
        var hc8 = (uint)(value8?.GetHashCode() ?? 0);

        StartingStates(out uint state1, out uint state2, out uint state3, out uint state4);

        state1 = StateAdd(state1, hc1);
        state2 = StateAdd(state2, hc2);
        state3 = StateAdd(state3, hc3);
        state4 = StateAdd(state4, hc4);

        state1 = StateAdd(state1, hc5);
        state2 = StateAdd(state2, hc6);
        state3 = StateAdd(state3, hc7);
        state4 = StateAdd(state4, hc8);

        uint hash = StateToHash(state1, state2, state3, state4);
        hash += 32;

        hash = HashFinalize(hash);
        return (int)hash;
    }

    /// <summary>
    /// Gets a hashcode generated from all the items in the given <paramref name="span"/>
    /// </summary>
    public static int Combine<T>(ReadOnlySpan<T> span)
    {
        switch (span.Length)
        {
            case 0: return _emptyHash;
            case 1: return GetHashCode(span[0]);
            case 2: return Combine(span[0], span[1]);
            case 3: return Combine(span[0], span[1], span[2]);
            case 4: return Combine(span[0], span[1], span[2], span[3]);
            case 5: return Combine(span[0], span[1], span[2], span[3], span[4]);
            case 6: return Combine(span[0], span[1], span[2], span[3], span[4], span[5]);
            case 7: return Combine(span[0], span[1], span[2], span[3], span[4], span[5], span[6]);
            case 8: return Combine(span[0], span[1], span[2], span[3], span[4], span[5], span[6], span[7]);
            default:
            {
                var hasher = new Hasher();
                hasher.AddMany<T>(span);
                return hasher.ToHashCode();
            }
        }
    }

    /// <summary>
    /// Gets a hashcode generated by <paramref name="comparer"/> from all the items in the given <paramref name="span"/>
    /// </summary>
    public static int Combine<T>(ReadOnlySpan<T> span, IEqualityComparer<T>? comparer)
    {
        var hasher = new Hasher();
        hasher.AddMany<T>(span, comparer);
        return hasher.ToHashCode();
    }

    /// <summary>
    /// Gets a hashcode generated from all the items in the given <paramref name="array"/>
    /// </summary>
    public static int Combine<T>(params T[]? array)
    {
        if (array is null) return _nullHash;
        switch (array.Length)
        {
            case 0: return _emptyHash;
            case 1: return GetHashCode(array[0]);
            case 2: return Combine(array[0], array[1]);
            case 3: return Combine(array[0], array[1], array[2]);
            case 4: return Combine(array[0], array[1], array[2], array[3]);
            case 5: return Combine(array[0], array[1], array[2], array[3], array[4]);
            case 6: return Combine(array[0], array[1], array[2], array[3], array[4], array[5]);
            case 7: return Combine(array[0], array[1], array[2], array[3], array[4], array[5], array[6]);
            case 8: return Combine(array[0], array[1], array[2], array[3], array[4], array[5], array[6], array[7]);
            default:
            {
                var hasher = new Hasher();
                hasher.AddMany<T>(array);
                return hasher.ToHashCode();
            }
        }
    }

    /// <summary>
    /// Gets a hashcode generated by <paramref name="comparer"/> from all the items in the given <paramref name="array"/>
    /// </summary>
    public static int Combine<T>(T[]? array, IEqualityComparer<T>? comparer)
    {
        if (array is null) return _nullHash;
        var hasher = new Hasher();
        hasher.AddMany<T>(array, comparer);
        return hasher.ToHashCode();
    }

    /// <summary>
    /// Gets a hashcode generated from all the items in the given <paramref name="enumerable"/>
    /// </summary>
    public static int Combine<T>(IEnumerable<T>? enumerable)
    {
        if (enumerable is null) return _nullHash;
        var hasher = new Hasher();
        hasher.AddMany<T>(enumerable);
        return hasher.ToHashCode();
    }

    /// <summary>
    /// Gets a hashcode generated by <paramref name="comparer"/> from all the items in the given <paramref name="enumerable"/>
    /// </summary>
    public static int Combine<T>(IEnumerable<T>? enumerable, IEqualityComparer<T>? comparer)
    {
        if (enumerable is null) return _nullHash;
        var hasher = new Hasher();
        hasher.AddMany<T>(enumerable, comparer);
        return hasher.ToHashCode();
    }



    private uint _state1;
    private uint _state2;
    private uint _state3;
    private uint _state4;
    
    private uint _queue1;
    private uint _queue2;
    private uint _queue3;
    
    private uint _length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddHash(int value)
    {
        uint val = (uint)value;
        uint previousLength = _length++;
        uint position = previousLength % 4;

        if (position == 0)
            _queue1 = val;
        else if (position == 1)
            _queue2 = val;
        else if (position == 2)
            _queue3 = val;
        else // position == 3
        {
            if (previousLength == 3)
                StartingStates(out _state1, out _state2, out _state3, out _state4);

            _state1 = StateAdd(_state1, _queue1);
            _state2 = StateAdd(_state2, _queue2);
            _state3 = StateAdd(_state3, _queue3);
            _state4 = StateAdd(_state4, val);
        }
    }

    /// <summary>
    /// Adds the hashcode for the given <paramref name="value"/> to this <see cref="Hasher"/>
    /// </summary>
    public void Add<T>(T? value)
    {
        if (value is null)
        {
            AddHash(0);
        }
        else
        {
            AddHash(value.GetHashCode());
        }
    }

    /// <summary>
    /// Adds the hashcode generated by a <paramref name="comparer"/> for the given <paramref name="value"/> to this <see cref="Hasher"/>
    /// </summary>
    public void Add<T>(T? value, IEqualityComparer<T>? comparer)
    {
        if (value is null)
        {
            AddHash(0);
        }
        else if (comparer is not null)
        {
            AddHash(comparer.GetHashCode(value));
        }
        else
        {
            AddHash(value.GetHashCode());
        }
    }

    /// <summary>
    /// Adds the hashcodes generated for the given <paramref name="values"/> to this <see cref="Hasher"/>
    /// </summary>
    public void AddMany<T>(scoped ReadOnlySpan<T> values)
    {
        for (var i = 0; i < values.Length; i++)
        {
            Add<T>(values[i]);
        }
    }

    /// <summary>
    /// Adds the hashcodes generated by a <paramref name="comparer"/> for the given <paramref name="values"/> to this <see cref="Hasher"/>
    /// </summary>
    public void AddMany<T>(scoped ReadOnlySpan<T> values, IEqualityComparer<T>? comparer)
    {
        for (var i = 0; i < values.Length; i++)
        {
            Add<T>(values[i], comparer);
        }
    }

    /// <summary>
    /// Adds the hashcodes generated for the given <paramref name="values"/> to this <see cref="Hasher"/>
    /// </summary>
    public void AddMany<T>(params T[]? values)
    {
        if (values is null) return;
        for (var i = 0; i < values.Length; i++)
        {
            Add<T>(values[i]);
        }
    }

    /// <summary>
    /// Adds the hashcodes generated by a <paramref name="comparer"/> for the given <paramref name="values"/> to this <see cref="Hasher"/>
    /// </summary>
    public void AddMany<T>(T[]? values, IEqualityComparer<T>? comparer)
    {
        if (values is null) return;
        for (var i = 0; i < values.Length; i++)
        {
            Add<T>(values[i], comparer);
        }
    }

    /// <summary>
    /// Adds the hashcodes generated for the given <paramref name="values"/> to this <see cref="Hasher"/>
    /// </summary>
    public void AddMany<T>(IEnumerable<T>? values)
    {
        if (values is null) return;
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
        if (values is null) return;
        foreach (var value in values)
        {
            Add<T>(value, comparer);
        }
    }

    /// <summary>
    /// Gets the hashcode generated by this <see cref="Hasher"/> instance
    /// </summary>
    /// <returns></returns>
    public int ToHashCode()
    {
        uint length = _length;

        // position refers to the *next* queue position in this method, so position == 1 means that _queue1 is populated;
        // _queue2 would have been populated on the next call to Add()
        uint position = length % 4;

        // If the length is less than 4, _state1 to _state4 don't contain anything yet
        uint hash = length < 4 ? StartingHash() : StateToHash(_state1, _state2, _state3, _state4);

        // _length is incremented once per AddHash() and is therefore 4 times too small (xxHash length is in bytes, not ints).
        hash += length * 4;

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

#pragma warning disable CS0809
    [Obsolete("Use ToHashCode to retrieve the computed hash code", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException();

    [Obsolete("Hasher is a mutable struct and should not be compared", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? _) => throw new NotSupportedException();
#pragma warning restore CS0809

    public override string ToString()
    {
        return $"Hasher: {ToHashCode()}";
    }
}