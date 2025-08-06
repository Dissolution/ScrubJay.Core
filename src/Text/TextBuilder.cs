#pragma warning disable CA1710

using System.Buffers;
using ScrubJay.Maths;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

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
    public delegate void BuildSegment<T>(TextBuilder builder, scoped ReadOnlySpan<T> segment)
        where T : IEquatable<T>;

    private static readonly string _newline = Environment.NewLine;

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

    public static string Build(ref InterpolatedTextBuilder interpolatedText)
    {
        return interpolatedText.ToStringAndDispose();
    }


    // Character array rented from array pool
    private char[] _chars;

    // Position in _chars that is next to be written to
    private int _position;

    // The current depth of indents
    private int _indents;

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


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBy(int adding)
    {
        Debug.Assert(adding > 0);
        GrowTo(Capacity + (adding * 16));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowTo(int minCapacity)
    {
        Debug.Assert(minCapacity > Capacity);
        char[] array = ArrayPool<char>.Shared.Rent(Math.Max(minCapacity * 2, 1024));
        if (_chars.Length > 0)
        {
            Debug.Assert(_chars is not null);
            Written.CopyTo(array);
            ArrayPool<char>.Shared.Return(_chars, true);
        }

        _chars = array;
    }

#region NewLine

    public TextBuilder NewLine()
    {
        return Append(_newline)
            .If(_indents, static id => id > 0, static (tb, id) => tb.RepeatAppend(id * 4, ' '));
    }

    public TextBuilder NewLine(Action<TextBuilder>? buildText)
        => Invoke(buildText).NewLine();

#endregion

#region Append

    /// <summary>
    /// Append a <see cref="char"/> to the end of this <see cref="TextBuilder"/>
    /// </summary>
    public TextBuilder Append(char ch)
    {
        int pos = _position;
        int newPos = pos + 1;
        if (newPos > Capacity)
        {
            GrowBy(1);
        }

        _chars[pos] = ch;
        _position = pos + 1;
        return this;
    }

    public TextBuilder Append(params text text)
    {
        int pos = _position;
        int len = text.Length;
        int newPos = pos + len;
        if (newPos > Capacity)
        {
            GrowBy(len);
        }

        Notsafe.Text.CopyBlock(text, _chars.AsSpan(pos), len);
        _position = newPos;
        return this;
    }

    public TextBuilder Append(char[]? chars)
    {
        if (chars is not null)
        {
            int pos = _position;
            int len = chars.Length;
            int newPos = pos + len;
            if (newPos > Capacity)
            {
                GrowBy(len);
            }

            Notsafe.Text.CopyBlock(chars, _chars.AsSpan(pos), len);
            _position = newPos;
        }

        return this;
    }

    public TextBuilder Append(string? str)
    {
        if (str is not null)
        {
            int pos = _position;
            int len = str.Length;
            int newPos = pos + len;
            if (newPos > Capacity)
            {
                GrowBy(len);
            }

            Notsafe.Text.CopyBlock(str, _chars.AsSpan(pos), len);
            _position = newPos;
        }

        return this;
    }

#pragma warning disable IDE0060, CA2000
    public TextBuilder Append(
        [HandlesResourceDisposal]
        [InterpolatedStringHandlerArgument("")] // pass this TextBuilder instance in as an argument
        InterpolatedTextBuilder interpolatedTextBuilder)
    {
        // as this TextBuilder instance was passed into the InterpolatedTextBuilder's constructor,
        // all the writing has already occurred
        interpolatedTextBuilder.Dispose();
        return this;
    }
#pragma warning restore IDE0060, CA2000

#endregion

#region AppendLine

    public TextBuilder AppendLine(char ch) => Append(ch).NewLine();

    public TextBuilder AppendLine(params text text) => Append(text).NewLine();

    public TextBuilder AppendLine(char[]? chars) => Append(chars).NewLine();

    public TextBuilder AppendLine(string? str) => Append(str).NewLine();

#pragma warning disable IDE0060
    public TextBuilder AppendLine(
        [InterpolatedStringHandlerArgument("")]
        [HandlesResourceDisposal]
        InterpolatedTextBuilder interpolatedText) => Append(interpolatedText).NewLine();
#pragma warning restore IDE0060

#endregion

#region Format

    // ReSharper disable MergeCastWithTypeCheck

    public TextBuilder Format<T>(T? value)
    {
        if (value is null)
        {
            return this;
        }

        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return this;
            }
#endif

            return Append(((IFormattable)value).ToString(default, default));
        }

        return Append(value.ToString());
    }

    public TextBuilder Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        if (value is null) return this;

        // special format codes for rendering
        if (format.Equate("@"))
            return Render<T>(value);
        if (format.Equate("@T", StringComparison.OrdinalIgnoreCase))
            return RenderType<T>(value);

        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return this;
            }
