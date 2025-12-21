namespace ScrubJay.Text;

public partial class TextBuilder
{
#region Repeat(action)

    public TextBuilder Repeat(int count, Action<TextBuilder>? build)
    {
        if (build is not null)
        {
            for (int i = 0; i < count; i++)
            {
                build(this);
            }
        }

        return this;
    }

    public TextBuilder Repeat<S>(int count, S state, Action<TextBuilder, S>? build)
#if NET9_0_OR_GREATER
        where S : allows ref struct
#endif
    {
        if (build is not null)
        {
            for (int i = 0; i < count; i++)
            {
                build(this, state);
            }
        }

        return this;
    }

#endregion

#region Repeat Append

    public TextBuilder Repeat(int count, char ch)
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

    public TextBuilder Repeat(int count, scoped text text)
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
                TextHelper.Notsafe.CopyBlock(text, ref span[pos], len);
            }

            Debug.Assert(pos == newPos);
            _position = newPos;
        }

        return this;
    }


    public TextBuilder Repeat(int count, string? str) => Repeat(count, str.AsSpan());

    public TextBuilder Repeat<T>(int count, T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (count > 0 && value is not null)
        {
            int start = _position;
            Append<T>(value);
            int pos = _position;
            Span<char> written = _chars.AsSpan(start, pos - start);
            return Repeat(count - 1, written);
        }

        return this;
    }

#endregion /Repeat Append

#region Repeat Format

    public TextBuilder Repeat<T>(int count, T? value, string? format, IFormatProvider? provider = null)
    {
        if (count > 0)
        {
            int start = _position;
            Format<T>(value, format, provider);
            int pos = _position;
            Span<char> written = _chars.AsSpan(start, pos - start);
            return Repeat(count - 1, written);
        }

        return this;
    }

    public TextBuilder Repeat<T>(int count, T? value, scoped text format, IFormatProvider? provider = null)
    {
        if (count > 0)
        {
            int start = _position;
            Format<T>(value, format, provider);
            int pos = _position;
            Span<char> written = _chars.AsSpan(start, pos - start);
            return Repeat(count - 1, written);
        }

        return this;
    }

    public TextBuilder Repeat<T>(int count, T? value, char format)
    {
        if (count > 0)
        {
            int start = _position;
            Format<T>(value, format);
            int pos = _position;
            Span<char> written = _chars.AsSpan(start, pos - start);
            return Repeat(count - 1, written);
        }

        return this;
    }

#if NET9_0_OR_GREATER
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TextBuilder Repeat<T>(int count, T? value, char format, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
    {
        if (count > 0)
        {
            int start = _position;
            Format<T>(value, format);
            int pos = _position;
            Span<char> written = _chars.AsSpan(start, pos - start);
            return Repeat(count - 1, written);
        }

        return this;
    }
#endif

    #endregion /Repeat Format
}