namespace ScrubJay.Text;

public partial class TextBuilder
{
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
            TextHelper.Notsafe.CopyBlock(text, slice, len);
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
                TextHelper.Notsafe.CopyBlock(str, slice, len);
            }
        }

        return this;
    }

    public TextBuilder InsertFormat<T>(Index index, T? value, string? format = null, IFormatProvider? provider = null)
    {
        int pos = _position;

        int offset = Throw.IfBadInsertIndex(index, pos);

        if (offset == pos)
            return Append<T>(value, format, provider);

        Measure(tb => tb.Append<T>(value, format, provider), out var written);
        int len = written.Length;
        if (len > 0)
        {
            var buffer = AllocateAt(offset, len);
            // written has been offset
            written = _chars.AsSpan(pos + len, len);
            TextHelper.Notsafe.CopyBlock(written, buffer, len);
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
            return Append<T>(value, '@');

        Measure(tb => tb.Append<T>(value, '@'), out var written);
        int len = written.Length;
        if (len > 0)
        {
            var buffer = AllocateAt(offset, len);
            // written has been offset
            written = _chars.AsSpan(pos + len, len);
            TextHelper.Notsafe.CopyBlock(written, buffer, len);
            // trim off the end
            _position = pos + len;
        }

        return this;
    }
}