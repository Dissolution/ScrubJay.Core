#pragma warning disable CA1710

using System.Buffers;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

[PublicAPI]
[MustDisposeResource(true)]
public sealed partial class TextBuilder :
    IFluentBuilder<TextBuilder>,
    IList<char>,
    IReadOnlyList<char>,
    ICollection<char>,
    IReadOnlyCollection<char>,
    IEnumerable<char>,
    IDisposable
{
    public delegate void BuildSegment<T>(TextBuilder builder, scoped ReadOnlySpan<T> segment)
        where T : IEquatable<T>;


    // Character array rented from array pool
    private char[] _chars;

    // Position in _chars that is next to be written to
    private int _position;

    int ICollection<char>.Count => Length;
    int IReadOnlyCollection<char>.Count => Length;
    bool ICollection<char>.IsReadOnly => false;
    TextBuilder IFluentBuilder<TextBuilder>.Self => this;

    /// <summary>
    /// Get a <see cref="Span{T}"/> over items in this <see cref="PooledList{T}"/>
    /// </summary>
    internal Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.AsSpan(0, _position);
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the unwritten, available portion of this <see cref="PooledList{T}"/>
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
            _chars[offset] = value;
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

    [MustDisposeResource]
    public TextBuilder()
    {
        _chars = [];
    }

    [MustDisposeResource(true)]
    public TextBuilder(int minCapacity)
    {
        int capacity = Math.Max(1024, minCapacity);
        _chars = ArrayPool<char>.Shared.Rent(capacity);
    }

    [HandlesResourceDisposal]
    ~TextBuilder() => Dispose();

    void ICollection<char>.Add(char item) => Append(item);


#region Invoke + ForEach

    public TextBuilder Invoke(Action<TextBuilder>? buildText)
    {
        if (buildText is not null)
        {
            buildText(this);
        }

        return this;
    }

    public TextBuilder Invoke<S>(S state, Action<TextBuilder, S>? buildText)
    {
        if (buildText is not null)
        {
            buildText(this, state);
        }

        return this;
    }

    public TextBuilder Invoke<R>(Func<TextBuilder, R>? buildText)
    {
        if (buildText is not null)
        {
            _ = buildText.Invoke(this);
        }

        return this;
    }

    public TextBuilder Invoke<S, R>(S state, Func<TextBuilder, S, R>? buildText)
    {
        if (buildText is not null)
        {
            _ = buildText.Invoke(this, state);
        }

        return this;
    }

#endregion

#region Getters & Setters

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

#endregion


    public Span<char> Allocate(int length)
    {
        Throw.IfLessThan(length, 0);

        int pos = _position;
        int newPos = pos + length;
        if (newPos > Capacity)
        {
            GrowBy(length);
        }

        Span<char> slice = _chars.Slice(pos, length);
        Notsafe.Text.ClearBlock(slice);
        _position = newPos;
        return slice;
    }

    public Span<char> AllocateAt(Index index, int length)
    {
        int i = Throw.IfBadInsertIndex(index, _position);
        Throw.IfLessThan(length, 0);
        if (i == _position)
            return Allocate(length);

        int pos = _position;
        int newPos = pos + length;
        if (newPos > Capacity)
        {
            GrowBy(length);
        }

        // slide right left
        Notsafe.Text.CopyBlock(
            _chars.AsSpan(i, pos - i),
            _chars.AsSpan(i + length),
            pos - i);
        Span<char> slice = _chars.Slice(i, length);
        Notsafe.Text.ClearBlock(slice);
        _position = newPos;
        return slice;
    }

    void ICollection<char>.CopyTo(char[] array, int arrayIndex)
    {
        Validate.CanCopyTo(array, arrayIndex, _position).ThrowIfError();
        Notsafe.Text.CopyBlock(Written, _chars.AsSpan(arrayIndex), _position);
    }

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

    public readonly ref struct Payload<T>
    {
        public static implicit operator Payload<T>(T? value) => new(value);
        public static implicit operator Payload<T>(ValueTuple<T?, string> tuple) => new(tuple.Item1, tuple.Item2);

        public static implicit operator Payload(Payload<T> payload) =>
            new Payload($"___format___{payload._format}", payload._value);

        private readonly T? _value;
        private readonly text _format;

        private Payload(T? value, text format = default)
        {
            _value = value;
            _format = format;
        }
#if NETFRAMEWORK || NETSTANDARD2_0
        private Payload(T? value, string? format)
        {
            _value = value;
            _format = format.AsSpan();
        }
#endif
    }

    public readonly ref struct Payload
    {
        public static implicit operator Payload(in char ch) => Append(in ch);
        public static implicit operator Payload(text text) => Append(text);
        public static implicit operator Payload(string? str) => Append(str);
        public static implicit operator Payload(char[]? chars) => Append(chars);
        public static implicit operator Payload(Action<TextBuilder>? buildText) => Invoke(buildText);

        public static Payload Append(in char ch) => new(ch.AsSpan());
        public static Payload Append(text text) => new(text);
        public static Payload Append(string? str) => new(str.AsSpan());
        public static Payload Append(params char[]? chars) => new(chars.AsSpan());
        public static Payload Invoke(Action<TextBuilder>? buildText) => new([], buildText);
        public static Payload Render<T>(T? value) => new("___render___", (object?)value);
        public static Payload Format<T>(T? value, string? format) => new(format, (object?)value);
        public static Payload Format<T>(T? value, text format) => new(format, (object?)value);


        private readonly text _text;
        private readonly object? _object;

        internal Payload(text text, object? obj = null)
        {
            _text = text;
            _object = obj;
        }

#if NETFRAMEWORK || NETSTANDARD2_0
        internal Payload(string? str, object? obj)
        {
            _text = str.AsSpan();
            _object = obj;
        }
#endif

        public void Deconstruct(out text text, out object? obj)
        {
            text = _text;
            obj = _object;
        }
    }

    public TextBuilder Accept(in Payload payload)
    {
        var (text, obj) = payload;

        if (text.Equate("___render___"))
            return Render(obj);

        if (obj is null)
        {
            return Append(text);
        }

        return Format(obj, text);
    }

    public TextBuilder If(bool condition, Payload onTrue = default, Payload onFalse = default)
    {
        throw new NotImplementedException();
    }


#region AppendIf, FormatIf

    public TextBuilder IfAppend(bool condition, char trueChar)
    {
        if (condition)
            return Append(trueChar);
        return this;
    }

    public TextBuilder IfAppend(bool condition, char trueChar, char falseChar)
    {
        if (condition)
            return Append(trueChar);
        else
            return Append(falseChar);
    }

    public TextBuilder IfAppend(bool condition, scoped text trueText)
    {
        if (condition)
            return Append(trueText);
        return this;
    }

    public TextBuilder IfAppend(bool condition, scoped text trueText, scoped text falseText)
    {
        if (condition)
            return Append(trueText);
        else
            return Append(falseText);
    }

    public TextBuilder IfAppend(bool condition, string? trueStr = null, string? falseStr = null)
    {
        if (condition)
            return Append(trueStr);
        else
            return Append(falseStr);
    }

    public TextBuilder IfFormat<T>(bool condition,
        T? trueValue,
        string? format = null,
        IFormatProvider? provider = null)
    {
        if (condition)
            return Format<T>(trueValue, format, provider);
        return this;
    }

    public TextBuilder IfFormat<T>(bool condition,
        T? trueValue,
        T? falseValue,
        string? format = null,
        IFormatProvider? provider = null)
    {
        if (condition)
            return Format<T>(trueValue, format, provider);
        return Format<T>(falseValue, format, provider);
    }

    public TextBuilder IfFormat<T, F>(bool condition,
        T? trueValue,
        F? falseValue,
        string? format = null,
        IFormatProvider? provider = null)
    {
        if (condition)
            return Format<T>(trueValue, format, provider);
        return Format<F>(falseValue, format, provider);
    }

    public TextBuilder IfFormat<T>(Option<T> option, string? format = null, IFormatProvider? provider = null)
    {
        if (option.IsSome(out var value))
            return Format<T>(value, format, provider);
        return this;
    }

    public TextBuilder IfFormat<T>(Result<T> result, string? format = null, IFormatProvider? provider = null)
    {
        return result.Match(
            ok => Format<T>(ok, format, provider),
            _ => this);
    }

    public TextBuilder IfFormat<T, E>(Result<T, E> result, string? format = null, IFormatProvider? provider = null)
    {
        return result.Match(
            ok => Format<T>(ok, format, provider),
            _ => this);
    }

#endregion

#region FormatSome, FormatOk, FormatError, RenderSome, RenderOk, RenderError

    public TextBuilder FormatSome<T>(Option<T> option, string? format = null, IFormatProvider? provider = null)
    {
        if (option.IsSome(out var value))
            return Format<T>(value, format, provider);
        return this;
    }

    public TextBuilder FormatOk<T>(Result<T> result, string? format = null, IFormatProvider? provider = null)
    {
        if (result.IsOk(out var value))
            return Format<T>(value, format, provider);
        return this;
    }

    public TextBuilder FormatOk<T, E>(Result<T, E> result, string? format = null, IFormatProvider? provider = null)
    {
        if (result.IsOk(out var value))
            return Format<T>(value, format, provider);
        return this;
    }

    public TextBuilder FormatError<T>(Result<T> result, string? format = null, IFormatProvider? provider = null)
    {
        if (result.IsError(out var error))
            return Format<Exception>(error, format, provider);
        return this;
    }

    public TextBuilder FormatError<T, E>(Result<T, E> result, string? format = null, IFormatProvider? provider = null)
    {
        if (result.IsError(out var error))
            return Format<E>(error, format, provider);
        return this;
    }

    public TextBuilder RenderSome<T>(Option<T> option)
    {
        if (option.IsSome(out var value))
            return Render<T>(value);
        return this;
    }

    public TextBuilder RenderOk<T>(Result<T> result)
    {
        if (result.IsOk(out var value))
            return Render<T>(value);
        return this;
    }

    public TextBuilder RenderOk<T, E>(Result<T, E> result)
    {
        if (result.IsOk(out var value))
            return Render<T>(value);
        return this;
    }

    public TextBuilder RenderError<T>(Result<T> result)
    {
        if (result.IsError(out var error))
            return Render<Exception>(error);
        return this;
    }

    public TextBuilder RenderError<T, E>(Result<T, E> result)
    {
        if (result.IsError(out var error))
            return Render<E>(error);
        return this;
    }

#endregion


#region Enumerate

    public TextBuilder Enumerate<T>(scoped ReadOnlySpan<T> values, Action<TextBuilder, T> buildValue)
    {
        foreach (var t in values)
        {
            buildValue(this, t);
        }

        return this;
    }

    public TextBuilder Enumerate<T>(scoped Span<T> values, Action<TextBuilder, T> buildValue)
    {
        foreach (var t in values)
        {
            buildValue(this, t);
        }

        return this;
    }

    public TextBuilder Enumerate<T>(T[]? values, Action<TextBuilder, T> buildValue)
    {
        if (values is not null)
        {
            foreach (var t in values)
            {
                buildValue(this, t);
            }
        }

        return this;
    }

    public TextBuilder Enumerate<T>(IEnumerable<T>? values, Action<TextBuilder, T> buildValue)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                buildValue(this, value);
            }
        }

        return this;
    }

    public TextBuilder Enumerate(string? str, Action<TextBuilder, char> buildValue)
    {
        if (str is not null)
        {
            foreach (char ch in str)
            {
                buildValue(this, ch);
            }
        }

        return this;
    }

    public TextBuilder Enumerate<T>(SpanSplitter<T> splitSpan, BuildSegment<T> buildSegment)
        where T : IEquatable<T>
    {
        while (splitSpan.MoveNext())
        {
            buildSegment(this, splitSpan.Current);
        }

        return this;
    }

