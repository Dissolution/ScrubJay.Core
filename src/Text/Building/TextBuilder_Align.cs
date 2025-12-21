namespace ScrubJay.Text;

public partial class TextBuilder
{
    public TextBuilder Align(
        char ch,
        int width,
        char paddingChar = ' ',
        Alignment alignment = Alignment.Right)
        => Align(ch.AsSpan(), width, paddingChar, alignment);

    public TextBuilder Align(
        string? str,
        int width,
        char paddingChar = ' ',
        Alignment alignment = Alignment.Right)
        => Align(str.AsSpan(), width, paddingChar, alignment);

    public TextBuilder Align(
        scoped text text,
        int width,
        char paddingChar = ' ',
        Alignment alignment = Alignment.Right)
    {
        // a negative width indicates a left-bias
        if (width < 0)
        {
            alignment |= Alignment.Left;
            width = -width;
        }
        // no width means append nothing
        else if (width == 0)
        {
            return this;
        }

        // fit text into width
        int len = text.Length;

        // no text: fill with padding
        if (len == 0)
        {
            return Repeat(width, paddingChar);
        }

        // if there is not enough space, we show a truncated version of the text
        if (width < len)
        {
            // if there is only one char available, we don't even show that, only an indicator
            if (width == 1)
            {
                return Append('…');
            }

            // right align, write the ellipsis and then the trailing text
            if (alignment == Alignment.Right)
            {
                return Append('…').Append(text[^(width - 1)..]);
            }

            // left align, write the starting text and then the ellipsis
            if (alignment == Alignment.Left)
            {
                return Append(text[..(width - 1)]).Append('…');
            }

            // center alignment requires two ellipsis
            Debug.Assert(alignment.HasFlags(Alignment.Center));
            if (width == 2)
            {
                // as above, only two available we only show indicator
                return Append("……");
            }

            // get the center part of the text
            width -= 2;
            double mid = (len / 2d) - (width / 2d);
            int start;
            if (alignment.HasFlags(Alignment.Left))
            {
                start = (int)Math.Floor(mid);
            }
            else
            {
                start = (int)Math.Ceiling(mid);
            }

            var slice = text.Slice(start, width);
            return Append('…').Append(slice).Append('…');
        }

        // calculate the amount of padding we have to add
        int padding = width - text.Length;

        // Use alignment
        if (alignment == Alignment.Left)
        {
            return Append(text).Repeat(padding, paddingChar);
        }

        if (alignment == Alignment.Right)
        {
            return Repeat(padding, paddingChar).Append(text);
        }

        Debug.Assert(alignment.HasFlags(Alignment.Center));

        // if padding is even, pre + post are the same
        if (padding.IsEven())
        {
            int pad = padding / 2;
            return Repeat(pad, paddingChar)
                .Append(text)
                .Repeat(pad, paddingChar);
        }

        // padding is odd, we need to use bias
        double half = padding / 2.0d;

        int pre;
        int post;

        // Center w/Left Bias?
        if (alignment.HasFlag(Alignment.Left))
        {
            pre = (int)Math.Floor(half);
            post = (int)Math.Ceiling(half);
        }
        else
        {
            // Defaults to Center w/Right Bias
            pre = (int)Math.Ceiling(half);
            post = (int)Math.Floor(half);
        }

        return Repeat(pre, paddingChar)
            .Append(text)
            .Repeat(post, paddingChar);
    }