#endif

            return Append(((IFormattable)value).ToString(format, provider));
        }

        return Append(value.ToString());
    }

    public TextBuilder Format<T>(T? value, scoped text format, IFormatProvider? provider = null)
    {
        if (value is null) return this;

        // special format codes for rendering
        if (format.Equate("@"))
            return Render<T>(value);
        if (format.Equate("@T", StringComparison.OrdinalIgnoreCase))
            return RenderType<T>(value);

        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return this;
            }
#endif

            return Append(((IFormattable)value).ToString(format.AsString(), provider));
        }

        return Append(value.ToString());
    }


    // ReSharper restore MergeCastWithTypeCheck

#endregion

#region FormatLine

    public TextBuilder FormatLine<T>(T? value) => Format<T>(value).NewLine();

    public TextBuilder FormatLine<T>(T? value, string? format, IFormatProvider? provider = null)
        => Format<T>(value, format, provider).NewLine();

    public TextBuilder FormatLine<T>(T? value, scoped text format, IFormatProvider? provider = null)
        => Format<T>(value, format, provider).NewLine();

#endregion

#region Render

    public TextBuilder Render<T>(T? value)
    {
        RendererCache.RenderTo<T>(value, this);
        return this;
    }

    public TextBuilder Render<T>(T[]? array)
    {
        RendererCache.RenderTo<T>(array, this);
        return this;
    }

    public TextBuilder Render<T>(scoped ReadOnlySpan<T> span)
    {
        RendererCache.RenderTo<T>(span, this);
        return this;
    }

    public TextBuilder Render<T>(scoped Span<T> span)
    {
        RendererCache.RenderTo<T>(span, this);
        return this;
    }

    public TextBuilder Render(scoped text text)
    {
        RendererCache.RenderTo(text, this);
        return this;
    }

    public TextBuilder RenderType<T>(T? value)
    {
        Type valueType = value?.GetType() ?? typeof(T);
        RendererCache.RenderTo<Type>(valueType, this);
        return this;
    }

#endregion

#region RenderLine

    public TextBuilder RenderLine<T>(T? value) => Render<T>(value).NewLine();

#endregion

#region Indents

    public TextBuilder Indent()
    {
        _indents++;
        return this;
    }

    public TextBuilder Dedent()
    {
        if (_indents <= 0)
            throw new InvalidOperationException("There are no indents to remove");
        _indents--;
        return this;
    }

    public TextBuilder Indented(Action<TextBuilder>? buildText)
    {
        if (buildText is not null)
        {
            _indents++;
            buildText(this);
            _indents--;
        }

        return this;
    }

#endregion

#region Repeat

    public TextBuilder Repeat(int count, Action<TextBuilder>? buildText)
    {
        if (buildText is not null)
        {
            for (int i = 0; i < count; i++)
            {
                buildText(this);
            }
        }

        return this;
    }

    public TextBuilder Repeat<S>(int count, S state, Action<TextBuilder, S>? buildStateText)
    {
        if (buildStateText is not null)
        {
            for (int i = 0; i < count; i++)
            {
                buildStateText(this, state);
            }
        }

        return this;
    }

#endregion

