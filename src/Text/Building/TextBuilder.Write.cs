namespace ScrubJay.Text;

/* This portion of TextBuilder contains the underlying methods that write text directly to the rented array
 * These are designed for efficiency and do not return TextBuilder fluently for better inlining
 */

public partial class TextBuilder
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndWrite(char ch)
    {
        int pos = _position;
        Debug.Assert(pos == _chars.Length);
        char[] array = ArrayNest<char>.Rent(pos * 2);
        if (pos > 0)
        {
            Debug.Assert(_chars is not null);
            TextHelper.Notsafe.CopyBlock(_chars!, array, pos);
        }
        ArrayNest.Return(_chars, true);
        Debug.Assert(pos < array.Length);
        array[pos] = ch;
        _chars = array;
        _position = pos + 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndWrite(scoped text text)
    {
        int pos = _position;
        int newPos = pos + text.Length;
        Debug.Assert(newPos > _chars.Length);
        char[] array = ArrayNest<char>.Rent(newPos * 2);
        if (pos > 0)
        {
            Debug.Assert(_chars is not null);
            TextHelper.Notsafe.CopyBlock(_chars!, array, pos);
        }
        ArrayNest.Return(_chars, true);
        Debug.Assert(newPos <= array.Length);
        TextHelper.Notsafe.CopyBlock(text, ref array[pos], text.Length);
        _chars = array;
        _position = newPos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(char ch)
    {
        if (_position < _chars.Length)
        {
            _chars[_position] = ch;
            _position++;
        }
        else
        {
            GrowAndWrite(ch);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped text text)
    {
        if (text.TryCopyTo(Available))
        {
            _position += text.Length;
        }
        else
        {
            GrowAndWrite(text);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(string? str) => Write(str.AsSpan());
}