#endregion

#region EnumerateFormat, EnumerateRender

    public TextBuilder EnumerateFormat<T>(scoped ReadOnlySpan<T> values,
        string? format = null,
        IFormatProvider? provider = null)
        => Enumerate<T>(values, (tb, value) => tb.Format<T>(value, format, provider));

    public TextBuilder EnumerateFormat<T>(scoped Span<T> values,
        string? format = null,
        IFormatProvider? provider = null)
        => Enumerate<T>(values, (tb, value) => tb.Format<T>(value, format, provider));

    public TextBuilder EnumerateFormat<T>(T[]? values,
        string? format = null,
        IFormatProvider? provider = null)
        => Enumerate<T>(values, (tb, value) => tb.Format<T>(value, format, provider));

    public TextBuilder EnumerateFormat<T>(IEnumerable<T>? values,
        string? format = null,
        IFormatProvider? provider = null)
        => Enumerate<T>(values, (tb, value) => tb.Format<T>(value, format, provider));

    public TextBuilder EnumerateRender<T>(scoped ReadOnlySpan<T> values)
        => Enumerate<T>(values, static (tb, value) => tb.Render<T>(value));

    public TextBuilder EnumerateRender<T>(scoped Span<T> values)
        => Enumerate<T>(values, static (tb, value) => tb.Render<T>(value));

    public TextBuilder EnumerateRender<T>(T[]? values)
        => Enumerate<T>(values, static (tb, value) => tb.Render<T>(value));

    public TextBuilder EnumerateRender<T>(IEnumerable<T>? values)
        => Enumerate<T>(values, static (tb, value) => tb.Render<T>(value));

