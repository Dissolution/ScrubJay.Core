#pragma warning disable CA1710

using System.Buffers;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

[PublicAPI]
[MustDisposeResource(true)]
public sealed partial class TextBuilder : IDisposable
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

    internal int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Length;
    }

    public ref char this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            int offset = Throw.IfBadIndex(index, _position);
            return ref _chars[offset];
        }
    }

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

#region Getters & Setters

    public Option<char> GetAt(Index index)
        => Validate
            .Index(index, _position)
            .Select(i => _chars[i])
            .AsOption();

    public RefOption<Span<char>> GetAt(Range range)
    {
        if (!Validate.Range(range, _position).IsOk(out var ol))
            return None;
        var span = _chars.AsSpan(ol.Offset, ol.Length);
        return RefOption<Span<char>>.Some(span);
    }

    public Option<char> SetAt(Index index, char ch)
    {
        return Validate.Index(index, _position)
            .Select(i => _chars[i] = ch)
            .AsOption();
    }

#endregion


    public Result<int> TryCopyTo(Span<char> destination)
    {
        int len = _position;
        if (len > destination.Length)
            return new ArgumentException($"{len} characters will not fit in a span of capacity {destination.Length}",
                nameof(destination));
        Notsafe.Text.CopyBlock(_chars, destination, len);
        return Ok(len);
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

    public TextBuilder Measure(Action<TextBuilder>? buildText, out Span<char> written)
    {
        if (buildText is not null)
        {
            int start = _position;
            buildText(this);
            int end = _position;
            written = _chars.AsSpan(start, end - start);
        }
        else
        {
            written = [];
        }

        return this;
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
        _whitespace?.Dispose();
        _position = 0;
        char[] toReturn = Reference.Exchange(ref _chars, []);
        if (toReturn.Length > 0)
        {
            ArrayPool<char>.Shared.Return(toReturn, true);
        }

        GC.SuppressFinalize(this);
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