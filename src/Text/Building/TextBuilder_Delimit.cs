using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

[PublicAPI]
public enum Delimiter
{
    None = 0,
    Comma,
    Space,
    CommaSpace,
    NewLine,
}

partial class TextBuilder
{
    private const char COMMA = ',';
    private const char SPACE = ' ';
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
            Delimiter.Comma => Delimit<T>(COMMA, values),
            Delimiter.Space => Delimit<T>(SPACE, values),
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
            Delimiter.Comma => Delimit<T>(COMMA, values, format, provider),
            Delimiter.Space => Delimit<T>(SPACE, values, format, provider),
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
            Delimiter.Comma => Delimit<T>(COMMA, values, build),
            Delimiter.Space => Delimit<T>(SPACE, values, build),
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

#region Delimit - Build

    public TextBuilder Delimit<T>(char delimiter, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter, new ReadOnlySpan<T>(values), build);

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter, new ReadOnlySpan<T>(values), build);

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values, Action<TextBuilder, T>? build)
        => Delimit(delimiter.AsSpan(), values, build);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimit, new ReadOnlySpan<T>(values), build);

    public TextBuilder Delimit<T>(Delimiter delimiter, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter, new ReadOnlySpan<T>(values), build);

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
            Delimiter.Comma => Delimit<T>(COMMA, values),
            Delimiter.Space => Delimit<T>(SPACE, values),
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
            Delimiter.Comma => Delimit<T>(COMMA, values, format, provider),
            Delimiter.Space => Delimit<T>(SPACE, values, format, provider),
            Delimiter.CommaSpace => Delimit<T>(COMMA_SPACE.AsSpan(), values, format, provider),
            Delimiter.NewLine => Delimit<T>(static tb => tb.NewLine(), values, format, provider),
            _ => throw InvalidEnumException.New(delimiter),
        };
    }

#endregion

