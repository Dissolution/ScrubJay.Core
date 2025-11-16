namespace ScrubJay.Text;

// sits between, not on


public ref struct TextCursor
{
    private readonly text _text;
    private int _position; // index of the 'next' item

    public text PreviousText
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _text[.._position];
    }

    public text NextText
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _text[_position..];
    }


    public bool MovePrevious()
    {
        int prev = _position - 1;
        if (prev < 0)
            return false;
        _position = prev;
        return true;
    }

    public bool MoveNext()
    {
        int next = _position + 1;
        if (next > _text.Length)
            return false;
        _position = next;
        return true;
    }
}