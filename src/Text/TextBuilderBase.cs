using ScrubJay.Fluent;
using ScrubJay.Maths;
#pragma warning disable S3247, S4136, RCS1220
#pragma warning disable IDE1006, IDE0060
#pragma warning disable CA1033, CA1045, CA1710, CA1715

// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
namespace ScrubJay.Text;

/// <summary>
/// A FluentTextBuilder uses fluent operations to build up complex <see cref="string">strings</see>
/// </summary>
/// <typeparam name="B">
/// The <see cref="Type"/> of <see cref="TextBuilderBase{B}"/> that will be returned from all fluent operations
/// </typeparam>
[PublicAPI]
[MustDisposeResource]
public abstract class TextBuilderBase<B> : FluentBuilder<B>,
    IList<char>,
    IReadOnlyList<char>,
    ICollection<char>,
    IReadOnlyCollection<char>,
    IEnumerable<char>,
    IDisposable
    where B : TextBuilderBase<B>
{
    // This manages all the actual writing
    protected readonly PooledList<char> _text;

    int ICollection<char>.Count => Length;
    bool ICollection<char>.IsReadOnly => false;
    int IReadOnlyCollection<char>.Count => Length;

    char IReadOnlyList<char>.this[int index] => this[index];

    char IList<char>.this[int index]
    {
        get => this[index];
        set => this[index] = value;
    }

    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _text[index];
    }

    public ref char this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _text[index];
    }

    public Span<char> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _text[range];
    }

    /// <summary>
    /// Gets the total length of text written to this Builder
    /// </summary>
    public int Length => _text.Count;

    protected TextBuilderBase() : base()
    {
        _text = new();
    }

    protected TextBuilderBase(int minCapacity) : base()
    {
        _text = new(minCapacity);
    }

    void ICollection<char>.Add(char item) => Append(item);

    protected internal virtual void InterpolatedExecute(Action<B> build) => build(_builder);


    #region Append
    /// <summary>
    /// Append a <see cref="char">character</see>
    /// </summary>
    /// <param name="ch">The <see cref="char"/> to append</param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Append(char ch)
    {
        _text.Add(ch);
        return _builder;
    }

    /// <summary>
    /// Append a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>
    /// </summary>
    /// <param name="text">The <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> to append</param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Append(scoped text text)
    {
        _text.AddMany(text);
        return _builder;
    }

    public virtual B Append(char[]? chars)
    {
        _text.AddMany(new text(chars));
        return _builder;
    }

    /// <summary>
    /// Append a <see cref="string"/>
    /// </summary>
    /// <param name="str">The <see cref="string"/> to append</param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Append(string? str)
    {
        _text.AddMany(str.AsSpan());
        return _builder;
    }

    /// <summary>
    /// Appends text using an <see cref="InterpolatedTextBuilder{B}"/>
    /// </summary>
    /// <param name="interpolatedTextBuilder"></param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Append(
        [InterpolatedStringHandlerArgument("")]
        ref InterpolatedTextBuilder<B> interpolatedTextBuilder)
    {
        // As soon as we've gotten here, the interpolation has occurred
        return _builder;
    }


    /// <summary>
    /// Append a <typeparamref name="T"/> <paramref name="value"/> with no formatting
    /// </summary>
    /// <param name="value">The value to append the textual representation of</param>
    /// <typeparam name="T">The <see cref="Type"/> of value to append</typeparam>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Append<T>(T? value)
    {
        if (value is null)
        {
            return _builder;
        }

        if (value is IWriteable)
        {
            ((IWriteable)value).WriteTo(_builder);
        }
        else if (value is IFormattable)
        {
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(_text.Available, out charsWritten, default, default))
                {
                    _text.Grow();
                }
                _text.Count += charsWritten;
            }
            else
            {
                _text.AddMany(((IFormattable)value).ToString(default, default).AsSpan());
            }
        }
        else
        {
            _text.AddMany(value.ToString().AsSpan());
        }

        return _builder;
    }

    /// <summary>
    /// Append a <typeparamref name="T"/> <paramref name="value"/> with a format
    /// </summary>
    /// <param name="value">The value to append the textual representation of</param>
    /// <param name="format">The <see cref="string"/> format</param>
    /// <param name="provider">The optional <see cref="IFormatProvider"/></param>
    /// <typeparam name="T">The <see cref="Type"/> of value to format and append</typeparam>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Append<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        if (value is null)
        {
            return _builder;
        }

        if (value is IWriteable)
        {
            ((IWriteable)value).WriteTo(_builder);
        }
        else if (value is IFormattable)
        {
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(_text.Available, out charsWritten, format.AsSpan(), provider))
                {
                    _text.Grow();
                }
                _text.Count += charsWritten;
            }
            else
            {
                _text.AddMany(((IFormattable)value).ToString(format, provider).AsSpan());
            }
        }
        else
        {
            _text.AddMany(value.ToString().AsSpan());
        }

        return _builder;
    }

    /// <summary>
    /// Append a <typeparamref name="T"/> <paramref name="value"/> with a format
    /// </summary>
    /// <param name="value">The value to append the textual representation of</param>
    /// <param name="format">The <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> format</param>
    /// <param name="provider">The optional <see cref="IFormatProvider"/></param>
    /// <typeparam name="T">The <see cref="Type"/> of value to format and append</typeparam>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Append<T>(T? value, scoped text format, IFormatProvider? provider = null)
    {
        if (value is null)
        {
            return _builder;
        }

        if (value is IWriteable)
        {
            ((IWriteable)value).WriteTo(_builder);
        }
        else if (value is IFormattable)
        {
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(_text.Available, out charsWritten, format, provider))
                {
                    _text.Grow();
                }
                _text.Count += charsWritten;
            }
            else
            {
                _text.AddMany(((IFormattable)value).ToString(format.AsString(), provider).AsSpan());
            }
        }
        else
        {
            _text.AddMany(value.ToString().AsSpan());
        }

        return _builder;
    }

    /// <summary>
    /// Appends a <see cref="Environment.NewLine"/>
    /// </summary>
    /// <returns></returns>
    public virtual B NewLine() => Append(Environment.NewLine);

    #region AppendLine
    public B AppendLine(char ch) => Append(ch).NewLine();
    public B AppendLine(scoped text text) => Append(text).NewLine();
    public B AppendLine(char[]? chars) => Append(chars).NewLine();
    public B AppendLine(string? str) => Append(str).NewLine();
    public B AppendLine<T>(T? value) => Append<T>(value).NewLine();
    public B AppendLine<T>(T? value, scoped text format, IFormatProvider? provider) => Append<T>(value, format, provider).NewLine();
    public B AppendLine<T>(T? value, string? format, IFormatProvider? provider) => Append<T>(value, format, provider).NewLine();
    public B AppendLine(
        [InterpolatedStringHandlerArgument("")]
        ref InterpolatedTextBuilder<B> interpolatedTextBuilder) => NewLine();
