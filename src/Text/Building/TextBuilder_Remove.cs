namespace ScrubJay.Text;

public partial class TextBuilder
{
    #region Removal

    bool ICollection<char>.Remove(char item) => throw new NotImplementedException();

    void IList<char>.RemoveAt(int index) => throw new NotImplementedException();

    public Result<char> RemoveAt(Index index)
    {
        if (!Validate.Index(index, _position).IsOk(out var offset, out var ex))
            return ex;

        char removed = _chars[offset];
        var right = _chars.AsSpan(offset + 1);
        Notsafe.Text.CopyBlock(right, _chars.AsSpan(offset), right.Length);
        _position--;
        return Ok(removed);
    }

    public Result<char[]> RemoveAt(Range range)
    {
        if (!Validate.Range(range, _position).IsOk(out var ok, out var ex))
            return ex;

        (int offset, int len) = ok;

        char[] removed = _chars.AsSpan(offset, len).ToArray();
        var right = _chars.AsSpan(offset + len);
        Notsafe.Text.CopyBlock(right, _chars.AsSpan(offset), right.Length);
        _position -= len;
        return Ok(removed);
    }

    public Result<int> RemoveWhere(Func<char, bool>? charPredicate)
    {
        if (charPredicate is null)
            return new ArgumentNullException(nameof(charPredicate));

        int freeIndex = 0; // the first free slot in span
        int pos = _position;
        var span = Written;

        // Find the first item which needs to be removed.
        while ((freeIndex < pos) && !charPredicate(span[freeIndex]))
            freeIndex++;

        if (freeIndex >= pos)
            return Ok(0);

        int current = freeIndex + 1;
        while (current < pos)
        {
            // Find the first item which needs to be kept.
            while ((current < pos) && charPredicate(span[current]))
                current++;

            if (current < pos)
            {
                // copy item to the free slot
                span[freeIndex++] = span[current++];
            }
        }

        int removedCount = pos - freeIndex;
        _position = freeIndex;
        return Ok(removedCount);
    }

    public Result<int> RemoveLast(int count)
    {
        if (count > _position)
            return new ArgumentOutOfRangeException(nameof(count), count, $"There are only {_position} items to remove");
        _position -= count;
        return Ok(count);
    }

    void ICollection<char>.Clear() => Clear();

    public TextBuilder Clear(bool zeroFill = false)
    {
        if (zeroFill)
            Notsafe.Text.ClearBlock(Written);
        _position = 0;
        return this;
    }

#endregion

}