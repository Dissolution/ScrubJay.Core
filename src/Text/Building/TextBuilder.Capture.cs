namespace ScrubJay.Text;

partial class TextBuilder
{
    public TextBuilder Capture(Action<TextBuilder>? build, out Span<char> written)
    {
        if (build is not null)
        {
            int start = _position;
            build(this);
            int end = _position;
            written = _chars.AsSpan(start, end - start);
        }
        else
        {
            written = default;
        }

        return this;
    }

    public TextBuilder Capture<T>(T value, Action<TextBuilder, T>? buildValue, out Span<char> written)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (buildValue is not null)
        {
            int start = _position;
            buildValue(this, value);
            int end = _position;
            written = _chars.AsSpan(start, end - start);
        }
        else
        {
            written = default;
        }

        return this;
    }
}