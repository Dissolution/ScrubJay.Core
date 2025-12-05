namespace ScrubJay.Iteration;

[PublicAPI]
public static class IteratorExtensions
{
    extension<R, V>(R iterator)
        where R : struct, IIterator<V>
    {
        public Option<V> MoveNext()
        {
            return iterator.TryMoveNext(out var value) ? Some<V>(value) : None;
        }
    }

    extension(Array? array)
    {
        public Iterable<ArrayIterator, object?> AsIterable()
        {
            return new(new(array));
        }
    }

    extension<T>(T[]? array)
    {
        public Iterable<Generic2DArrayIterator<T>, T> AsIterable()
        {
            return new(new(array));
        }
    }

#if NET9_0_OR_GREATER
    extension<T>(ReadOnlySpan<T> span)
    {
        public Iterable<ReadOnlySpanIterator<T>, T> AsIterable()
        {
            return new(new(span));
        }
    }

    extension<T>(Span<T> span)
    {
        public Iterable<SpanIterator<T>, T> AsIterable()
        {
            return new(new(span));
        }
    }
#endif

    extension<T>(T tuple)
        where T : ITuple
    {
        public Iterable<TupleIterator<T>, object?> AsIterable()
        {
            return new(new(tuple));
        }
    }
}