namespace ScrubJay.Text;

partial class TextBuilder
{
    public Span<char> Allocate(int length)
    {
        Throw.IfLessThan(length, 0);

        int pos = _position;
        int newPos = pos + length;
        if (newPos > Capacity)
        {
            GrowBy(length);
        }

        Span<char> slice = _chars.Slice(pos, length);
        TextHelper.Clear(slice);
        _position = newPos;
        return slice;
    }

    public Span<char> AllocateAt(Index index, int length)
    {
        int i = Throw.IfBadInsertIndex(index, _position);
        Throw.IfLessThan(length, 0);
        if (i == _position)
            return Allocate(length);

        int pos = _position;
        int newPos = pos + length;
        if (newPos > Capacity)
        {
            GrowBy(length);
        }

        // right side
        var right = _chars.AsSpan(new Range(i, pos));
        // where it will be written
        var target = _chars.AsSpan(i + length);
        // slide
        TextHelper.Notsafe.CopyBlock(right, target, right.Length);

        // the hole we created
        Span<char> slice = _chars.Slice(i, length);
        // clear it (we only copied, not moved)
        TextHelper.Clear(slice);

        _position = newPos;
        return slice;
    }
}