#region RepeatAppend

    public TextBuilder RepeatAppend(int count, char ch)
    {
        if (count > 0)
        {
            int pos = _position;
            int newPos = pos + count;
            if (newPos > Capacity)
            {
                GrowBy(count);
            }

            _chars.AsSpan(pos, count).Fill(ch);
            _position = newPos;
        }

        return this;
    }

    public TextBuilder RepeatAppend(int count, scoped text text)
    {
        int len = text.Length;
        if (count > 0 && len > 0)
        {
            int pos = _position;
            int newPos = pos + (len * count);
            if (newPos > Capacity)
                GrowTo(newPos);

            var span = _chars.AsSpan();
            for (int i = 0; i < count; i++, pos += len)
            {
                Notsafe.Text.CopyBlock(text, span[pos..], len);
            }

            Debug.Assert(pos == newPos);
            _position = newPos;
        }

        return this;
    }

    public TextBuilder RepeatAppend(int count, char[]? chars)
    {
        if (chars is not null)
        {
            int len = chars.Length;
            if (count > 0 && len > 0)
            {
                int pos = _position;
                int newPos = pos + (len * count);
                if (newPos > Capacity)
                    GrowTo(newPos);

                var span = _chars.AsSpan();
                for (int i = 0; i < count; i++, pos += len)
                {
                    Notsafe.Text.CopyBlock(chars, span[pos..], len);
                }

                Debug.Assert(pos == newPos);
                _position = newPos;
            }
        }

        return this;
    }

    public TextBuilder RepeatAppend(int count, string? str)
    {
        if (str is not null)
        {
            int len = str.Length;
            if (count > 0 && len > 0)
            {
                int pos = _position;
                int newPos = pos + (len * count);
                if (newPos > Capacity)
                    GrowTo(newPos);

                var span = _chars.AsSpan();
                for (int i = 0; i < count; i++, pos += len)
                {
                    Notsafe.Text.CopyBlock(str, ref span[pos], len);
                }

                Debug.Assert(pos == newPos);
                _position = newPos;
            }
        }

        return this;
    }

#endregion

#region RepeatFormat

    public TextBuilder RepeatFormat<T>(int count, T? value)
    {
        if (count > 0)
        {
            int start = _position;
            Format<T>(value);
            int pos = _position;
            Span<char> written = _chars.AsSpan(start, pos - start);
            return RepeatAppend(count - 1, written);
        }

        return this;
    }

    public TextBuilder RepeatFormat<T>(int count, T? value, string? format, IFormatProvider? provider = null)
    {
        if (count > 0)
        {
            int start = _position;
            Format<T>(value, format, provider);
            int pos = _position;
            Span<char> written = _chars.AsSpan(start, pos - start);
            return RepeatAppend(count - 1, written);
        }

        return this;
    }

    public TextBuilder RepeatFormat<T>(int count, T? value, scoped text format, IFormatProvider? provider = null)
    {
        if (count > 0)
        {
            int start = _position;
            Format<T>(value, format, provider);
            int pos = _position;
            Span<char> written = _chars.AsSpan(start, pos - start);
            return RepeatAppend(count - 1, written);
        }

        return this;
    }

#endregion


