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
        [HandlesResourceDisposal] [InterpolatedStringHandlerArgument("")] // pass this TextBuilder instance in as an argument
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
            interpolatedTextBuilder.Clear();
            return this;
        }
    }

#if NET9_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TextBuilder Append<T>(T? value, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
    {
        Write<T>(value, _);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TextBuilder Append<T>(T? value, char format, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
    {
        Write<T>(value, format, _);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append<T>(T? value, char format)
    {
        Write<T>(value, format, null);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append<T>(T? value, char format, IFormatProvider? provider)
    {
        Write<T>(value, format, provider);
        return this;
    }

#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append<T>(T? value, char format, IFormatProvider? provider = null)
    {
        Write<T>(value, format, provider);
        return this;
    }

#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append<T>(T? value)
    {
        Write<T>(value);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append<T>(T? value,
        string? format,
        IFormatProvider? provider = null)
    {
        Write<T>(value, format, provider);
        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append<T>(T? value, scoped text format, IFormatProvider? provider = null)
    {
        Write<T>(value, format, provider);
        return this;
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

#if NET9_0_OR_GREATER
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TextBuilder AppendMany<T>(IEnumerable<T>? values, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
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
#endif

    public TextBuilder AppendMany<T>(IEnumerable<T>? values)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Write<T>(value);
            }
        }

        return this;
    }

    public TextBuilder AppendMany<T>(IEnumerable<T>? values, string? format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Write<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder AppendMany<T>(IEnumerable<T>? values, char format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Write<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder AppendMany<T>(IEnumerable<T>? values, scoped text format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Write<T>(value, format, provider);
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

#if NET9_0_OR_GREATER
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TextBuilder AppendLine<T>(T? value, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
        => Append<T>(value).NewLine();
#endif

    public TextBuilder AppendLine<T>(T? value)
        => Append<T>(value).NewLine();

    public TextBuilder AppendLine<T>(T? value, string? format, IFormatProvider? provider = null)
        => Append<T>(value, format, provider).NewLine();

    public TextBuilder AppendLine<T>(T? value, char format, IFormatProvider? provider = null)
        => Append<T>(value, format, provider).NewLine();

    public TextBuilder AppendLine<T>(T? value, scoped text format, IFormatProvider? provider = null)
        => Append<T>(value, format, provider).NewLine();

#endregion
}