#endregion

#region EnumerateAndDelimit

    public TextBuilder EnumerateAndDelimit<T>(
        scoped ReadOnlySpan<T> values,
        Action<TextBuilder, T> buildValue,
        Action<TextBuilder> buildDelimiter)
    {
        int len = values.Length;
        if (len == 0)
            return this;
        buildValue(this, values[0]);
        for (int i = 1; i < len; i++)
        {
            buildDelimiter(this);
            buildValue(this, values[i]);
        }

        return this;
    }

    public TextBuilder EnumerateAndDelimit<T>(
        scoped Span<T> values,
        Action<TextBuilder, T> buildValue,
        Action<TextBuilder> buildDelimiter)
    {
        int len = values.Length;
        if (len == 0)
            return this;
        buildValue(this, values[0]);
        for (int i = 1; i < len; i++)
        {
            buildDelimiter(this);
            buildValue(this, values[i]);
        }

        return this;
    }

    public TextBuilder EnumerateAndDelimit<T>(
        T[]? values,
        Action<TextBuilder, T> buildValue,
        Action<TextBuilder> buildDelimiter)
    {
        if (values is not null)
        {
            int len = values.Length;
            if (len == 0)
                return this;
            buildValue(this, values[0]);
            for (int i = 1; i < len; i++)
            {
                buildDelimiter(this);
                buildValue(this, values[i]);
            }
        }

        return this;
    }

    public TextBuilder EnumerateAndDelimit<T>(
        IList<T>? values,
        Action<TextBuilder, T> buildValue,
        Action<TextBuilder> buildDelimiter)
    {
        if (values is not null)
        {
            int len = values.Count;
            if (len == 0)
                return this;
            buildValue(this, values[0]);
            for (int i = 1; i < len; i++)
            {
                buildDelimiter(this);
                buildValue(this, values[i]);
            }
        }

        return this;
    }

    public TextBuilder EnumerateAndDelimit<T>(
        IEnumerable<T>? values,
        Action<TextBuilder, T> buildValue,
        Action<TextBuilder> buildDelimiter)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;
            buildValue(this, e.Current);
            while (e.MoveNext())
            {
                buildDelimiter(this);
                buildValue(this, e.Current);
            }
        }

        return this;
    }

