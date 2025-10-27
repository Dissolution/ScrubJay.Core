namespace ScrubJay.Text;

public partial class TextBuilder
{
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
                Notsafe.Text.CopyBlock(text, span[pos..], len);
            }

            Debug.Assert(pos == newPos);
            _position = newPos;
        }

        return this;
    }


    public TextBuilder Repeat(int count, string? str)
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
}