#region Delimit - Build

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            build?.Invoke(this, e.Current);
            while (e.MoveNext())
            {
                Write(delimiter);
                build?.Invoke(this, e.Current);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            build?.Invoke(this, e.Current);
            while (e.MoveNext())
            {
                Write(delimiter);
                build?.Invoke(this, e.Current);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values, Action<TextBuilder, T>? build)
        => Delimit(delimiter.AsSpan(), values, build);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return this;

            build?.Invoke(this, e.Current);
            while (e.MoveNext())
            {
                Invoke(delimit);
                build?.Invoke(this, e.Current);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Delimiter delimiter, IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        return delimiter switch
        {
            Delimiter.None => Enumerate<T>(values, build),
            Delimiter.Comma => Delimit<T>(COMMA, values, build),
            Delimiter.Space => Delimit<T>(SPACE, values, build),
            Delimiter.CommaSpace => Delimit<T>(COMMA_SPACE.AsSpan(), values, build),
            Delimiter.NewLine => Delimit<T>(static tb => tb.NewLine(), values, build),
            _ => throw InvalidEnumException.New(delimiter),
        };
    }

#endregion

#endregion

#region Iterable

#region Delimit - Render

    public TextBuilder Delimit<T>(char delimiter, Func<Option<T>>? iterate)
    {
        if (iterate is not null && iterate().IsSome(out var value))
        {
            WriteValue<T>(value);
            while (iterate().IsSome(out value))
            {
                Write(delimiter);
                WriteValue<T>(value);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, Func<Option<T>>? iterate)
    {
        if (iterate is not null && iterate().IsSome(out var value))
        {
            WriteValue<T>(value);
            while (iterate().IsSome(out value))
            {
                Write(delimiter);
                WriteValue<T>(value);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, Func<Option<T>>? iterate)
        => Delimit(delimiter.AsSpan(), iterate);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, Func<Option<T>>? iterate)
    {
        if (iterate is not null && iterate().IsSome(out var value))
        {
            WriteValue<T>(value);
            while (iterate().IsSome(out value))
            {
                Invoke(delimit);
                WriteValue<T>(value);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Delimiter delimiter, Func<Option<T>>? iterate)
    {
        return delimiter switch
        {
            Delimiter.None => RenderMany<T>(iterate),
            Delimiter.Comma => Delimit<T>(COMMA, iterate),
            Delimiter.Space => Delimit<T>(SPACE, iterate),
            Delimiter.CommaSpace => Delimit<T>(COMMA_SPACE.AsSpan(), iterate),
            Delimiter.NewLine => Delimit<T>(static tb => tb.NewLine(), iterate),
            _ => throw InvalidEnumException.New(delimiter),
        };
    }

#endregion

#region Delimit - Format

    public TextBuilder Delimit<T>(char delimiter, Func<Option<T>>? iterate, string? format,
        IFormatProvider? provider = null)
    {
        if (iterate is not null && iterate().IsSome(out var value))
        {
            WriteValue<T>(value, format, provider);
            while (iterate().IsSome(out value))
            {
                Write(delimiter);
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, Func<Option<T>>? iterate, string? format,
        IFormatProvider? provider = null)
    {
        if (iterate is not null && iterate().IsSome(out var value))
        {
            WriteValue<T>(value, format, provider);
            while (iterate().IsSome(out value))
            {
                Write(delimiter);
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, Func<Option<T>>? iterate, string? format, IFormatProvider? provider =
        null)
        => Delimit(delimiter.AsSpan(), iterate, format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, Func<Option<T>>? iterate, string? format,
        IFormatProvider? provider = null)
    {
        if (iterate is not null && iterate().IsSome(out var value))
        {
            WriteValue<T>(value, format, provider);
            while (iterate().IsSome(out value))
            {
                Invoke(delimit);
                WriteValue<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Delimiter delimiter, Func<Option<T>>? iterate, string? format,
        IFormatProvider? provider = null)
    {
        return delimiter switch
        {
            Delimiter.None => FormatMany<T>(iterate, format, provider),
            Delimiter.Comma => Delimit<T>(COMMA, iterate, format, provider),
            Delimiter.Space => Delimit<T>(SPACE, iterate, format, provider),
            Delimiter.CommaSpace => Delimit<T>(COMMA_SPACE.AsSpan(), iterate, format, provider),
            Delimiter.NewLine => Delimit<T>(static tb => tb.NewLine(), iterate, format, provider),
            _ => throw InvalidEnumException.New(delimiter),
        };
    }

#endregion

#region Delimit - Build

    public TextBuilder Delimit<T>(char delimiter, Func<Option<T>>? iterate, Action<TextBuilder, T>? build)
    {
        if (iterate is not null &&
            build is not null &&
            iterate().IsSome(out var value))
        {
            build(this, value);
            while (iterate().IsSome(out value))
            {
                Write(delimiter);
                build(this, value);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, Func<Option<T>>? iterate, Action<TextBuilder, T>? build)
    {
        if (iterate is not null &&
            build is not null &&
            iterate().IsSome(out var value))
        {
            build(this, value);
            while (iterate().IsSome(out value))
            {
                Write(delimiter);
                build(this, value);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, Func<Option<T>>? iterate, Action<TextBuilder, T>? build)
        => Delimit(delimiter.AsSpan(), iterate, build);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, Func<Option<T>>? iterate, Action<TextBuilder, T>? build)
    {
        if (iterate is not null &&
            build is not null &&
            iterate().IsSome(out var value))
        {
            build(this, value);
            while (iterate().IsSome(out value))
            {
                Invoke(delimit);
                build(this, value);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Delimiter delimiter, Func<Option<T>>? iterate, Action<TextBuilder, T>? build)
    {
        return delimiter switch
        {
            Delimiter.None => Enumerate<T>(iterate, build),
            Delimiter.Comma => Delimit<T>(COMMA, iterate, build),
            Delimiter.Space => Delimit<T>(SPACE, iterate, build),
            Delimiter.CommaSpace => Delimit<T>(COMMA_SPACE.AsSpan(), iterate, build),
            Delimiter.NewLine => Delimit<T>(static tb => tb.NewLine(), iterate, build),
            _ => throw InvalidEnumException.New(delimiter),
        };
    }

#endregion

#endregion
}