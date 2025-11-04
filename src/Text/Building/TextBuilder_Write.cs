namespace ScrubJay.Text;

/* This portion of TextBuilder contains the underlying methods that write text directly to the rented array
 * These are designed for efficiency and do not return TextBuilder fluently for better inlining
 */

public partial class TextBuilder
{
    public void Write(char ch)
    {
        if (_position >= Capacity)
        {
            GrowBy(1);
        }

        _chars[_position] = ch;
        _position++;
    }

    public void Write(scoped text text)
    {
        if (!text.IsEmpty)
        {
            if (_position + text.Length > Capacity)
            {
                GrowBy(text.Length);
            }

            TextHelper.Notsafe.CopyBlock(text, _chars.AsSpan(_position), text.Length);
            _position += text.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(string? str) => Write(str.AsSpan());
}