#region char delimiter

    public TextBuilder EnumerateAndDelimit<T>(
        scoped ReadOnlySpan<T> values,
        Action<TextBuilder, T> buildValue,
        char delimiter)
        => EnumerateAndDelimit(values, buildValue, tb => tb.Append(delimiter));

    public TextBuilder EnumerateAndDelimit<T>(
        scoped Span<T> values,
        Action<TextBuilder, T> buildValue,
        char delimiter)
        => EnumerateAndDelimit(values, buildValue, tb => tb.Append(delimiter));

    public TextBuilder EnumerateAndDelimit<T>(
        T[]? values,
        Action<TextBuilder, T> buildValue,
        char delimiter)
        => EnumerateAndDelimit(values, buildValue, tb => tb.Append(delimiter));

    public TextBuilder EnumerateAndDelimit<T>(
        IList<T>? values,
        Action<TextBuilder, T> buildValue,
        char delimiter)
        => EnumerateAndDelimit(values, buildValue, tb => tb.Append(delimiter));

    public TextBuilder EnumerateAndDelimit<T>(
        IEnumerable<T>? values,
        Action<TextBuilder, T> buildValue,
        char delimiter)
        => EnumerateAndDelimit(values, buildValue, tb => tb.Append(delimiter));

#endregion

#region text delimiter

    public TextBuilder EnumerateAndDelimit<T>(
        scoped ReadOnlySpan<T> values,
        Action<TextBuilder, T> buildValue,
        scoped text delimiter)
    {
        int len = values.Length;
        if (len == 0)
            return this;
        buildValue(this, values[0]);
        for (int i = 1; i < len; i++)
        {
            Append(delimiter);
            buildValue(this, values[i]);
        }

        return this;
    }

    public TextBuilder EnumerateAndDelimit<T>(
        scoped Span<T> values,
        Action<TextBuilder, T> buildValue,
        scoped text delimiter)
    {
        int len = values.Length;
        if (len == 0)
            return this;
        buildValue(this, values[0]);
        for (int i = 1; i < len; i++)
        {
            Append(delimiter);
            buildValue(this, values[i]);
        }

        return this;
    }


    public TextBuilder EnumerateAndDelimit<T>(
        T[]? values,
        Action<TextBuilder, T> buildValue,
        scoped text delimiter)
    {
        if (values is not null)
        {
            int len = values.Length;
            if (len == 0)
                return this;
            buildValue(this, values[0]);
            for (int i = 1; i < len; i++)
            {
                Append(delimiter);
                buildValue(this, values[i]);
            }
        }

        return this;
    }

    public TextBuilder EnumerateAndDelimit<T>(
        IList<T>? values,
        Action<TextBuilder, T> buildValue,
        scoped text delimiter)
    {
        if (values is not null)
        {
            int len = values.Count;
            if (len == 0)
                return this;
            buildValue(this, values[0]);
            for (int i = 1; i < len; i++)
            {
                Append(delimiter);
                buildValue(this, values[i]);
            }
        }

        return this;
    }

    public TextBuilder EnumerateAndDelimit<T>(
        IEnumerable<T>? values,
        Action<TextBuilder, T> buildValue,
        scoped text delimiter)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;
            buildValue(this, e.Current);
            while (e.MoveNext())
            {
                Append(delimiter);
                buildValue(this, e.Current);
            }
        }

        return this;
    }

