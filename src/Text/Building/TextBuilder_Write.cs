namespace ScrubJay.Text;

public partial class TextBuilder
{
    // Non-inline from List.Add to improve its code quality as uncommon path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowWrite(char ch)
    {
        Debug.Assert(_position == _chars.Length);
        int size = _position;
        GrowTo(size + 1);
        _position = size + 1;
        _chars[size] = ch;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Write(char ch)
    {
        Span<char> chars = _chars.AsSpan();
        int pos = _position;
        if (pos < chars.Length)
        {
            _position = pos + 1;
            chars[pos] = ch;
        }
        else
        {
            GrowWrite(ch);
        }
    }

    internal void Write(scoped text text)
    {
        if (!text.IsEmpty)
        {
            if (_position + text.Length > Capacity)
            {
                GrowBy(text.Length);
            }

            Notsafe.Text.CopyBlock(text, _chars.AsSpan(_position), text.Length);
            _position += text.Length;
        }
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Write(string? str) => Write(str.AsSpan());
#endif
}