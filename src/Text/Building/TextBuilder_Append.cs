namespace ScrubJay.Text;

public partial class TextBuilder
{
#region Append

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append(char ch)
    {
        Write(ch);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append(scoped text text)
    {
        Write(text);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append(string? str)
    {
        Write(str);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append(char[]? chars)
    {
        Write(chars);
        return this;
    }

#pragma warning disable IDE0060, CA2000
    public TextBuilder Append(
        [HandlesResourceDisposal] [InterpolatedStringHandlerArgument("")] // pass this TextBuilder instance in as an argument
        ref InterpolatedTextBuilder interpolatedTextBuilder)
    {
        // as this TextBuilder instance was passed into the InterpolatedTextBuilder's constructor,
        // all the writing has already occurred

        // not needed!
        interpolatedTextBuilder.Dispose();

        return this;
    }
#pragma warning restore IDE0060, CA2000

#endregion

#region AppendMany

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder AppendMany(params text characters) => Append(characters);

    public TextBuilder AppendMany(IEnumerable<char>? characters)
    {
        if (characters is not null)
        {
            foreach (char ch in characters)
            {
                Write(ch);
            }
        }

        return this;
    }

    public TextBuilder AppendMany(IEnumerable<string?>? strings)
    {
        if (strings is not null)
        {
            foreach (string? str in strings)
            {
                Write(str);
            }
        }

        return this;
    }

#endregion

#region AppendMany with Delimiter

    public TextBuilder AppendMany(scoped text text, Delimiter delimiter)
    {
        if (!text.IsEmpty)
        {
            Write(text[0]);
            for (var i = 1; i < text.Length; i++)
            {
                delimiter.Invoke(this);
                Write(text[i]);
            }
        }

        return this;
    }

    public TextBuilder AppendMany(IEnumerable<char>? characters, Delimiter delimiter)
    {
        if (characters is not null)
        {
            using var e = characters.GetEnumerator();
            if (!e.MoveNext())
                return this;

            Write(e.Current);
            while (e.MoveNext())
            {
                delimiter.Invoke(this);
                Write(e.Current);
            }
        }

        return this;
    }

    public TextBuilder AppendMany(IEnumerable<string?>? strings, Delimiter delimiter)
    {
        if (strings is not null)
        {
            using var e = strings.GetEnumerator();
            if (!e.MoveNext())
                return this;

            Write(e.Current);
            while (e.MoveNext())
            {
                delimiter.Invoke(this);
                Write(e.Current);
            }
        }

        return this;
    }

#endregion

#region AppendLine

    public TextBuilder AppendLine(char ch) => Append(ch).NewLine();

    public TextBuilder AppendLine(params text text) => Append(text).NewLine();

    public TextBuilder AppendLine(string? str) => Append(str).NewLine();

    public TextBuilder AppendLine(char[]? chars) => Append(chars).NewLine();

#pragma warning disable IDE0060, CA2000
    public TextBuilder AppendLine(
        [InterpolatedStringHandlerArgument("")] [HandlesResourceDisposal]
        ref InterpolatedTextBuilder interpolatedText) => Append(ref interpolatedText).NewLine();
#pragma warning restore IDE0060, CA2000

#endregion
}