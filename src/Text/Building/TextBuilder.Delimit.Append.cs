using ScrubJay.Iteration;

namespace ScrubJay.Text;

partial class TextBuilder
{
#region Action: Append

#region Values: ReadOnlySpan<T>

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            Append<T>(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                Append(delimiter).Append<T>(values[i]);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            Append<T>(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                Append(delimiter).Append<T>(values[i]);
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
            Append<T>(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                Invoke(delimit).Append<T>(values[i]);
            }
        }

        return this;
    }

#endregion /Values: ReadOnlySpan<T>

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

#endregion /Values: T[]

#region Values: IEnumerable<T>

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Append<T>(e.Current);
            }

            while (e.MoveNext())
            {
                Append(delimiter).Append<T>(e.Current);
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
                Append<T>(e.Current);
            }

            while (e.MoveNext())
            {
                Append(delimiter).Append<T>(e.Current);
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
                Append<T>(e.Current);
                while (e.MoveNext())
                {
                    Invoke(delimit).Append<T>(e.Current);
                }
            }
        }

        return this;
    }

#endregion /Values: IEnumerable<T>

#region Values: Iterable

    public TextBuilder Delimit<R, V>(char delimiter, Iterable<R, V> iterable)
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
            Append<V>(value);
            while (iterator.TryMoveNext(out value))
            {
                Append(delimiter).Append<V>(value);
            }
        }

        return this;
    }

    public TextBuilder Delimit<R, V>(scoped text delimiter, Iterable<R, V> iterable)
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
            Append<V>(value);
            while (iterator.TryMoveNext(out value))
            {
                Append(delimiter).Append<V>(value);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<R, V>(string? delimiter, Iterable<R, V> iterable)
        where R : struct, IIterator<V>
        => Delimit<R, V>(delimiter.AsSpan(), iterable);
#endif

    public TextBuilder Delimit<R, V>(Action<TextBuilder>? delimit, Iterable<R, V> iterable)
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
            Append<V>(value);
            while (iterator.TryMoveNext(out value))
            {
                Invoke(delimit).Append<V>(value);
            }
        }

        return this;
    }

#endregion /Values: Iterable

#endregion /Action: Append
}