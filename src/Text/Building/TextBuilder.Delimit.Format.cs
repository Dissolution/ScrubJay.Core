using ScrubJay.Iteration;
// ReSharper disable InlineOutVariableDeclaration

namespace ScrubJay.Text;

partial class TextBuilder
{
#region Action: Format

#region Values: ReadOnlySpan<T>

#region Format: string?

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values, string? format,
        IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Format<T>(values[i], format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values, string? format,
        IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Format<T>(values[i], format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values,
        string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values, format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values, string? format,
        IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                delimit?.Invoke(this);
                Format<T>(values[i], format, provider);
            }
        }

        return this;
    }

#endregion /Format: string?

#region Format: text

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values, scoped text format,
        IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Format<T>(values[i], format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values, scoped text format,
        IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Format<T>(values[i], format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values,
        scoped text format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values, format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values, scoped text format,
        IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                delimit?.Invoke(this);
                Format<T>(values[i], format, provider);
            }
        }

        return this;
    }

#endregion /Format: text

#region Format: char

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values, char format)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Format<T>(values[i], format);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values, char format)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Format<T>(values[i], format);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values, char format)
        => Delimit<T>(delimiter.AsSpan(), values, format);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values, char format)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format);
            for (int i = 1; i < values.Length; i++)
            {
                delimit?.Invoke(this);
                Format<T>(values[i], format);
            }
        }

        return this;
    }

#endregion /Format: char

#endregion Values: ReadOnlySpan<T>

#region Values: T[]

#region Format: string?

    public TextBuilder Delimit<T>(char delimiter, T[]? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter, values.AsSpan(), format, provider);

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter, values.AsSpan(), format, provider);

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values.AsSpan(), format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimit, values.AsSpan(), format, provider);

#endregion /Format: string?

#region Format: text

    public TextBuilder Delimit<T>(char delimiter, T[]? values, scoped text format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter, values.AsSpan(), format, provider);

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values, scoped text format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter, values.AsSpan(), format, provider);

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values, scoped text format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values.AsSpan(), format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values, scoped text format,
        IFormatProvider? provider = null)
        => Delimit<T>(delimit, values.AsSpan(), format, provider);

#endregion /Format: text

#region Format: char

    public TextBuilder Delimit<T>(char delimiter, T[]? values, char format)
        => Delimit<T>(delimiter, values.AsSpan(), format);

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values, char format)
        => Delimit<T>(delimiter, values.AsSpan(), format);

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values, char format)
        => Delimit<T>(delimiter.AsSpan(), values.AsSpan(), format);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values, char format)
        => Delimit<T>(delimit, values.AsSpan(), format);

#endregion /Format: char

#endregion /Values: T[]

#region Values: IEnumerable<T>

#region Format: string?

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values, string? format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Format<T>(e.Current, format, provider);
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
            if (e.MoveNext())
            {
                Format<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Format<T>(e.Current, format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values, format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values, string? format,
        IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                delimit?.Invoke(this);
                Format<T>(e.Current, format, provider);
            }
        }

        return this;
    }

#endregion /Format: string?

#region Format: text

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values, scoped text format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Format<T>(e.Current, format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, IEnumerable<T>? values, scoped text format,
        IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Format<T>(e.Current, format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values, scoped text format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values, format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values, scoped text format,
        IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                delimit?.Invoke(this);
                Format<T>(e.Current, format, provider);
            }
        }

        return this;
    }

#endregion /Format: text

#region Format: char

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values, char format)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Format<T>(e.Current, format);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, IEnumerable<T>? values, char format)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Format<T>(e.Current, format);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values, char format)
        => Delimit<T>(delimiter.AsSpan(), values, format);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values, char format)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format);
            }

            while (e.MoveNext())
            {
                delimit?.Invoke(this);
                Format<T>(e.Current, format);
            }
        }

        return this;
    }

#endregion /Format: char

#endregion Values: IEnumerable<T>

#region Values: Iterable

#region Format: string?

    public TextBuilder Delimit<R, V>(char delimiter, Iterable<R, V> iterable, string? format, IFormatProvider? provider = null)
        where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Format<V>(value, format, provider);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Format<V>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<R, V>(scoped text delimiter, Iterable<R, V> iterable, string? format,
        IFormatProvider? provider = null)
        where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Format<V>(value, format, provider);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Format<V>(value, format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<R, V>(string? delimiter, Iterable<R, V> iterable, string? format,
        IFormatProvider? provider = null)
        where R : struct, IIterator<V>
        => Delimit<R, V>(delimiter.AsSpan(), iterable, format, provider);
#endif

    public TextBuilder Delimit<R, V>(Action<TextBuilder>? delimit, Iterable<R, V> iterable, string? format,
        IFormatProvider? provider = null)
        where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Format<V>(value, format, provider);
            while (iterator.TryMoveNext(out value))
            {
                delimit?.Invoke(this);
                Format<V>(value, format, provider);
            }
        }

        return this;
    }

#endregion /Format: string?

#region Format: text

    public TextBuilder Delimit<R, V>(char delimiter, Iterable<R, V> iterable, scoped text format,
        IFormatProvider? provider = null)
        where R : struct, IIterator<V>
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Format<V>(value, format, provider);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Format<V>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<R, V>(scoped text delimiter, Iterable<R, V> iterable, scoped text format,
        IFormatProvider? provider = null)
        where R : struct, IIterator<V>
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Format<V>(value, format, provider);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Format<V>(value, format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<R, V>(string? delimiter, Iterable<R, V> iterable, scoped text format,
        IFormatProvider? provider = null)
        where R : struct, IIterator<V>
        => Delimit<R, V>(delimiter.AsSpan(), iterable, format, provider);
#endif

    public TextBuilder Delimit<R, V>(Action<TextBuilder>? delimit, Iterable<R, V> iterable, scoped text format,
        IFormatProvider? provider = null)
        where R : struct, IIterator<V>
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Format<V>(value, format, provider);
            while (iterator.TryMoveNext(out value))
            {
                delimit?.Invoke(this);
                Format<V>(value, format, provider);
            }
        }

        return this;
    }

#endregion /Format: text

#region Format: text

    public TextBuilder Delimit<R, V>(char delimiter, Iterable<R, V> iterable, char format)
        where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
        , allows ref struct
        where V : allows ref struct
#endif
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Format<V>(value, format);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Format<V>(value, format);
            }
        }

        return this;
    }

    public TextBuilder Delimit<R, V>(scoped text delimiter, Iterable<R, V> iterable, char format)
        where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
        , allows ref struct
        where V : allows ref struct
#endif
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Format<V>(value, format);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Format<V>(value, format);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<R, V>(string? delimiter, Iterable<R, V> iterable, char format)
        where R : struct, IIterator<V>
        => Delimit<R, V>(delimiter.AsSpan(), iterable, format);
#endif

    public TextBuilder Delimit<R, V>(Action<TextBuilder>? delimit, Iterable<R, V> iterable, char format)
        where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
        , allows ref struct
        where V : allows ref struct
#endif
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Format<V>(value, format);
            while (iterator.TryMoveNext(out value))
            {
                delimit?.Invoke(this);
                Format<V>(value, format);
            }
        }

        return this;
    }

#endregion /Format: char

#endregion /Values: Iterable<T>

#endregion /Action: Format
}