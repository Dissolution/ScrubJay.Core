using System.Buffers;

namespace Scratch;

[PublicAPI]
[MustDisposeResource(true)]
public sealed class TextBuilder :
    IFluentBuilder<TextBuilder>,
    IList<char>,
    IReadOnlyList<char>,
    ICollection<char>,
    IReadOnlyCollection<char>,
    IEnumerable<char>,
    IDisposable
{
    /// <summary>
    /// Creates a <c>new</c> <see cref="TextBuilder"/> instance
    /// </summary>
    public static TextBuilder New
    {
        [MustDisposeResource(true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new();
    }

    /// <summary>
    /// Builds a <see cref="string"/> using a temporary <see cref="TextBuilder"/> instance
    /// </summary>
    /// <param name="build">
    /// The <see cref="Action{T}"/> to invoke on a temporary <see cref="TextBuilder"/> instance
    /// </param>
    /// <returns>
    /// The <see cref="string"/> produced by calling <see cref="TextBuilder.ToString"/> on the temporary instance before disposing it
    /// </returns>
    public static string Build(Action<TextBuilder>? build)
    {
        if (build is null)
            return string.Empty;
        using var builder = new TextBuilder();
        build(builder);
        return builder.ToString();
    }

    public static string Build<S>(S state, Action<S, TextBuilder>? build)
    {
        if (build is null)
            return string.Empty;
        using var builder = new TextBuilder();
        build(state, builder);
        return builder.ToString();
    }


    private char[] _chars;
    private int _position;
    private int _version = 0;


    int ICollection<char>.Count => Length;
    int IReadOnlyCollection<char>.Count => Length;
    bool ICollection<char>.IsReadOnly => false;
    TextBuilder IFluentBuilder<TextBuilder>.Self => this;


    /// <summary>
    /// Get a <see cref="Span{T}"/> over items in this <see cref="PooledList{T}"/>
    /// </summary>
    private Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.AsSpan(0, _position);
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the unwritten, available portion of this <see cref="PooledList{T}"/>
    /// </summary>
    private Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.AsSpan(_position);
    }

    private int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Length;
    }

    public char this[int index]
    {
        get
        {
            Throw.IfBadIndex(index, _position);
            return _chars[index];
        }
        set
        {
            Throw.IfBadIndex(index, _position);
            // assume change rather than call Equals
            _version++;
            _chars[index] = value;
        }
    }

    public char this[Index index]
    {
        get
        {
            int offset = Throw.IfBadIndex(index, _position);
            return _chars[offset];
        }
        set
        {
            int offset = Throw.IfBadIndex(index, _position);
            // assume change rather than call Equals
            _version++;
            _chars[offset] = value;
        }
    }

    public Span<char> this[Range range]
    {
        get
        {
            (int offset, int length) = Throw.IfBadRange(range, _position);
            // assume changes will occur
            _version++;
            return _chars.AsSpan(offset, length);
        }
    }



    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
        private set
        {
            Debug.Assert((value >= 0) && (value < Capacity));
            _version++;
            _position = value;
        }
    }

    [MustDisposeResource(true)]
    public TextBuilder()
    {
        _chars = []; // empty array, will be expanded as needed
    }

    [MustDisposeResource(true)]
    public TextBuilder(int minCapacity)
    {
        int capacity = Math.Max(1024, minCapacity);
        _chars = ArrayPool<char>.Shared.Rent(capacity);
    }

    [HandlesResourceDisposal]
    ~TextBuilder()
    {
        this.Dispose();
    }

    void ICollection<char>.Add(char item) => Append(item);


    private void GrowBy(int adding)
    {
        Debug.Assert(adding > 0);
        if (_chars.Length == 0)
        {
            _chars = ArrayPool<char>.Shared.Rent(Math.Max(1024, adding * 2));
            return;
        }

        char[] array = ArrayPool<char>.Shared.Rent((_chars.Length + adding) * 2);
        Notsafe.Text.CopyBlock(_chars, array, _position);
        Reference.Exchange(ref _chars, array);
        ArrayPool<char>.Shared.Return(array, true);
    }

    #region Append

    #endregion Append


    public void Clear()
    {
        _position = 0;
        // We do not clear the array, that happens when it is returned
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
}