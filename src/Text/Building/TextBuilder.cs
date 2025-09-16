#pragma warning disable CA1710

using System.Buffers;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

[PublicAPI]
[MustDisposeResource(true)]
public sealed partial class TextBuilder :
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IFormattable,
    IRenderable,
    IDisposable
{
    // character array rented from ArrayPool<char>.Shared
    private char[] _chars;

    // next write position in _chars
    private int _position;

    /// <summary>
    /// Get a <see cref="Span{T}"/> of written <see cref="char">chars</see> in this <see cref="TextBuilder"/>
    /// </summary>
    internal Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.AsSpan(0, _position);
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> of the unwritten, available portion of this <see cref="TextBuilder"/>
    /// </summary>
    internal Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.AsSpan(_position);
    }

    /// <summary>
    /// Gets the current capacity for this <see cref="TextBuilder"/> to store characters;<br/>
    /// this will automatically be increased as needed.
    /// </summary>
    internal int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Length;
    }

    /// <summary>
    /// Gets a <c>ref</c> to the written <see cref="char"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">
    /// The <see cref="Index"/> of the character to reference
    /// </param>
    public ref char this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            int offset = Throw.IfBadIndex(index, _position);
            return ref _chars[offset];
        }
    }

    /// <summary>
    /// Gets a <see cref="Span{T}">Span&lt;char&gt;</see> over the <paramref name="range"/> of written characters
    /// </summary>
    /// <param name="range">
    /// The <see cref="Range"/> of the characters to reference
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the given <paramref name="range"/> is invalid
    /// </exception>
    public Span<char> this[Range range]
    {
        get
        {
            (int offset, int length) = Throw.IfBadRange(range, _position);
            return _chars.AsSpan(offset, length);
        }
    }

    /// <summary>
    /// Gets the current count of characters that have been written to this <see cref="TextBuilder"/>
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
        internal set
        {
            Debug.Assert((value >= 0) && (value < Capacity));
            _position = value;
        }
    }

    /// <summary>
    /// Create a new <see cref="TextBuilder"/> instance
    /// </summary>
    [MustDisposeResource(true)]
    public TextBuilder()
    {
        _chars = [];
    }

    /// <summary>
    /// Create a new <see cref="TextBuilder"/> instance with a minimum starting Capacity
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum starting capacity the TextBuilder instance will have (it may be higher)
    /// </param>
    [MustDisposeResource(true)]
    public TextBuilder(int minCapacity)
    {
        int capacity = Math.Max(1024, minCapacity);
        _chars = ArrayPool<char>.Shared.Rent(capacity);
    }

    [HandlesResourceDisposal]
    ~TextBuilder() => Dispose();

    public Option<char> GetAt(Index index)
        => Validate
            .Index(index, _position)
            .Select(i => _chars[i])
            .AsOption();

    public Option<char> SetAt(Index index, char ch)
    {
        return Validate.Index(index, _position)
            .Select(i => _chars[i] = ch)
            .AsOption();
    }


    public Span<char> Slice(int index)
    {
        Validate.Index(index, _position).ThrowIfError();
        return _chars.AsSpan(index.._position);
    }

    public Span<char> Slice(Index index)
    {
        int offset = Validate.Index(index, _position).OkOrThrow();
        return _chars.AsSpan(offset.._position);
    }

    public Span<char> Slice(int index, int count)
    {
        Validate.IndexLength(index, count, _position).ThrowIfError();
        return _chars.AsSpan(index, count);
    }

    public Span<char> Slice(Index index, int count)
    {
        (int offset, int len) = Validate.IndexLength(index, count, _position).OkOrThrow();
        return _chars.AsSpan(offset, len);
    }

    public Span<char> Slice(Range range)
    {
        (int offset, int len) = Validate.Range(range, _position).OkOrThrow();
        return _chars.AsSpan(offset, len);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AsSpan() => _chars.AsSpan(0, _position);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public text AsText() => new text(_chars, 0, _position);

    public char[] ToArray() => _chars.Slice(0, _position);

    public Result<int> TryCopyTo(Span<char> destination)
    {
        int len = _position;
        if (Validate.CanCopyTo(destination, len).IsError(out var error))
            return error;
        Notsafe.Text.CopyBlock(_chars, destination, len);
        return Ok(len);
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            null => false,
            string str => TextHelper.Equate(Written, str),
            char[] chars => TextHelper.Equate(Written, chars),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return Hasher.HashMany(Written);
    }

    [HandlesResourceDisposal]
    public void Dispose()
    {
        _position = 0;
        char[] toReturn = Reference.Exchange(ref _chars, []);
        if (toReturn.Length > 0)
        {
            ArrayPool<char>.Shared.Return(toReturn, true);
        }

        GC.SuppressFinalize(this);
    }

    public TextBuilder RenderTo(TextBuilder builder)
    {
        return builder.Append(Written);
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        text format = default,
        IFormatProvider? provider = default)
    {
        int len = _position;
        if (len <= destination.Length)
        {
            Notsafe.Text.CopyBlock(_chars, destination, len);
            charsWritten = len;
            return true;
        }
        else
        {
            charsWritten = 0;
            return false;
        }
    }

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        return Written.AsString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Written.AsString();

    [HandlesResourceDisposal]
    public string ToStringAndDispose()
    {
        string str = ToString();
        Dispose();
        return str;
    }
}