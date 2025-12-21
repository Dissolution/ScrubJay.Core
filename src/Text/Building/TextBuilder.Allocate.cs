namespace ScrubJay.Text;

partial class TextBuilder
{
    /// <summary>
    /// Allocates a <see cref="Span{T}"/> with a given <paramref name="length"/> in this <see cref="TextBuilder"/>.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public Span<char> Allocate(int length)
    {
        if (length <= 0)
            return [];

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
        int position = _position;
        int offset = Guard.InsertIndex(index, _position);
        if (length <= 0)
            return [];

        if (offset == _position)
            return Allocate(length);

        int newPosition = position + length;
        if (newPosition > Capacity)
        {
            GrowBy(length);
        }

        Sequence.SelfCopy(_chars, Range.OffsetLength(offset, length), (offset+length)..);

        // the hole we created
        Span<char> slice = _chars.Slice(offset, length);
        // clear it (we copied above, not moved)
        TextHelper.Clear(slice);
        _position = newPosition;
        return slice;
    }
}