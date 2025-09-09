namespace ScrubJay.Text;

public partial class TextBuilder
{
    #region IndexOf and Contains

    public bool Contains(char ch)
    {
#if NETSTANDARD2_1
        return Written.Contains(ch, null);
#else
        return Written.Contains(ch);
#endif
    }

    public Option<int> TryFindIndex(
        char ch,
        bool firstToLast = true,
        Index? index = null)
    {
        int pos = _position;
        int end = pos - 1;

        // starting index?
        int offset;
        if (index.TryGetValue(out Index idx))
        {
            if (!Validate.Index(idx, pos).IsOk(out offset))
                return None;
        }
        else
        {
            // first-to-last: start at first item
            // last-to-first: start at the last item
            offset = firstToLast ? 0 : pos - 1;
        }

        // search
        var span = Written;
        if (firstToLast)
        {
            // we can scan until the last item
            for (; offset <= end; offset++)
            {
                if (span[offset] == ch)
                {
                    return Some(offset);
                }
            }
        }
        else
        {
            // we can scan until the first item
            for (; offset >= 0; offset--)
            {
                if (span[offset] == ch)
                {
                    return Some(offset);
                }
            }
        }

        // no match
        return None;
    }

    public Option<int> TryFindIndex(
        char ch,
        StringComparison comparison,
        bool firstToLast = true,
        Index? index = null
    )
    {
        int pos = _position;
        int end = pos - 1;

        // starting index?
        int offset;
        if (index.TryGetValue(out Index idx))
        {
            if (!Validate.Index(idx, pos).IsOk(out offset))
                return None;
        }
        else
        {
            // first-to-last: start at first item
            // last-to-first: start at the last item
            offset = firstToLast ? 0 : pos - 1;
        }

        // search
        var span = Written;
        var charSpan = ch.AsSpan();
        if (firstToLast)
        {
            for (; offset <= end; offset++)
            {
                if (TextHelper.Equate(span.Slice(offset, 1), charSpan, comparison))
                    return Some(offset);
            }
        }
        else
        {
            for (; offset >= 0; offset--)
            {
                if (TextHelper.Equate(span.Slice(offset, 1), charSpan, comparison))
                    return Some(offset);
            }
        }

        // no match
        return None;
    }


    public Option<int> TryFindIndex(
        scoped text text,
        bool firstToLast = true,
        Index? index = null,
        StringComparison comparison = StringComparison.Ordinal)
    {
        int len = text.Length;
        int pos = _position;

        // nothing to find or thing to find is bigger than we are
        if ((len == 0) || (len > pos))
            return None;

        // we can only scan until a certain ending item
        // any further and there wouldn't be enough characters to match
        int end = pos - len;

        // starting index?
        int offset;
        if (index.TryGetValue(out Index idx))
        {
            if (!Validate.Index(idx, pos).IsOk(out offset))
                return None;
        }
        else
        {
            // first-to-last: start at first item
            // last-to-first: start at the last item
            offset = firstToLast ? 0 : pos - 1;
        }

        // clamp offset to what we can match on
        offset = offset.Clamp(0, end);

        // search
        var span = Written;
        if (firstToLast)
        {
            for (; offset <= end; offset++)
            {
                if (TextHelper.Equate(span.Slice(offset, len), text, comparison))
                    return Some(offset);
            }
        }
        else
        {
            for (; offset >= 0; offset--)
            {
                if (TextHelper.Equate(span.Slice(offset, len), text, comparison))
                    return Some(offset);
            }
        }

        // no match
        return None;
    }

    public Option<int> TryFindIndex(string? str, bool firstToLast = true, Index? index = null,
        StringComparison comparison = StringComparison.Ordinal)
        => TryFindIndex(str.AsSpan(), firstToLast, index, comparison);

    public Option<(int Index, char Char)> TryFindIndex(
        Func<char, bool>? charPredicate,
        bool firstToLast = true,
        Index? index = null)
    {
        if (charPredicate is null)
            return None;

        int pos = _position;
        int end = pos - 1;

        // starting index?
        int offset;
        if (index.TryGetValue(out Index idx))
        {
            if (!Validate.Index(idx, pos).IsOk(out offset))
                return None;
        }
        else
        {
            // first-to-last: start at first item
            // last-to-first: start at the last item
            offset = firstToLast ? 0 : pos - 1;
        }

        // search
        var span = Written;
        if (firstToLast)
        {
            // we can scan until the last item
            for (; offset <= end; offset++)
            {
                if (charPredicate(span[offset]))
                {
                    return Some((offset, span[offset]));
                }
            }
        }
        else
        {
            // we can scan until the first item
            for (; offset >= 0; offset--)
            {
                if (charPredicate(span[offset]))
                {
                    return Some((offset, span[offset]));
                }
            }
        }

        // no match
        return None;
    }

#endregion

}