#endregion

#region string delimiter

    public TextBuilder EnumerateAndDelimit<T>(
        scoped ReadOnlySpan<T> values,
        Action<TextBuilder, T> buildValue,
        string? delimiter)
        => EnumerateAndDelimit<T>(values, buildValue, tb => tb.Append(delimiter));

    public TextBuilder EnumerateAndDelimit<T>(
        scoped Span<T> values,
        Action<TextBuilder, T> buildValue,
        string? delimiter)
        => EnumerateAndDelimit<T>(values, buildValue, tb => tb.Append(delimiter));

    public TextBuilder EnumerateAndDelimit<T>(
        T[]? values,
        Action<TextBuilder, T> buildValue,
        string? delimiter)
        => EnumerateAndDelimit<T>(values, buildValue, tb => tb.Append(delimiter));

    public TextBuilder EnumerateAndDelimit<T>(
        IList<T>? values,
        Action<TextBuilder, T> buildValue,
        string? delimiter)
        => EnumerateAndDelimit<T>(values, buildValue, tb => tb.Append(delimiter));

    public TextBuilder EnumerateAndDelimit<T>(
        IEnumerable<T>? values,
        Action<TextBuilder, T> buildValue,
        string? delimiter)
        => EnumerateAndDelimit<T>(values, buildValue, tb => tb.Append(delimiter));

#endregion

#region newline delimiter

    public TextBuilder EnumerateAndDelimitLines<T>(
        scoped ReadOnlySpan<T> values,
        Action<TextBuilder, T> buildValue)
        => EnumerateAndDelimit(values, buildValue, static tb => tb.NewLine());

    public TextBuilder EnumerateAndDelimitLines<T>(
        scoped Span<T> values,
        Action<TextBuilder, T> buildValue)
        => EnumerateAndDelimit(values, buildValue, static tb => tb.NewLine());

    public TextBuilder EnumerateAndDelimitLines<T>(T[]? values,
        Action<TextBuilder, T> buildValue)
        => EnumerateAndDelimit(values, buildValue, static tb => tb.NewLine());

    public TextBuilder EnumerateAndDelimitLines<T>(IList<T>? values,
        Action<TextBuilder, T> buildValue)
        => EnumerateAndDelimit(values, buildValue, static tb => tb.NewLine());

    public TextBuilder EnumerateAndDelimitLines<T>(IEnumerable<T>? values,
        Action<TextBuilder, T> buildValue)
        => EnumerateAndDelimit(values, buildValue, static tb => tb.NewLine());

#endregion

#endregion

#region EnumerateFormatAndDelimit

