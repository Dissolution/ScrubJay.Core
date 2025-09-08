using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

partial class TextBuilder
{
    private const string COMMA_SPACE = ", ";

#region ReadOnlySpan<T>

#region Delimit - Render

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            RendererCache.RenderTo<T>(this, values[0]);
            for (var i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                RendererCache.RenderTo<T>(this, values[i]);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            RendererCache.RenderTo<T>(this, values[0]);
            for (var i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                RendererCache.RenderTo<T>(this, values[i]);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values)
        => Delimit(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            RendererCache.RenderTo<T>(this, values[0]);
            for (var i = 1; i < values.Length; i++)
            {
                Invoke(delimit);
                RendererCache.RenderTo<T>(this, values[i]);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Delimiter delimiter, scoped ReadOnlySpan<T> values)
    {
        return delimiter switch
        {
            Delimiter.None => RenderMany<T>(values),
            Delimiter.Comma => Delimit<T>(',', values),
            Delimiter.Space => Delimit<T>(' ', values),
            Delimiter.CommaSpace => Delimit<T>(COMMA_SPACE.AsSpan(), values),
            Delimiter.NewLine => Delimit<T>(static tb => tb.NewLine(), values),
            _ => throw InvalidEnumException.New(delimiter),
        };
    }

#endregion

#region Delimit - Format

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values, string? format,
        IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            WriteValue<T>(values[0], format, provider);
            for (var i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                WriteValue<T>(values[i], format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values, string? format,
        IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            WriteValue<T>(values[0], format, provider);
            for (var i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                WriteValue<T>(values[i], format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values, string? format, IFormatProvider? provider =
        null)
        => Delimit(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values, string? format,
        IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            WriteValue<T>(values[0], format, provider);
            for (var i = 1; i < values.Length; i++)
            {
                Invoke(delimit);
                WriteValue<T>(values[i], format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Delimiter delimiter, scoped ReadOnlySpan<T> values, string? format,
        IFormatProvider? provider = null)
    {
        return delimiter switch
        {
            Delimiter.None => FormatMany<T>(values, format, provider),
            Delimiter.Comma => Delimit<T>(',', values, format, provider),
            Delimiter.Space => Delimit<T>(' ', values, format, provider),
            Delimiter.CommaSpace => Delimit<T>(COMMA_SPACE.AsSpan(), values, format, provider),
            Delimiter.NewLine => Delimit<T>(static tb => tb.NewLine(), values, format, provider),
            _ => throw InvalidEnumException.New(delimiter),
        };
    }

#endregion

  #region Delimit - Build

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        if (!values.IsEmpty)
        {
            build?.Invoke(this, values[0]);
            for (var i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                build?.Invoke(this, values[i]);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        if (!values.IsEmpty)
        {
            build?.Invoke(this, values[0]);
            for (var i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                build?.Invoke(this, values[i]);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
        => Delimit(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        if (!values.IsEmpty)
        {
            build?.Invoke(this, values[0]);
            for (var i = 1; i < values.Length; i++)
            {
                Invoke(delimit);
                build?.Invoke(this, values[i]);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Delimiter delimiter, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        return delimiter switch
        {
            Delimiter.None => Enumerate<T>(values, build),
            Delimiter.Comma => Delimit<T>(',', values, build),
            Delimiter.Space => Delimit<T>(' ', values, build),
            Delimiter.CommaSpace => Delimit<T>(COMMA_SPACE.AsSpan(), values, build),
            Delimiter.NewLine => Delimit<T>(static tb => tb.NewLine(), values, build),
            _ => throw InvalidEnumException.New(delimiter),
        };
    }

#endregion

#endregion

#region T[]

#region Delimit - Render

    public TextBuilder Delimit<T>(char delimiter, T[]? values)
        => Delimit<T>(delimiter, new ReadOnlySpan<T>(values));

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values)
        => Delimit<T>(delimiter, new ReadOnlySpan<T>(values));

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values)
        => Delimit(delimiter.AsSpan(), new ReadOnlySpan<T>(values));
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values)
        => Delimit<T>(delimit, new ReadOnlySpan<T>(values));

    public TextBuilder Delimit<T>(Delimiter delimiter, T[]? values)
        => Delimit<T>(delimiter, new ReadOnlySpan<T>(values));

#endregion

#region Delimit - Format

    public TextBuilder Delimit<T>(char delimiter, T[]? values, string? format,
        IFormatProvider? provider = null)
        => Delimit<T>(delimiter, new ReadOnlySpan<T>(values), format, provider);

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values, string? format,
        IFormatProvider? provider = null)
        => Delimit<T>(delimiter, new ReadOnlySpan<T>(values), format, provider);

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values, string? format, IFormatProvider? provider =
        null)
        => Delimit<T>(delimiter.AsSpan(), new ReadOnlySpan<T>(values), format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values, string? format,
        IFormatProvider? provider = null)
        => Delimit<T>(delimit, new ReadOnlySpan<T>(values), format, provider);

    public TextBuilder Delimit<T>(Delimiter delimiter, T[]? values, string? format,
        IFormatProvider? provider = null)
        => Delimit<T>(delimiter, new ReadOnlySpan<T>(values), format, provider);

#endregion

#endregion


#region IEnumerable<T>

#region Delimit - Render

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            RendererCache.RenderTo<T>(this, e.Current);
            while (e.MoveNext())
            {
                Write(delimiter);
                RendererCache.RenderTo<T>(this, e.Current);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, IEnumerable<T>? values)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            RendererCache.RenderTo<T>(this, e.Current);
            while (e.MoveNext())
            {
                Write(delimiter);
                RendererCache.RenderTo<T>(this, e.Current);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values)
        => Delimit(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            RendererCache.RenderTo<T>(this, e.Current);
            while (e.MoveNext())
            {
                Invoke(delimit);
                RendererCache.RenderTo<T>(this, e.Current);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Delimiter delimiter, IEnumerable<T>? values)
    {
        return delimiter switch
        {
            Delimiter.None => RenderMany<T>(values),
            Delimiter.Comma => Delimit<T>(',', values),
            Delimiter.Space => Delimit<T>(' ', values),
            Delimiter.CommaSpace => Delimit<T>(COMMA_SPACE.AsSpan(), values),
            Delimiter.NewLine => Delimit<T>(static tb => tb.NewLine(), values),
            _ => throw InvalidEnumException.New(delimiter),
        };
    }

#endregion

#region Delimit - Format

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values, string? format,
        IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            WriteValue<T>(e.Current, format, provider);
            while (e.MoveNext())
            {
                Write(delimiter);
                WriteValue<T>(e.Current, format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, IEnumerable<T>? values, string? format,
        IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            WriteValue<T>(e.Current, format, provider);
            while (e.MoveNext())
            {
                Write(delimiter);
                WriteValue<T>(e.Current, format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values, string? format, IFormatProvider? provider =
        null)
        => Delimit(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values, string? format,
        IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            WriteValue<T>(e.Current, format, provider);
            while (e.MoveNext())
            {
                Invoke(delimit);
                WriteValue<T>(e.Current, format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Delimiter delimiter, IEnumerable<T>? values, string? format,
        IFormatProvider? provider = null)
    {
        return delimiter switch
        {
            Delimiter.None => FormatMany<T>(values, format, provider),
            Delimiter.Comma => Delimit<T>(',', values, format, provider),
            Delimiter.Space => Delimit<T>(' ', values, format, provider),
            Delimiter.CommaSpace => Delimit<T>(COMMA_SPACE.AsSpan(), values, format, provider),
            Delimiter.NewLine => Delimit<T>(static tb => tb.NewLine(), values, format, provider),
            _ => throw InvalidEnumException.New(delimiter),
        };
    }

#endregion

#endregion
}