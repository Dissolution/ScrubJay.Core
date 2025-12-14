using ScrubJay.Iteration;

namespace ScrubJay.Text;

partial class TextBuilder
{
#region Delimit

#region Values: Append

#region Values: ReadOnlySpan<T>

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            Write<T>(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Write<T>(values[i]);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            Write<T>(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Write<T>(values[i]);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values)
        => Delimit<T>(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            Write<T>(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                delimit?.Invoke(this);
                Write<T>(values[i]);
            }
        }

        return this;
    }

#endregion Values: ReadOnlySpan<T>

#region Values: T[]

    public TextBuilder Delimit<T>(char delimiter, T[]? values)
        => Delimit<T>(delimiter, values.AsSpan());

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values)
        => Delimit<T>(delimiter, values.AsSpan());

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values)
        => Delimit<T>(delimiter.AsSpan(), values.AsSpan());
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values)
        => Delimit<T>(delimit, values.AsSpan());

#endregion Values: T[]

#region Values: IEnumerable<T>

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Write<T>(e.Current);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Write<T>(e.Current);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, IEnumerable<T>? values)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Write<T>(e.Current);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Write<T>(e.Current);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values)
        => Delimit<T>(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Write<T>(e.Current);
                while (e.MoveNext())
                {
                    delimit?.Invoke(this);
                    Write<T>(e.Current);
                }
            }
        }

        return this;
    }

#endregion Values: IEnumerable<T>

#region Values: Iterable

    public TextBuilder Delimit<R, V>(char delimiter, Iterable<R, V> iterable)
        where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        V? value;
        var iterator = iterable.Iterator;
        if (iterator.TryMoveNext(out value))
        {
            Write<V>(value);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Write<V>(value);
            }
        }

        return this;
    }

    public TextBuilder Delimit<R, V>(scoped text delimiter, Iterable<R, V> iterable)
        where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        V? value;
        var iterator = iterable.Iterator;
        if (iterator.TryMoveNext(out value))
        {
            Write<V>(value);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Write<V>(value);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<R, V>(string? delimiter, Iterable<R, V> iterable)
        where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        => Delimit<R, V>(delimiter.AsSpan(), iterable);
#endif

    public TextBuilder Delimit<R, V>(Action<TextBuilder>? delimit, Iterable<R, V> iterable)
        where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        V? value;
        var iterator = iterable.Iterator;
        if (iterator.TryMoveNext(out value))
        {
            Write<V>(value);
            while (iterator.TryMoveNext(out value))
            {
                delimit?.Invoke(this);
                Write<V>(value);
            }
        }

        return this;
    }

#endregion Values: Iterable

#endregion Values: Append

#region Values: Format

#region Values: ReadOnlySpan<T>

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values,
        string? format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Write<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Write<T>(values[i], format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values,
        string? format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Append<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Write<T>(values[i], format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values,
        string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values,
        string? format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Write<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                delimit?.Invoke(this);
                Write<T>(values[i], format, provider);
            }
        }

        return this;
    }

#endregion Values: ReadOnlySpan<T>

#region Values: T[]

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

#endregion Values: T[]

#region Values: IEnumerable<T>

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values, string? format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Write<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Write<T>(e.Current, format, provider);
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
                Write<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Write<T>(e.Current, format, provider);
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
                Write<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                delimit?.Invoke(this);
                Write<T>(e.Current, format, provider);
            }
        }

        return this;
    }

#endregion Values: IEnumerable<T>

#region Values: Iterable

    public TextBuilder Delimit<R, V>(char delimiter, Iterable<R, V> iterable, string? format, IFormatProvider? provider = null)
        where R : struct, IIterator<V>
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Write<V>(value, format, provider);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Write<V>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<R, V>(scoped text delimiter, Iterable<R, V> iterable, string? format,
        IFormatProvider? provider = null)
        where R : struct, IIterator<V>
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Write<V>(value, format, provider);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Write<V>(value, format, provider);
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
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Write<V>(value, format, provider);
            while (iterator.TryMoveNext(out value))
            {
                delimit?.Invoke(this);
                Write<V>(value, format, provider);
            }
        }

        return this;
    }

#endregion Values: Iterable<T>

#endregion Values: Format

#region Values: Build

#region Values: ReadOnlySpan<T>

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        if (!values.IsEmpty && build is not null)
        {
            Invoke(values[0], build);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Invoke(values[i], build);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        if (!values.IsEmpty && build is not null)
        {
            Invoke(values[0], build);
            for (int i = 1; i < values.Length; i++)
            {
                Write(delimiter);
                Invoke(values[i], build);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter.AsSpan(), values, build);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        if (!values.IsEmpty && build is not null)
        {
            Invoke(values[0], build);
            for (int i = 1; i < values.Length; i++)
            {
                Invoke(delimit).Invoke(values[i], build);
            }
        }

        return this;
    }

#endregion Values: ReadOnlySpan<T>

#region Values: T[]

    public TextBuilder Delimit<T>(char delimiter, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter, values.AsSpan(), build);

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter, values.AsSpan(), build);

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter.AsSpan(), values.AsSpan(), build);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimit, values.AsSpan(), build);

#endregion Values: T[]

#region Values: IEnumerable<T>

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        if (values is not null && build is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Invoke(e.Current, build);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Invoke(e.Current, build);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Invoke(e.Current, build);
            }

            while (e.MoveNext())
            {
                Write(delimiter);
                Invoke(e.Current, build);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter.AsSpan(), values, build);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Invoke(e.Current, build);
                while (e.MoveNext())
                {
                    Invoke(delimit).Invoke(e.Current, build);
                }
            }
        }

        return this;
    }

#endregion Values: IEnumerable<T>

#region Values: Iterable<T>

    public TextBuilder Delimit<R, V>(char delimiter, Iterable<R, V> iterable, Action<TextBuilder, V>? build)
        where R : struct, IIterator<V>
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Invoke(value, build);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Invoke(value, build);
            }
        }

        return this;
    }

    public TextBuilder Delimit<R, V>(scoped text delimiter, Iterable<R, V> iterable, Action<TextBuilder, V>? build)
        where R : struct, IIterator<V>
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Invoke(value, build);
            while (iterator.TryMoveNext(out value))
            {
                Write(delimiter);
                Invoke(value, build);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<R, V>(string? delimiter, Iterable<R, V> iterable, Action<TextBuilder, V>? build)
        where R : struct, IIterator<V>
        => Delimit<R, V>(delimiter.AsSpan(), iterable, build);
#endif

    public TextBuilder Delimit<R, V>(Action<TextBuilder>? delimit, Iterable<R, V> iterable, Action<TextBuilder, V>? build)
        where R : struct, IIterator<V>
    {
        V? value;
        var iterator = iterable.Iterator;

        if (iterator.TryMoveNext(out value))
        {
            Invoke(value, build);
            while (iterator.TryMoveNext(out value))
            {
                Invoke(delimit).Invoke(value, build);
            }
        }

        return this;
    }

#endregion Values: Iterable<T>

#endregion Values: Build

#endregion Delimit
}