#region char delimiter

    public TextBuilder EnumerateFormatAndDelimit<T>(
        scoped ReadOnlySpan<T> values,
        char delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        scoped Span<T> values,
        char delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        T[]? values,
        char delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        IList<T>? values,
        char delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        IEnumerable<T>? values,
        char delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

#endregion

#region text delimiter

    public TextBuilder EnumerateFormatAndDelimit<T>(
        scoped ReadOnlySpan<T> values,
        scoped text delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        scoped Span<T> values,
        scoped text delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        T[]? values,
        scoped text delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        IList<T>? values,
        scoped text delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        IEnumerable<T>? values,
        scoped text delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

#endregion

#region string delimiter

    public TextBuilder EnumerateFormatAndDelimit<T>(
        scoped ReadOnlySpan<T> values,
        string? delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        scoped Span<T> values,
        string? delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        T[]? values,
        string? delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        IList<T>? values,
        string? delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

    public TextBuilder EnumerateFormatAndDelimit<T>(
        IEnumerable<T>? values,
        string? delimiter)
        => EnumerateAndDelimit<T>(values, static (tb, ch) => tb.Format<T>(ch), delimiter);

#endregion

#endregion


#region Enumerate, Format, and Line Delimit

    public TextBuilder EnumerateFormatAndDelimitLines<T>(
        params ReadOnlySpan<T> values)
        => EnumerateAndDelimit(values,
            static (tb, value) => tb.Format<T>(value),
            static tb => tb.NewLine());

    public TextBuilder EnumerateFormatAndDelimitLines<T>(
        scoped Span<T> values)
        => EnumerateAndDelimit(values,
            static (tb, value) => tb.Format<T>(value),
            static tb => tb.NewLine());

    public TextBuilder EnumerateFormatAndDelimitLines<T>(
        T[]? values)
        => EnumerateAndDelimit(values,
            static (tb, value) => tb.Format<T>(value),
            static tb => tb.NewLine());


    public TextBuilder EnumerateFormatAndDelimitLines<T>(
        IList<T>? values)
        => EnumerateAndDelimit(values,
            static (tb, value) => tb.Format<T>(value),
            static tb => tb.NewLine());


    public TextBuilder EnumerateFormatAndDelimitLines<T>(
        IEnumerable<T>? values)
        => EnumerateAndDelimit(values,
            static (tb, value) => tb.Format<T>(value),
            static tb => tb.NewLine());

#endregion

#region Iterate

    public TextBuilder Iterate<T>(
        scoped ReadOnlySpan<T> values,
        Action<TextBuilder, T, int> buildTextWithValueIndex)
    {
        for (int i = 0; i < values.Length; i++)
        {
            buildTextWithValueIndex(this, values[i], i);
        }

        return this;
    }

    public TextBuilder Iterate<T>(
        scoped Span<T> values,
        Action<TextBuilder, T, int> buildTextWithValueIndex)
    {
        for (int i = 0; i < values.Length; i++)
        {
            buildTextWithValueIndex(this, values[i], i);
        }

        return this;
    }

    public TextBuilder Iterate<T>(
        T[]? values,
        Action<TextBuilder, T, int> buildTextWithValueIndex)
    {
        if (values is not null)
        {
            for (int i = 0; i < values.Length; i++)
            {
                buildTextWithValueIndex(this, values[i], i);
            }
        }

        return this;
    }

    public TextBuilder Iterate<T>(
        IList<T>? values,
        Action<TextBuilder, T, int> buildTextWithValueIndex)
    {
        if (values is not null)
        {
            for (int i = 0; i < values.Count; i++)
            {
                buildTextWithValueIndex(this, values[i], i);
            }
        }

        return this;
    }

    public TextBuilder Iterate<T>(
        IEnumerable<T>? values,
        Action<TextBuilder, T, int> buildTextWithValueIndex)
    {
        if (values is not null)
        {
            int index = 0;
            foreach (var value in values)
            {
                buildTextWithValueIndex(this, value, index);
                index++;
            }
        }

        return this;
    }

#endregion

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


#region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (var i = 0; i < _position; i++)
        {
            yield return _chars[i];
        }
    }

    IEnumerator<char> IEnumerable<char>.GetEnumerator()
    {
        for (var i = 0; i < _position; i++)
        {
            yield return _chars[i];
        }
    }

    public Span<char>.Enumerator GetEnumerator() => Written.GetEnumerator();

#endregion

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