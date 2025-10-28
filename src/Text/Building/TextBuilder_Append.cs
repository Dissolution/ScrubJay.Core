namespace ScrubJay.Text;

/* APPEND
 * All Append operations act like StringBuilder.Append:
 * They append text-like values directly, and then other values by calling `.ToString()` on that value
 */

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

    public TextBuilder Append(
        [HandlesResourceDisposal]
        [InterpolatedStringHandlerArgument("")] // pass this TextBuilder instance in as an argument
        ref InterpolatedTextBuilder interpolatedTextBuilder)
    {
        if (ReferenceEquals(interpolatedTextBuilder._builder, this))
        {
            // the writing has already occured by this point
            return this;
        }
        else
        {
            Write(interpolatedTextBuilder.AsSpan());
            interpolatedTextBuilder.Dispose();
            return this;
        }
    }


    public TextBuilder Append<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        // stringify can be called on any value, including ref structs
        return Append(value.Stringify());
    }

#endregion

#region AppendMany

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder AppendMany(params text characters) => Append(characters);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder AppendMany(char[]? characters) => Append(new text(characters));

    public TextBuilder AppendMany(IEnumerable<char>? characters)
    {
        if (characters is ICollection<char> collection)
        {
#if !NETFRAMEWORK && !NETSTANDARD
            if (characters is List<char> list)
            {
                var listSpan = CollectionsMarshal.AsSpan(list);
                return Append(listSpan);
            }
#endif

            int count = collection.Count;
            if (count + _position > Capacity)
                GrowBy(count);
            collection.CopyTo(_chars, _position);
            _position += count;
        }
        else if (characters is not null)
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

    public TextBuilder AppendMany<T>(IEnumerable<T>? values)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Write(value.Stringify());
            }
        }

        return this;
    }

#endregion

#region AppendLine

    public TextBuilder AppendLine(char ch) => Append(ch).NewLine();

    public TextBuilder AppendLine(scoped text text) => Append(text).NewLine();

    public TextBuilder AppendLine(string? str) => Append(str).NewLine();

    public TextBuilder AppendLine(
        [InterpolatedStringHandlerArgument("")]
        ref InterpolatedTextBuilder interpolatedTextBuilder)
    {
        // writing has occurred
        return NewLine();
    }

#endregion
}