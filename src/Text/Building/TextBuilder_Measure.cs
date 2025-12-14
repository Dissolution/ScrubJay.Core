namespace ScrubJay.Text;

partial class TextBuilder
{
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
            written = default;
        }

        return this;
    }

    public TextBuilder Measure<T>(T? value, out Span<char> written)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    {
        int start = _position;
        Write<T>(value);
        int end = _position;
        written = _chars.AsSpan(start, end - start);
        return this;
    }
}