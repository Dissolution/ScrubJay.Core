namespace ScrubJay.Text;

public partial class TextBuilder
{
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

    public TextBuilder Repeat<S>(int count, S state, Action<TextBuilder, S>? buildStatefulText)
    {
        if (buildStatefulText is not null)
        {
            for (int i = 0; i < count; i++)
            {
                buildStatefulText(this, state);
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

#region RepeatRender

    public TextBuilder RepeatRender<T>(int count, T? value)
    {
        if (count > 0)
        {
            int start = _position;
            Rendering.Renderer.RenderValue<T>(this, value);
            int pos = _position;
            Span<char> written = _chars.AsSpan(start, pos - start);
            return RepeatAppend(count - 1, written);
        }

        return this;
    }

#endregion
}