#endregion
#endregion

    #region Format
    // https://doc.rust-lang.org/std/fmt/


    #endregion

#region Align
    public B Align(
        char ch,
        int width,
        char paddingChar = ' ',
        Alignment alignment = Alignment.None)
        => Align(ch.AsSpan(), width, paddingChar, alignment);

    public B Align(
        scoped text text,
        int width,
        char paddingChar = ' ',
        Alignment alignment = Alignment.None)
    {
        // if no alignment is specified, we use -width as Left, +width as Right (same as string.Format)
        if (alignment == Alignment.None)
        {
            if (width < 0)
            {
                alignment = Alignment.Left;
                width = -width;
            }
            else
            {
                alignment = Alignment.Right;
            }
        }

        // calculate the amount of padding we have to add
        int padding = width - text.Length;

        // as per string.Format, if width < text, we just write value
        if (padding <= 0)
        {
            return Append(text);
        }

        // faster path when padding == 1
        if (padding == 1)
        {
            if (alignment.HasFlags(Alignment.Left))
            {
                return Append(text).Append(paddingChar);
            }
            else
            {
                return Append(paddingChar).Append(paddingChar);
            }
        }

        // Use alignment
        if (alignment == Alignment.Left)
        {
            return Append(text).Repeat(padding, paddingChar);
        }

        if (alignment == Alignment.Right)
        {
            return Repeat(padding, paddingChar).Append(text);
        }

        // if (alignment.HasFlag(Alignment.Center))
        Debug.Assert(alignment.HasFlags(Alignment.Center));

        // if padding is even, pre + post are the same
        if (padding.IsEven())
        {
            int pad = MathHelper.HalfRoundDown(padding);
            return Repeat(pad, paddingChar)
                .Append(text)
                .Repeat(pad, paddingChar);
        }

        // padding is odd, we need to use bias
        double half = padding / 2.0d;

        int pre;
        int post;

        // Center w/Left Bias?
        if (alignment.HasFlag(Alignment.Left))
        {
            pre = (int)Math.Floor(half);
            post = (int)Math.Ceiling(half);
        }
        else
        {
            // Defaults to Center w/Right Bias
            pre = (int)Math.Ceiling(half);
            post = (int)Math.Floor(half);
        }

        return Repeat(pre, paddingChar)
            .Append(text)
            .Repeat(post, paddingChar);
    }

    public B AlignFormat<T>(T? value,
        int width,
        string? format = null,
        char paddingChar = ' ',
        Alignment alignment = Alignment.None)
    {
        if (width == 0)
            return Append(value, format);

        // if no alignment is specified, we use -width as Left, +width as Right (same as string.Format)
        if (alignment == Alignment.None)
        {
            if (width < 0)
            {
                alignment = Alignment.Left;
                width = -width;
            }
            else
            {
                alignment = Alignment.Right;
            }
        }

        // Grab our position before formatting the value
        int start = _text.Count;

        // Format the value onto us
        Append<T>(value, format);

        // get end and length
        int end = _text.Count;
        int length = end - start;

        // calculate the amount of padding we have to add
        int padding = width - length;

        // as per string.Format, if width < text, we just write the value, which we've done
        if (padding <= 0)
            return _builder;

        // Fast path for padding == 1
        if (padding == 1)
        {
            // we start off left-aligned
            if (alignment.HasFlags(Alignment.Left))
                return Append(paddingChar);
            // shift one right
            return Insert(start, paddingChar);
        }

        // We're already left-aligned
        if (alignment == Alignment.Left)
        {
            return Repeat(padding, paddingChar);
        }

        // Everything after this point will involve some amount of insertion

        // Easy shift to right-align
        if (alignment == Alignment.Right)
        {
            return Insert(start, TextHelper.Repeat(padding, paddingChar));
        }

        // Assume centered at this point
        Debug.Assert(alignment.HasFlags(Alignment.Center));

        // if padding is even, pre + post are the same
        if (padding.IsEven())
        {
            string pad = TextHelper.Repeat(MathHelper.HalfRoundDown(padding), paddingChar);
            // Insert before, then just append after
            return Insert(start, pad).Append(pad);
        }

        // padding is odd, we need to use bias
        double half = padding / 2.0d;

        int pre;
        int post;

        // Center w/Left Bias?
        if (alignment.HasFlag(Alignment.Left))
        {
            pre = (int)Math.Floor(half);
            post = (int)Math.Ceiling(half);
        }
        else
        {
            // Defaults to Center w/Right Bias
            pre = (int)Math.Ceiling(half);
            post = (int)Math.Floor(half);
        }

        // Insert pre before, append post behind
        return Insert(start, TextHelper.Repeat(pre, paddingChar))
            .Repeat(post, paddingChar);
    }