    public TextBuilder AlignFormat<T>(
        T? value,
        int width,
        string? format = null,
        IFormatProvider? provider = null,
        char paddingChar = ' ',
        Alignment alignment = Alignment.Right)
    {
        // a negative width indicates a left-bias
        if (width < 0)
        {
            alignment |= Alignment.Left;
            width = -width;
        }
        // no width means append nothing
        else if (width == 0)
        {
            return this;
        }

        // have to format the value (otherwise we have no idea what len is)
        int start = _position;
        Format<T>(value, format, provider);
        int end = _position;
        int len = end - start;

        // fit text into width

        // no text; fill with padding
        if (len == 0)
        {
            return Repeat(width, paddingChar);
        }

        // not enough space?
        if (width < len)
        {
            if (width == 1)
            {
                // just an ellipsis
                _chars[start] = '…';
                _position = start + 1;
                return this;
            }
            else if (alignment == Alignment.Left)
            {
                // trim back before ellipsis
                end = (start + width) - 1;
                _chars[end] = '…';
                _position = start + width;
                return this;
            }
            else if (alignment == Alignment.Right)
            {
                // start with ellipsis
                _chars[start] = '…';
                // copy and trim
                var chunk = _chars.AsSpan(start, len)[^(width - 1)..];
                Debug.Assert(width - 1 == chunk.Length);
                TextHelper.Notsafe.CopyBlock(chunk, _chars.AsSpan(start + 1), width - 1);

                _position = start + width;
                return this;
            }
            else
            {
                Debug.Assert(alignment.HasFlags(Alignment.Center));
                if (width == 2)
                    return Append("……");

                var wrote = _chars.AsSpan(start, len);

                // get the center width-2 characters
                width -= 2;
                double mid = (len / 2d) - (width / 2d);
                if (alignment.HasFlags(Alignment.Left))
                {
                    start = (int)Math.Floor(mid);
                }
                else
                {
                    start = (int)Math.Ceiling(mid);
                }

                end = width - start;
                Debugger.Break();
                var slice = wrote.Slice(start, width);
                Debug.Assert(slice.Length == end - start);
                //return Append('…').Append(slice).Append('…');
                throw new NotImplementedException();
            }
        }

        throw new NotImplementedException();
        /*

        // calculate the amount of padding we have to add
        int padding = width - len;

        // Use alignment
        if (alignment == Alignment.Left)
        {
            // value has been written, rest is padding
            return RepeatAppend(padding, paddingChar);
        }

        if (alignment == Alignment.Right)
        {
            // we have to insert the padding before what we wrote
            Span<char> pad = stackalloc char[padding];
            pad.Fill(paddingChar);
            return Insert(start, pad);
        }

        Debug.Assert(alignment.HasFlags(Alignment.Center));

        // if padding is even, pre + post are the same
        if (padding.IsEven())
        {
            int pad = MathHelper.HalfRoundDown(padding);
            return RepeatAppend(pad, paddingChar)
                .Append(text)
                .RepeatAppend(pad, paddingChar);
        }

        // padding is odd, we need to use bias
        double half = padding / 2.0d;

        int pre;
        int post;

        // Center w/Left Bias?
        if (alignment.HasFlag(Alignment.Left))
        {
            pre = (int)Math.Floor(half);
            post = (int)Math.Ceiling(half);
        }
        else
        {
            // Defaults to Center w/Right Bias
            pre = (int)Math.Ceiling(half);
            post = (int)Math.Floor(half);
        }

        return RepeatAppend(pre, paddingChar)
            .Append(text)
            .RepeatAppend(post, paddingChar);

        */
        /*
        if (width == 0)
            return Append(value, format);

        // if no alignment is specified, we use -width as Left, +width as Right (same as string.Format)
        if (alignment == Alignment.None)
        {
            if (width < 0)
            {
                alignment = Alignment.Left;
                width = -width;
            }
            else
            {
                alignment = Alignment.Right;
            }
        }

        // Grab our position before formatting the value
        int start = _text.Count;

        // Format the value onto us
        Append<T>(value, format);

        // get end and length
        int end = _text.Count;
        int length = end - start;

        // calculate the amount of padding we have to add
        int padding = width - length;

        // as per string.Format, if width < text, we just write the value, which we've done
        if (padding <= 0)
            return _builder;

        // Fast path for padding == 1
        if (padding == 1)
        {
            // we start off left-aligned
            if (alignment.HasFlags(Alignment.Left))
                return Append(paddingChar);
            // shift one right
            return Insert(start, paddingChar);
        }

        // We're already left-aligned
        if (alignment == Alignment.Left)
        {
            return Repeat(padding, paddingChar);
        }

        // Everything after this point will involve some amount of insertion

        // Easy shift to right-align
        if (alignment == Alignment.Right)
        {
            return Insert(start, TextHelper.Repeat(padding, paddingChar));
        }

        // Assume centered at this point
        Debug.Assert(alignment.HasFlags(Alignment.Center));

        // if padding is even, pre + post are the same
        if (padding.IsEven())
        {
            string pad = TextHelper.Repeat(MathHelper.HalfRoundDown(padding), paddingChar);
            // Insert before, then just append after
            return Insert(start, pad).Append(pad);
        }

        // padding is odd, we need to use bias
        double half = padding / 2.0d;

        int pre;
        int post;

        // Center w/Left Bias?
        if (alignment.HasFlag(Alignment.Left))
        {
            pre = (int)Math.Floor(half);
            post = (int)Math.Ceiling(half);
        }
        else
        {
            // Defaults to Center w/Right Bias
            pre = (int)Math.Ceiling(half);
            post = (int)Math.Floor(half);
        }

        // Insert pre before, append post behind
        return Insert(start, TextHelper.Repeat(pre, paddingChar))
            .Repeat(post, paddingChar);
        */
    }
}