#region Align

    public TextBuilder Align(
        char ch,
        int width,
        char paddingChar = ' ',
        Alignment alignment = Alignment.Right)
        => Align(ch.AsSpan(), width, paddingChar, alignment);

    public TextBuilder Align(
        string? str,
        int width,
        char paddingChar = ' ',
        Alignment alignment = Alignment.Right)
        => Align(str.AsSpan(), width, paddingChar, alignment);

    public TextBuilder Align(
        scoped text text,
        int width,
        char paddingChar = ' ',
        Alignment alignment = Alignment.Right)
    {
        // a negative width indicates a left-bias
        if (width < 0)
        {
            alignment |= Alignment.Left;
            width = -width;
        }
        // no width means append nothing
        else if (width == 0)
        {
            return this;
        }

        // fit text into width
        int len = text.Length;

        // no text: fill with padding
        if (len == 0)
        {
            return RepeatAppend(width, paddingChar);
        }

        // if there is not enough space, we show a truncated version of the text
        if (width < len)
        {
            // if there is only one char available, we don't even show that, only an indicator
            if (width == 1)
                return Append('…');

            // right align, write the ellipsis and then the trailing text
            if (alignment == Alignment.Right)
            {
                return Append('…').Append(text[^(width - 1)..]);
            }

            // left align, write the starting text and then the ellipsis
            if (alignment == Alignment.Left)
            {
                return Append(text[..(width - 1)]).Append('…');
            }

            // center alignment requires two ellipsis
            Debug.Assert(alignment.HasFlags(Alignment.Center));
            if (width == 2)
                return Append("……");

            // get the center part of the text
            width -= 2;
            double mid = (len / 2d) - (width / 2d);
            int start;
            if (alignment.HasFlags(Alignment.Left))
            {
                start = (int)Math.Floor(mid);
            }
            else
            {
                start = (int)Math.Ceiling(mid);
            }

            var slice = text.Slice(start, width);
            return Append('…').Append(slice).Append('…');
        }

        // calculate the amount of padding we have to add
        int padding = width - text.Length;

        // Use alignment
        if (alignment == Alignment.Left)
        {
            return Append(text).RepeatAppend(padding, paddingChar);
        }

        if (alignment == Alignment.Right)
        {
            return RepeatAppend(padding, paddingChar).Append(text);
        }

        Debug.Assert(alignment.HasFlags(Alignment.Center));

        // if padding is even, pre + post are the same
        if (padding.IsEven())
        {
            int pad = padding / 2;
            return RepeatAppend(pad, paddingChar)
                .Append(text)
                .RepeatAppend(pad, paddingChar);
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

        return RepeatAppend(pre, paddingChar)
            .Append(text)
            .RepeatAppend(post, paddingChar);
    }

    public TextBuilder AlignFormat<T>(
        T? value,
        int width,
        string? format = null,
        IFormatProvider? provider = null,
        char paddingChar = ' ',
        Alignment alignment = Alignment.Right)
    {
        // a negative width indicates a left-bias
        if (width < 0)
        {
            alignment |= Alignment.Left;
            width = -width;
        }
        // no width means append nothing
        else if (width == 0)
        {
            return this;
        }

        // have to format the value (otherwise we have no idea what len is)
        int start = _position;
        Format<T>(value, format, provider);
        int end = _position;
        int len = end - start;

        // fit text into width

        // no text; fill with padding
        if (len == 0)
        {
            return RepeatAppend(width, paddingChar);
        }

        // not enough space?
        if (width < len)
        {
            if (width == 1)
            {
                // just an ellipsis
                _chars[start] = '…';
                _position = start + 1;
                return this;
            }
            else if (alignment == Alignment.Left)
            {
                // trim back before ellipsis
                end = (start + width) - 1;
                _chars[end] = '…';
                _position = start + width;
                return this;
            }
            else if (alignment == Alignment.Right)
            {
                // start with ellipsis
                _chars[start] = '…';
                // copy and trim
                var chunk = _chars.AsSpan(start, len)[^(width - 1)..];
                Debug.Assert(width - 1 == chunk.Length);
                Notsafe.Text.CopyBlock(chunk, _chars.AsSpan(start + 1), width - 1);

                _position = start + width;
                return this;
            }
            else
            {
                Debug.Assert(alignment.HasFlags(Alignment.Center));
                if (width == 2)
                    return Append("……");

                var wrote = _chars.AsSpan(start, len);

                // get the center width-2 characters
                width -= 2;
                double mid = (len / 2d) - (width / 2d);
                if (alignment.HasFlags(Alignment.Left))
                {
                    start = (int)Math.Floor(mid);
                }
                else
                {
                    start = (int)Math.Ceiling(mid);
                }

                end = width - start;
                Debugger.Break();
                var slice = wrote.Slice(start, width);
                Debug.Assert(slice.Length == end - start);
                //return Append('…').Append(slice).Append('…');
                throw new NotImplementedException();
            }
        }

        throw new NotImplementedException();
        /*

        // calculate the amount of padding we have to add
        int padding = width - len;

        // Use alignment
        if (alignment == Alignment.Left)
        {
            // value has been written, rest is padding
            return RepeatAppend(padding, paddingChar);
        }

        if (alignment == Alignment.Right)
        {
            // we have to insert the padding before what we wrote
            Span<char> pad = stackalloc char[padding];
            pad.Fill(paddingChar);
            return Insert(start, pad);
        }

        Debug.Assert(alignment.HasFlags(Alignment.Center));

        // if padding is even, pre + post are the same
        if (padding.IsEven())
        {
            int pad = MathHelper.HalfRoundDown(padding);
            return RepeatAppend(pad, paddingChar)
                .Append(text)
                .RepeatAppend(pad, paddingChar);
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

        return RepeatAppend(pre, paddingChar)
            .Append(text)
            .RepeatAppend(post, paddingChar);

        */
        /*
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
        */
    }

#endregion

#region Wrap

#region single

    public TextBuilder Wrap(char wrapChar, Action<TextBuilder>? buildText)
        => Append(wrapChar).Invoke(buildText).Append(wrapChar);

    public TextBuilder WrapAppend(char wrapChar, char ch)
        => Append(wrapChar).Append(ch).Append(wrapChar);

    public TextBuilder WrapAppend(char wrapChar, scoped text text)
        => Append(wrapChar).Append(text).Append(wrapChar);

    public TextBuilder WrapAppend(char wrapChar, string? str)
        => Append(wrapChar).Append(str).Append(wrapChar);

    public TextBuilder WrapFormat<T>(char wrapChar, T? value, string? format = null, IFormatProvider? provider = null)
        => Append(wrapChar).Format(value, format, provider).Append(wrapChar);


    public TextBuilder Wrap(scoped text wrapText, Action<TextBuilder>? buildText)
        => Append(wrapText).Invoke(buildText).Append(wrapText);

    public TextBuilder WrapAppend(scoped text wrapText, char ch)
        => Append(wrapText).Append(ch).Append(wrapText);

    public TextBuilder WrapAppend(scoped text wrapText, scoped text text)
        => Append(wrapText).Append(text).Append(wrapText);

    public TextBuilder WrapAppend(scoped text wrapText, string? str)
        => Append(wrapText).Append(str).Append(wrapText);

    public TextBuilder WrapFormat<T>(scoped text wrapText, T? value, string? format = null, IFormatProvider? provider = null)
        => Append(wrapText).Format(value, format, provider).Append(wrapText);


    public TextBuilder Wrap(string? wrapString, Action<TextBuilder>? buildText)
        => Append(wrapString).Invoke(buildText).Append(wrapString);

    public TextBuilder WrapAppend(string? wrapString, char ch)
        => Append(wrapString).Append(ch).Append(wrapString);

    public TextBuilder WrapAppend(string? wrapString, scoped text text)
        => Append(wrapString).Append(text).Append(wrapString);

    public TextBuilder WrapAppend(string? wrapString, string? str)
        => Append(wrapString).Append(str).Append(wrapString);

    public TextBuilder WrapFormat<T>(string? wrapString, T? value, string? format = null, IFormatProvider? provider = null)
        => Append(wrapString).Format(value, format, provider).Append(wrapString);

    public TextBuilder Wrap(Action<TextBuilder>? wrapBuild, Action<TextBuilder>? buildText)
        => Invoke(wrapBuild).Invoke(buildText).Invoke(wrapBuild);

    public TextBuilder WrapAppend(Action<TextBuilder>? wrapBuild, char ch)
        => Invoke(wrapBuild).Append(ch).Invoke(wrapBuild);

    public TextBuilder WrapAppend(Action<TextBuilder>? wrapBuild, scoped text text)
        => Invoke(wrapBuild).Append(text).Invoke(wrapBuild);

    public TextBuilder WrapAppend(Action<TextBuilder>? wrapBuild, string? str)
        => Invoke(wrapBuild).Append(str).Invoke(wrapBuild);

    public TextBuilder WrapFormat<T>(Action<TextBuilder>? wrapBuild, T? value, string? format = null,
        IFormatProvider? provider = null)
        => Invoke(wrapBuild).Format(value, format, provider).Invoke(wrapBuild);

#endregion

#region double

    public TextBuilder WrapAppend(char preChar, char postChar, char ch)
        => Append(preChar).Append(ch).Append(postChar);

    public TextBuilder WrapAppend(char preChar, char postChar, scoped text text)
        => Append(preChar).Append(text).Append(postChar);

    public TextBuilder WrapAppend(char preChar, char postChar, string? str)
        => Append(preChar).Append(str).Append(postChar);

    public TextBuilder WrapFormat<T>(char preChar, char postChar, T? value, string? format = null,
        IFormatProvider? provider = null)
        => Append(preChar).Format(value, format, provider).Append(postChar);

    public TextBuilder Wrap(char preChar, char postChar, Action<TextBuilder>? buildText)
        => Append(preChar).Invoke(buildText).Append(postChar);


    public TextBuilder WrapAppend(scoped text preText, scoped text postText, char ch)
        => Append(preText).Append(ch).Append(postText);

    public TextBuilder WrapAppend(scoped text preText, scoped text postText, scoped text text)
        => Append(preText).Append(text).Append(postText);

    public TextBuilder WrapAppend(scoped text preText, scoped text postText, string? str)
        => Append(preText).Append(str).Append(postText);

    public TextBuilder WrapFormat<T>(scoped text preText, scoped text postText, T? value, string? format = null,
        IFormatProvider? provider = null)
        => Append(preText).Format(value, format, provider).Append(postText);

    public TextBuilder Wrap(scoped text preText, scoped text postText, Action<TextBuilder>? buildText)
        => Append(preText).Invoke(buildText).Append(postText);

    public TextBuilder WrapAppend(string? preString, string? postString, char ch)
        => Append(preString).Append(ch).Append(postString);

    public TextBuilder WrapAppend(string? preString, string? postString, scoped text text)
        => Append(preString).Append(text).Append(postString);

    public TextBuilder WrapAppend(string? preString, string? postString, string? str)
        => Append(preString).Append(str).Append(postString);

    public TextBuilder WrapFormat<T>(string? preString, string? postString, T? value, string? format = null,
        IFormatProvider? provider = null)
        => Append(preString).Format(value, format, provider).Append(postString);

    public TextBuilder Wrap(string? preString, string? postString, Action<TextBuilder>? buildText)
        => Append(preString).Invoke(buildText).Append(postString);

    public TextBuilder WrapAppend(Action<TextBuilder>? preBuild, Action<TextBuilder>? postBuild, char ch)
        => Invoke(preBuild).Append(ch).Invoke(postBuild);

    public TextBuilder WrapAppend(Action<TextBuilder>? preBuild, Action<TextBuilder>? postBuild, scoped text text)
        => Invoke(preBuild).Append(text).Invoke(postBuild);

    public TextBuilder WrapAppend(Action<TextBuilder>? preBuild, Action<TextBuilder>? postBuild, string? str)
        => Invoke(preBuild).Append(str).Invoke(postBuild);

    public TextBuilder WrapFormat<T>(Action<TextBuilder>? preBuild, Action<TextBuilder>? postBuild, T? value,
        string? format = null,
        IFormatProvider? provider = null)
        => Invoke(preBuild).Format(value, format, provider).Invoke(postBuild);

    public TextBuilder Wrap(Action<TextBuilder>? preBuild, Action<TextBuilder>? postBuild, Action<TextBuilder>? buildText)
        => Invoke(preBuild).Invoke(buildText).Invoke(postBuild);

#endregion

#endregion

#region Insert

    void IList<char>.Insert(int index, char item) => Insert(index, item);

    public TextBuilder Insert(Index index, char ch)
    {
        AllocateAt(index, 1)[0] = ch;
        return this;
    }

    public TextBuilder Insert(Index index, scoped text text)
    {
        int len = text.Length;
        if (len > 0)
        {
            var slice = AllocateAt(index, len);
            Notsafe.Text.CopyBlock(text, slice, len);
        }

        return this;
    }

    public TextBuilder Insert(Index index, string? str)
    {
        if (str is not null)
        {
            int len = str.Length;
            if (len > 0)
            {
                var slice = AllocateAt(index, len);
                Notsafe.Text.CopyBlock(str, slice, len);
            }
        }

        return this;
    }

    public TextBuilder InsertFormat<T>(Index index, T? value, string? format = null, IFormatProvider? provider = null)
    {
        int pos = _position;

        int offset = Throw.IfBadInsertIndex(index, pos);

        if (offset == pos)
            return Format<T>(value, format, provider);

        Measure(tb => tb.Format<T>(value, format, provider), out var written);
        int len = written.Length;
        if (len > 0)
        {
            var buffer = AllocateAt(offset, len);
            // written has been offset
            written = _chars.AsSpan(pos + len, len);
            Notsafe.Text.CopyBlock(written, buffer, len);
            // trim off the end
            _position = pos + len;
        }

        return this;
    }

    public TextBuilder InsertRender<T>(Index index, T? value)
    {
        int pos = _position;

        int offset = Throw.IfBadInsertIndex(index, pos);

        if (offset == pos)
            return Render<T>(value);

        Measure(tb => tb.Render<T>(value), out var written);
        int len = written.Length;
        if (len > 0)
        {
            var buffer = AllocateAt(offset, len);
            // written has been offset
            written = _chars.AsSpan(pos + len, len);
            Notsafe.Text.CopyBlock(written, buffer, len);
            // trim off the end
            _position = pos + len;
        }

        return this;
    }

#endregion

#region IndexOf and Contains

    public bool Contains(char ch)
    {
#if NETSTANDARD2_1
        return Written.Contains(ch, null);
#else
        return Written.Contains(ch);
#endif
    }

    int IList<char>.IndexOf(char item) => TryFindIndex(item).SomeOr(-1);

    public Option<int> TryFindIndex(
        char ch,
        bool firstToLast = true,
        Index? index = null)
    {
        int pos = _position;
        int end = pos - 1;

        // starting index?
        int offset;
        if (index.TryGetValue(out Index idx))
        {
            if (!Validate.Index(idx, pos).IsOk(out offset))
                return None<int>();
        }
        else
        {
            // first-to-last: start at first item
            // last-to-first: start at the last item
            offset = firstToLast ? 0 : pos - 1;
        }

        // search
        var span = Written;
        if (firstToLast)
        {
            // we can scan until the last item
            for (; offset <= end; offset++)
            {
                if (span[offset] == ch)
                {
                    return Some(offset);
                }
            }
        }
        else
        {
            // we can scan until the first item
            for (; offset >= 0; offset--)
            {
                if (span[offset] == ch)
                {
                    return Some(offset);
                }
            }
        }

        // no match
        return None<int>();
    }

    public Option<int> TryFindIndex(
        char ch,
        StringComparison comparison,
        bool firstToLast = true,
        Index? index = null
    )
    {
        int pos = _position;
        int end = pos - 1;

        // starting index?
        int offset;
        if (index.TryGetValue(out Index idx))
        {
            if (!Validate.Index(idx, pos).IsOk(out offset))
                return None<int>();
        }
        else
        {
            // first-to-last: start at first item
            // last-to-first: start at the last item
            offset = firstToLast ? 0 : pos - 1;
        }

        // search
        var span = Written;
        var charSpan = ch.AsSpan();
        if (firstToLast)
        {
            for (; offset <= end; offset++)
            {
                if (TextHelper.Equate(span.Slice(offset, 1), charSpan, comparison))
                    return Some(offset);
            }
        }
        else
        {
            for (; offset >= 0; offset--)
            {
                if (TextHelper.Equate(span.Slice(offset, 1), charSpan, comparison))
                    return Some(offset);
            }
        }

        // no match
        return None<int>();
    }


    public Option<int> TryFindIndex(
        scoped text text,
        bool firstToLast = true,
        Index? index = null,
        StringComparison comparison = StringComparison.Ordinal)
    {
        int len = text.Length;
        int pos = _position;

        // nothing to find or thing to find is bigger than we are
        if ((len == 0) || (len > pos))
            return None<int>();

        // we can only scan until a certain ending item
        // any further and there wouldn't be enough characters to match
        int end = pos - len;

        // starting index?
        int offset;
        if (index.TryGetValue(out Index idx))
        {
            if (!Validate.Index(idx, pos).IsOk(out offset))
                return None<int>();
        }
        else
        {
            // first-to-last: start at first item
            // last-to-first: start at the last item
            offset = firstToLast ? 0 : pos - 1;
        }

        // clamp offset to what we can match on
        offset = offset.Clamp(0, end);

        // search
        var span = Written;
        if (firstToLast)
        {
            for (; offset <= end; offset++)
            {
                if (TextHelper.Equate(span.Slice(offset, len), text, comparison))
                    return Some(offset);
            }
        }
        else
        {
            for (; offset >= 0; offset--)
            {
                if (TextHelper.Equate(span.Slice(offset, len), text, comparison))
                    return Some(offset);
            }
        }

        // no match
        return None<int>();
    }

    public Option<int> TryFindIndex(string? str, bool firstToLast = true, Index? index = null,
        StringComparison comparison = StringComparison.Ordinal)
        => TryFindIndex(str.AsSpan(), firstToLast, index, comparison);

    public Option<(int Index, char Char)> TryFindIndex(
        Func<char, bool>? charPredicate,
        bool firstToLast = true,
        Index? index = null)
    {
        if (charPredicate is null)
            return None();

        int pos = _position;
        int end = pos - 1;

        // starting index?
        int offset;
        if (index.TryGetValue(out Index idx))
        {
            if (!Validate.Index(idx, pos).IsOk(out offset))
                return None();
        }
        else
        {
            // first-to-last: start at first item
            // last-to-first: start at the last item
            offset = firstToLast ? 0 : pos - 1;
        }

        // search
        var span = Written;
        if (firstToLast)
        {
            // we can scan until the last item
            for (; offset <= end; offset++)
            {
                if (charPredicate(span[offset]))
                {
                    return Some((offset, span[offset]));
                }
            }
        }
        else
        {
            // we can scan until the first item
            for (; offset >= 0; offset--)
            {
                if (charPredicate(span[offset]))
                {
                    return Some((offset, span[offset]));
                }
            }
        }

        // no match
        return None();
    }

#endregion


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

    public TextBuilder ForEach(FnRef<char, None>? refChar)
    {
        if (refChar is not null)
        {
            var span = Written;
            for (var i = 0; i < span.Length; i++)
            {
                refChar(ref span[i]);
            }
        }

        return this;
    }

    public TextBuilder ForEach(FnRef<char, int, None>? refCharIndex)
    {
        if (refCharIndex is not null)
        {
            var span = Written;
            for (var i = 0; i < span.Length; i++)
            {
                refCharIndex(ref span[i], i);
            }
        }

        return this;
    }

#endregion

    public TextBuilder Reverse()
    {
        Written.Reverse();
        return this;
    }

#region Removal

    bool ICollection<char>.Remove(char item) => throw new NotImplementedException();

    void IList<char>.RemoveAt(int index) => throw new NotImplementedException();

    public Result<char> RemoveAt(Index index)
    {
        if (!Validate.Index(index, _position).IsOkWithError(out var offset, out var ex))
            return ex;

        char removed = _chars[offset];
        var right = _chars.AsSpan(offset + 1);
        Notsafe.Text.CopyBlock(right, _chars.AsSpan(offset), right.Length);
        _position--;
        return Ok(removed);
    }

    public Result<char[]> RemoveAt(Range range)
    {
        if (!Validate.Range(range, _position).IsOkWithError(out var ok, out var ex))
            return ex;

        (int offset, int len) = ok;

        char[] removed = _chars.AsSpan(offset, len).ToArray();
        var right = _chars.AsSpan(offset + len);
        Notsafe.Text.CopyBlock(right, _chars.AsSpan(offset), right.Length);
        _position -= len;
        return Ok(removed);
    }

    public Result<int> RemoveWhere(Func<char, bool>? charPredicate)
    {
        if (charPredicate is null)
            return new ArgumentNullException(nameof(charPredicate));

        int freeIndex = 0; // the first free slot in span
        int pos = _position;
        var span = Written;

        // Find the first item which needs to be removed.
        while ((freeIndex < pos) && !charPredicate(span[freeIndex]))
            freeIndex++;

        if (freeIndex >= pos)
            return 0;

        int current = freeIndex + 1;
        while (current < pos)
        {
            // Find the first item which needs to be kept.
            while ((current < pos) && charPredicate(span[current]))
                current++;

            if (current < pos)
            {
                // copy item to the free slot
                span[freeIndex++] = span[current++];
            }
        }

        int removedCount = pos - freeIndex;
        _position = freeIndex;
        return Ok(removedCount);
    }

    public Result<int> RemoveLast(int count)
    {
        if (count > _position)
            return new ArgumentOutOfRangeException(nameof(count), count, $"There are only {_position} items to remove");
        _position -= count;
        return Ok(count);
    }

    void ICollection<char>.Clear() => Clear();

    public TextBuilder Clear(bool zeroFill = false)
    {
        if (zeroFill)
            Notsafe.Text.ClearBlock(Written);
        _position = 0;
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


#region Non-Fluent

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

#endregion

#region Former Extensions

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