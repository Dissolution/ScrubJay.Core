using ScrubJay.Iteration;

namespace ScrubJay.Text;

partial class TextBuilder
{
#region Action: Invoke

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

#endregion /Values: ReadOnlySpan<T>

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

#endregion /Values: T[]

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

#endregion /Values: IEnumerable<T>

#region Values: Iterable<T>

    public TextBuilder Delimit<R, V>(char delimiter, Iterable<R, V> iterable, Action<TextBuilder, V>? build)
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
#if NET9_0_OR_GREATER
        , allows ref struct
        where V : allows ref struct
#endif
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
#if NET9_0_OR_GREATER
        , allows ref struct
        where V : allows ref struct
#endif
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

#endregion /Values: Iterable<T>

#region Values: SpanSplitIterator

    public TextBuilder Delimit<T>(scoped txt delimiter, SpanSplitIterator<T> iterator, Action<TextBuilder, Segment<T>>? build)
        where T : IEquatable<T>
    {
        if (build is not null)
        {
            if (iterator.TryMoveNext(out var segment))
            {
                build(this, segment);
                while (iterator.TryMoveNext(out segment))
                {
                    Write(delimiter);
                    build(this, segment);
                }
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, SpanSplitIterator<T> iterator,
        Action<TextBuilder, Segment<T>>? build)
        where T : IEquatable<T>
    {
        if (build is not null)
        {
            if (iterator.TryMoveNext(out var segment))
            {
                build(this, segment);
                while (iterator.TryMoveNext(out segment))
                {
                    Invoke(delimit);
                    build(this, segment);
                }
            }
        }

        return this;
    }

#endregion /Values: SpanSplitIterator

#region Values: SpanSplitEqualityIterator

    public TextBuilder Delimit<T>(scoped txt delimiter, SpanSplitEqualityIterator<T> iterator,
        Action<TextBuilder, Segment<T>>? build)
    {
        if (build is not null)
        {
            if (iterator.TryMoveNext(out var segment))
            {
                build(this, segment);
                while (iterator.TryMoveNext(out segment))
                {
                    Write(delimiter);
                    build(this, segment);
                }
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, SpanSplitEqualityIterator<T> iterator,
        Action<TextBuilder, Segment<T>>? build)
    {
        if (build is not null)
        {
            if (iterator.TryMoveNext(out var segment))
            {
                build(this, segment);
                while (iterator.TryMoveNext(out segment))
                {
                    Invoke(delimit);
                    build(this, segment);
                }
            }
        }

        return this;
    }

#endregion /Values: SpanSplitEqualityIterator


#region Values: TextSplitIterator

    public TextBuilder Delimit(scoped txt delimiter, TextSplitIterator iterator, Action<TextBuilder, Segment<char>>? build)
    {
        if (build is not null)
        {
            if (iterator.TryMoveNext(out var segment))
            {
                build(this, segment);
                while (iterator.TryMoveNext(out segment))
                {
                    Write(delimiter);
                    build(this, segment);
                }
            }
        }

        return this;
    }

    public TextBuilder Delimit(Action<TextBuilder>? delimit, TextSplitIterator iterator,
        Action<TextBuilder, Segment<char>>? build)
    {
        if (build is not null)
        {
            if (iterator.TryMoveNext(out var segment))
            {
                build(this, segment);
                while (iterator.TryMoveNext(out segment))
                {
                    Invoke(delimit);
                    build(this, segment);
                }
            }
        }

        return this;
    }

#endregion /Values: TextSplitIterator

#endregion /Action: Invoke
}