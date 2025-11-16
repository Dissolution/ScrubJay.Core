namespace ScrubJay.Iteration;

[PublicAPI]
public readonly ref struct Iterable<R, V>
    where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    public readonly R Iterator;

    public Iterable(R iterator)
    {
        this.Iterator = iterator;
    }
}

[PublicAPI]
public struct ArrayIterator : IIterator<object?>
{
    private readonly Array _array;
    private int _index;

    public ArrayIterator(Array? array)
    {
        _array = array ?? Array.Empty<object?>();
    }

    public bool TryMoveNext(out object? value)
    {
        if (_index < _array.Length)
        {
            value = _array.GetValue(_index);
            _index++;
            return true;
        }

        value = null;
        return false;
    }
}

[PublicAPI]
public struct Generic2DArrayIterator<T> : IIterator<T>
{
    private readonly T[] _array;
    private int _index;

    public Generic2DArrayIterator(T[]? array)
    {
        _array = array ?? [];
    }

    public bool TryMoveNext([MaybeNullWhen(false)] out T value)
    {
        if (_index < _array.Length)
        {
            value = _array[_index];
            _index++;
            return true;
        }

        value = default;
        return false;
    }
}

[PublicAPI]
public struct TupleIterator<T> : IIterator<object?>
    where T : ITuple
{
    private readonly T _tuple;
    private int _index;

    public TupleIterator(T tuple)
    {
        _tuple = tuple;
    }

    public bool TryMoveNext(out object? value)
    {
        if (_index < _tuple.Length)
        {
            value = _tuple[_index];
            _index++;
            return true;
        }

        value = null;
        return false;
    }
}

[PublicAPI]
public interface IIterator<T>
{
    bool TryMoveNext([MaybeNullWhen(false)] out T value);
}

public readonly struct EmptyIterator<T> : IIterator<T>, IHasDefault<EmptyIterator<T>>
{
    public static EmptyIterator<T> Default { get; } = new();

    public bool TryMoveNext([MaybeNullWhen(false)] out T value)
    {
        value = default;
        return false;
    }
}

public static class Iterable
{
    public static Iterable<EmptyIterator<T>, T> Empty<T>() => new(EmptyIterator<T>.Default);
}

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

    extension<T>(T tuple)
        where T : ITuple
    {
        public Iterable<TupleIterator<T>, object?> AsIterable()
        {
            return new(new(tuple));
        }
    }
}