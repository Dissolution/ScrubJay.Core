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

using ScrubJay.Maths;

#pragma warning disable CS0809, CA1720

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
    private const uint START_HASH = SEED + PRIME5;

    /// <summary>
    /// The seed for this Hasher
    /// </summary>
    private const uint SEED = 0xDEADBEEFU;

    /// <summary>
    /// The current hashcode for no value
    /// </summary>
    public static int EmptyHash { get; }

    /// <summary>
    /// The current hashcode for <c>null</c>
    /// </summary>
    public static int NullHash { get; }

    static DeterministicHasher()
    {
        var hasher = new DeterministicHasher();
        EmptyHash = hasher.ToHashCode();
        hasher.AddNull();
        NullHash = hasher.ToHashCode();
    }


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
    private static uint StateAdd(uint hash, uint input)
        => MathHelper.RotateLeft(hash + (input * PRIME2), 13) * PRIME1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint HashAdd(uint hash, uint queuedValue)
        => MathHelper.RotateLeft(hash + (queuedValue * PRIME3), 17) * PRIME4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint StateToHash(uint value1, uint value2, uint value3, uint value4)
        => MathHelper.RotateLeft(value1, 1) +
           MathHelper.RotateLeft(value2, 7) +
           MathHelper.RotateLeft(value3, 12) +
           MathHelper.RotateLeft(value4, 18);

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

    public void Add(byte u8) => Add((uint)u8);

    public void Add(sbyte i8) => Add((uint)i8);

    public void Add(short i16) => Add((uint)i16);

    public void Add(ushort u16) => Add((uint)u16);

    public void Add(int i32) => Add((uint)i32);

    public void Add(uint u32)
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

    public void Add(long i64)
    {
        Add((uint)i64); // low bits
        Add((uint)(i64 >> 32)); // high bits
    }

    public void Add(ulong u64)
    {
        Add((uint)u64); // low bits
        Add((uint)(u64 >> 32)); // high bits
    }

    public void Add(char ch)
    {
        Add((uint)ch);
    }

    public void Add(scoped text text)
    {
        Add(MemoryMarshal.Cast<char, byte>(text));
    }

    public void Add(string? str)
    {
        if (str is null)
        {
            AddNull();
        }
        else
        {
            Add(MemoryMarshal.Cast<char, byte>(str.AsSpan()));
        }
    }

    public void Add(Guid guid)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        var buffer = guid.ToByteArray();
#else
        Span<byte> buffer = stackalloc byte[16];
        guid.TryWriteBytes(buffer);
#endif
        Add(buffer);
    }

    public void Add(TimeSpan timeSpan)
    {
        Add(timeSpan.Ticks);
    }

    public void Add(DateTime dateTime)
    {
        Add(dateTime.Ticks);
        AddUnmanaged(dateTime.Kind);
    }

    public void Add(scoped ReadOnlySpan<byte> bytes)
    {
        var leftoverBytes = bytes.Length % sizeof(uint);

        switch (leftoverBytes)
        {
            case 1:
                Add(bytes[0]);
                bytes = bytes[1..];
                break;
            case 2:
                Add(MemoryMarshal.Read<ushort>(bytes[..2]));
                bytes = bytes[2..];
                break;
            case 3:
                var u32 = MemoryMarshal.Read<uint>(bytes[..4]); // cannot only read 3 bytes
                u32 >>= 8; // but we shift out the third byte
                Add(u32);
                bytes = bytes[3..]; // and we still use the fourth here
                break;
            default:
                Debug.Assert(leftoverBytes == 0);
                break;
        }

        var hashes = MemoryMarshal.Cast<byte, uint>(bytes);
        foreach (var hash in hashes)
        {
            Add(hash);
        }
    }

    public void Add<U>(scoped ReadOnlySpan<U> values)
        where U : unmanaged
    {
        Add(MemoryMarshal.Cast<U, byte>(values));
    }

    public void AddNull()
    {
        Add(0);
    }

    public void AddUnmanaged<U>(U value)
        where U : unmanaged
    {
        ReadOnlySpan<byte> span;

        unsafe
        {
            span = new ReadOnlySpan<byte>(Notsafe.InAsVoidPtr<U>(in value), sizeof(U));
        }

        Add(span);
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