#endregion


#region Enumeration
    public B Enumerate<T>(ReadOnlySpan<T> values, Action<B, T> onBuilderValue)
    {
        foreach (var t in values)
        {
            onBuilderValue(_builder, t);
        }

        return _builder;
    }

    public B Enumerate<T>(IEnumerable<T> values, Action<B, T> onBuilderValue)
    {
        foreach (var value in values)
        {
            onBuilderValue(_builder, value);
        }

        return _builder;
    }

    public B EnumerateAppend<T>(ReadOnlySpan<T> values)
        => Enumerate(values, static (tb, value) => tb.Append(value));

    public B EnumerateAppend<T>(IEnumerable<T> values)
        => Enumerate(values, static (tb, value) => tb.Append(value));


    public B Iterate<T>(ReadOnlySpan<T> values, Action<B, T, int> onBuilderValueIndex)
    {
        for (int i = 0; i < values.Length; i++)
        {
            onBuilderValueIndex(_builder, values[i], i);
        }

        return _builder;
    }

    public B Iterate<T>(IEnumerable<T> values, Action<B, T, int> onBuilderValueIndex)
    {
        int index = 0;
        foreach (var value in values)
        {
            onBuilderValueIndex(_builder, value, index);
            index++;
        }

        return _builder;
    }

