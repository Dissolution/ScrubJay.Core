// Exception to Identifiers Require Correct Suffix

using ScrubJay.Text.Rendering;

#pragma warning disable CA1710
// CA1043 : Use Integral Or String Argument For Indexers
#pragma warning disable CA1043

namespace ScrubJay.Collections.NonGeneric;

[PublicAPI]
public sealed class ArrayWrapper<T> :
    IReadOnlyCollection<T?>,
    IEnumerable<T?>
{
    private static Result<T?> ParseObject(
        object? obj,
        [CallerArgumentExpression(nameof(obj))]
        string? objName = null)
    {
        if (obj.As<T>(out var value))
            return Ok(value);
        return new ArgumentException($"Object `{obj}` is not a {typeof(T).Render()}", objName);
    }



    private readonly Array _array;
    private readonly int[] _lowerBounds;
    private readonly int[] _upperBounds;

    public T? this[int[] indices]
    {
        get => TryGetValue(indices).OkOrThrow();
        set => TrySetValue(indices, value).OkOrThrow();
    }

    public int Count => _array.Length;


    public ArrayWrapper(Array array)
    {
        _array = array.ThrowIfNull();

        int dimensions = array.Rank;
        int[] lowerBounds = new int[dimensions];
        int[] upperBounds = new int[dimensions];
        for (int d = 0; d < dimensions; d++)
        {
            int lower = array.GetLowerBound(d);
            if (lower == int.MinValue)
                throw new ArgumentException($"Dimension {d} has an unsupported lower bound of int.MinValue ({int.MinValue})", nameof(array));
            lowerBounds[d] = lower;
            int upper = array.GetUpperBound(d);
            if (upper < lower)
                throw new ArgumentException($"Dimension {d} has an upper bound {upper} lower than its lower bound {lower}", nameof(array));
            upperBounds[d] = upper;
        }

        _lowerBounds = lowerBounds;
        _upperBounds = upperBounds;
    }

    private Result<int[]> ValidateIndices(
        int[] indices,
        [CallerArgumentExpression(nameof(indices))]
        string? indicesName = null)
    {
        for (int d = 0; d < _array.Rank; d++)
        {
            int index = indices[d];
            int lower = _lowerBounds[d];
            int upper = _upperBounds[d];
            if ((index < lower) || (index > upper))
                return new ArgumentOutOfRangeException(indicesName, indices, $"Indices[{d}] of {index} was not in [{lower}..{upper}]");
        }
        return Ok(indices);
    }

    public Result<T?> TryGetValue(int[] indices)
    {
        return ValidateIndices(indices)
            .Select(idx => ParseObject(_array.GetValue(idx)));
    }

    public Result<T?> TrySetValue(int[] indices, T? item)
    {
        return ValidateIndices(indices)
            .Select(idx =>
            {
                _array.SetValue(item, idx);
                return item;
            });
    }

    public bool Contains(T? element) => IndexOf(element).IsSome();

    public Option<int[]> IndexOf(T? element, IEqualityComparer<T>? elementComparer = null)
    {
        var comparer = elementComparer ?? EqualityComparer<T>.Default;
        var indices = new ArrayIndicesEnumerator(_lowerBounds, _upperBounds);
        foreach (int[] index in indices)
        {
            var arrayElement = ParseObject(_array.GetValue(index)).OkOrThrow();
            if (comparer.Equals(element!, arrayElement!))
                return Some(index);
        }
        return None<int[]>();
    }

    public void Clear() => Array.Clear(_array, 0, Count);

    public bool TryCopyTo(Span<T?> destination)
    {
        if (!Validate.CanCopyTo(destination, Count))
            return false;

        int d = 0;
        var indices = new ArrayIndicesEnumerator(_lowerBounds, _upperBounds);
        foreach (int[] index in indices)
        {
            var arrayElement = ParseObject(_array.GetValue(index)).OkOrThrow();
            destination[d++] = arrayElement;
        }
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T?> IEnumerable<T?>.GetEnumerator() => GetEnumerator();
    public ArrayWrapperEnumerator GetEnumerator() => new(this);

    // Add readonly
    // _indices is used in Span<> and needs to be writable
#pragma warning disable IDE0044
    [MustDisposeResource(false)]
    public sealed class ArrayWrapperEnumerator : IEnumerator<T?>, IEnumerator, IDisposable
    {
        private readonly Array _array;

#pragma warning disable CA2213
        private readonly ArrayIndicesEnumerator _arrayIndicesEnumerator;
#pragma warning restore CA2213

        object? IEnumerator.Current => Current;

        public T? Current
        {
            get
            {
                int[] indices = _arrayIndicesEnumerator.Current;
                object? value = _array.GetValue(indices);
                return ParseObject(value).OkOrThrow();
            }
        }

        public ArrayWrapperEnumerator(ArrayWrapper<T> wrapper)
        {
            _array = wrapper._array;
            _arrayIndicesEnumerator = new(wrapper._lowerBounds, wrapper._upperBounds);
            Reset();
        }

        bool IEnumerator.MoveNext() => _arrayIndicesEnumerator.TryMoveNext();

        void IDisposable.Dispose()
        {
            // _arrayIndicesEnumerator does not need to be disposed
            //((IDisposable)_arrayIndicesEnumerator).Dispose();
        }

        public Result<T?> TryMoveNext()
        {
            if (_arrayIndicesEnumerator.TryMoveNext())
            {
                return Ok(Current);
            }
            return new InvalidOperationException("There are no more elements in this array");
        }

        public void Reset() => _arrayIndicesEnumerator.Reset();
    }


}
