using InvalidOperationException = System.InvalidOperationException;

namespace ScrubJay.Collections;

/// <summary>
/// A typed <see cref="IEnumerator{T}"/> over an <see cref="Array"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class ArrayEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    private readonly T[] _array;
    private readonly int _version;
    private readonly Func<int>? _getCurrentVersion;
    private readonly int _minIndex;
    private readonly int _maxIndex;
    private readonly int _step;

    private int _index;

    object? IEnumerator.Current => Current;

    public T Current => TryGetCurrent().OkOrThrow();

    public Result<T, Exception> TryGetCurrent()
    {
        if (_getCurrentVersion is not null && _getCurrentVersion() != _version)
            return new InvalidOperationException("Source array has changed");

        if (_step == +1)
        {
            if (_index < _minIndex)
                return new InvalidOperationException("Enumeration has not yet started");
            if (_index > _maxIndex)
                return new InvalidOperationException("Enumeration has finished");
        }
        else
        {
            if (_index > _maxIndex)
                return new InvalidOperationException("Enumeration has not yet started");
            if (_index < _minIndex)
                return new InvalidOperationException("Enumeration has finished");
        }
        Debug.Assert((uint)_index < (uint)_array.Length);
        return _array[_index];
    }

    public ArrayEnumerator(T[] array) : this(array, step: +1) { }

    public ArrayEnumerator(T[] array, int step)
    {
        _array = array;
        _minIndex = 0;
        _maxIndex = array.Length - 1;
        _step = step;
        _index = step switch
        {
            +1 => _minIndex - 1,
            -1 => _maxIndex + 1,
            _ => throw new ArgumentOutOfRangeException(nameof(step), step, "Step must be +1 or -1")
        };
    }

    public ArrayEnumerator(T[] array, Range arrayRange, int step)
    {
        _array = array.ThrowIfNull();
        int arrLen = array.Length;
        (int offset, int length) = Validate.Range(arrayRange, arrLen).OkOrThrow();
        _minIndex = offset;
        _maxIndex = offset + (length - 1);
        _step = step;
        _index = step switch
        {
            +1 => _minIndex - 1,
            -1 => _maxIndex + 1,
            _ => throw new ArgumentOutOfRangeException(nameof(step), step, "Step must be +1 or -1"),
        };
    }

    public ArrayEnumerator(T[] array, Index minArrayIndex, Index maxArrayIndex, int step)
    {
        _array = array.ThrowIfNull();
        int arrLen = array.Length;
        _minIndex = Validate.Index(minArrayIndex, arrLen).OkOrThrow();
        _maxIndex = Validate.Index(maxArrayIndex, arrLen).OkOrThrow();
        if (_maxIndex < _minIndex)
            throw new ArgumentOutOfRangeException(nameof(maxArrayIndex), maxArrayIndex, "Max Array Index must be greater than Min Array Index");
        _step = step;
        _index = step switch
        {
            +1 => _minIndex - 1,
            -1 => _maxIndex + 1,
            _ => throw new ArgumentOutOfRangeException(nameof(step), step, "Step must be +1 or -1"),
        };
    }

    internal ArrayEnumerator(
        T[] array,
        int minArrayIndex, int maxArrayIndex,
        int step,
        Func<int> getCurrentVersion)
    {
        _array = array.ThrowIfNull();
        int arrLen = array.Length;
        _minIndex = Validate.Index(minArrayIndex, arrLen).OkOrThrow();
        _maxIndex = Validate.Index(maxArrayIndex, arrLen).OkOrThrow();
        if (_maxIndex < _minIndex)
            throw new ArgumentOutOfRangeException(nameof(maxArrayIndex), maxArrayIndex, "Max Array Index must be greater than Min Array Index");
        _step = step;
        _index = step switch
        {
            +1 => _minIndex - 1,
            -1 => _maxIndex + 1,
            _ => throw new ArgumentOutOfRangeException(nameof(step), step, "Step must be +1 or -1"),
        };

        _version = getCurrentVersion();
        _getCurrentVersion = getCurrentVersion;
    }


    public bool MoveNext()
    {
        if (_getCurrentVersion is not null)
            Throw.IfEnumerationSourceHasChanged(_getCurrentVersion() != _version);

        int newIndex = _index + _step;
        if (newIndex < _minIndex || newIndex > _maxIndex)
            return false;
        _index = newIndex;
        return true;
    }

    public Option<T> TryMoveNext()
    {
        if (_getCurrentVersion is not null && _getCurrentVersion() != _version)
            return None<T>();

        int newIndex = _index + 1;
        if (newIndex < _minIndex || newIndex > _maxIndex)
            return None<T>();
        _index = newIndex;
        return Some(_array[newIndex]);
    }

    public void Reset()
    {
        if (_getCurrentVersion is not null)
            Throw.IfEnumerationSourceHasChanged(_getCurrentVersion() != _version);

        if (_step == +1)
        {
            _index = _minIndex - 1;
        }
        else
        {
            Debug.Assert(_step == -1);
            _index = _maxIndex + 1;
        }
    }

    void IDisposable.Dispose()
    {
        // do nothing
    }
}