#region Delimit
#region ReadOnlySpan
    public B Delimit<T>(char delimiter, ReadOnlySpan<T> values, Action<B, T> onBuilderValue)
    {
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (int i = 1; i < len; i++)
        {
            Append(delimiter);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }


    public B Delimit<T>(string delimiter, ReadOnlySpan<T> values, Action<B, T> onBuilderValue) => Delimit(delimiter.AsSpan(), values, onBuilderValue);

    public B Delimit<T>(scoped text delimiter, ReadOnlySpan<T> values, Action<B, T> onBuilderValue)
    {
        if (delimiter.Length == 0)
            return Enumerate(values, onBuilderValue);
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (int i = 1; i < len; i++)
        {
            Append(delimiter);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }

    public B Delimit<T>(Action<B> onDelimit, ReadOnlySpan<T> values, Action<B, T> onBuilderValue)
    {
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (int i = 1; i < len; i++)
        {
            onDelimit(_builder);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }

    public B DelimitAppend<T>(char delimiter, ReadOnlySpan<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(string delimiter, ReadOnlySpan<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(scoped text delimiter, ReadOnlySpan<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(Action<B> onDelimit, ReadOnlySpan<T> values) => Delimit(onDelimit, values, static (tb, value) => tb.Append(value));
#endregion

#region IEnumerable
    public B Delimit<T>(char delimiter, IEnumerable<T> values, Action<B, T> onBuilderValue)
    {
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            _text.Add(delimiter);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }


    public B Delimit<T>(string delimiter, IEnumerable<T> values, Action<B, T> onBuilderValue)
        => Delimit(delimiter.AsSpan(), values, onBuilderValue);

    public B Delimit<T>(scoped text delimiter, IEnumerable<T> values, Action<B, T> onBuilderValue)
    {
        if (delimiter.Length == 0)
            return Enumerate(values, onBuilderValue);
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            _text.AddMany(delimiter);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }

    public B Delimit<T>(Action<B> onDelimit, IEnumerable<T> values, Action<B, T> onBuilderValue)
    {
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            onDelimit(_builder);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }

    public B DelimitAppend<T>(char delimiter, IEnumerable<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(string delimiter, IEnumerable<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(scoped text delimiter, IEnumerable<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(Action<B> onDelimit, IEnumerable<T> values) => Delimit(onDelimit, values, static (tb, value) => tb.Append(value));
#endregion
#endregion
#endregion

#region Insertion
    void IList<char>.Insert(int index, char item) => _text.TryInsert(index, item).ThrowIfError();

    public B Insert(Index index, char ch)
    {
        _text.TryInsert(index, ch).ThrowIfError();
        return _builder;
    }

    public B Insert(Index index, scoped text text)
    {
        _text.TryInsertMany(index, text).ThrowIfError();
        return _builder;
    }

    public B Insert(Index index, string? str) => Insert(index, str.AsSpan());
#endregion

    public B Allocate(int count, WSAct<char> write)
    {
        if (count <= 0)
            return _builder;
        var span = _text.Allocate(count);
        write(span);
        return _builder;
    }

    public B Repeat(int count, char ch)
    {
        if (count <= 0)
            return _builder;
        var span = _text.Allocate(count);
        span.Fill(ch);
        return _builder;
    }

    public B Repeat(int count, scoped text text)
    {
        int textLength = text.Length;
        int totalLength = count * textLength;
        if (totalLength <= 0)
            return _builder;
        Span<char> buffer = _text.Allocate(totalLength);
        int i = 0;
        do
        {
            TextHelper.Notsafe.CopyBlock(text, buffer[i..], textLength);
            i += textLength;
        } while (i < totalLength);
        return _builder;
    }

    public B RepeatFormat<T>(int count, T? value, string? format = null)
    {
        if (count <= 0)
            return _builder;
        int start = _text.Count;
        Append<T>(value, format);
        int length = _text.Count - start;
        var formattedSpan = _text.Written[^length..];
        Debugger.Break();
        return Repeat(count - 1, formattedSpan);
    }


    public Option<int> TryFindIndex(char ch, bool firstToLast = true, Index? offset = default, IEqualityComparer<char>? charComparer = null)
        => _text.TryFindIndex(ch, firstToLast, offset, charComparer);

    public Option<int> TryFindIndex(
        scoped text text,
        bool firstToLast = true,
        Index? offset = default,
        IEqualityComparer<char>? charComparer = null)
        => _text.TryFindIndex(text, firstToLast, offset, charComparer);

    bool ICollection<char>.Contains(char item) => _text.Contains(item);

    int IList<char>.IndexOf(char item) => _text.TryFindIndex(item).SomeOr(-1);


    public bool TryRemoveAt(Index index)
        => _text.TryRemoveAt(index);

    public bool TryRemoveMany(Range range)
        => _text.TryRemoveMany(range);

    bool ICollection<char>.Remove(char item)
    {
        if (TryFindIndex(item).HasSome(out int index))
        {
            return _text.TryRemoveAt(index);
        }
        return false;
    }

    void IList<char>.RemoveAt(int index)
    {
        if (!_text.TryRemoveAt(index))
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public B RemoveLast(int count)
    {
        _text.Count -= count;
        return _builder;
    }


    public B Clear()
    {
        _text.Clear();
        return _builder;
    }

    void ICollection<char>.Clear() => Clear();

    public bool TryCopyTo(Span<char> span) => _text.TryCopyTo(span);

    void ICollection<char>.CopyTo(char[] array, int arrayIndex)
    {
        Validate.CanCopyTo(array, arrayIndex, _text.Count).ThrowIfError();
        _ = _text.TryCopyTo(array.AsSpan(arrayIndex));
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public text AsText() => _text.Written;

    IEnumerator IEnumerable.GetEnumerator() => _text.GetEnumerator();
    IEnumerator<char> IEnumerable<char>.GetEnumerator() => _text.GetEnumerator();

    [HandlesResourceDisposal]
    public virtual void Dispose() => _text.Dispose();

    [HandlesResourceDisposal]
    public string ToStringAndDispose()
    {
        string str = ToString();
        Dispose();
        return str;
    }

    public override string ToString() => _text.ToString();
}
