// ReSharper disable FieldCanBeMadeReadOnly.Local
#pragma warning disable IDE0044

namespace ScrubJay.Collections.NonGeneric;

[PublicAPI]
[MustDisposeResource(false)]
public sealed class ArrayIndicesEnumerator : IEnumerator<int[]>
{
    public static ArrayIndicesEnumerator For(Array array)
    {
        Throw.IfNull(array);

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

        return new(lowerBounds, upperBounds);
    }

    /// <summary>
    /// Create <see cref="ArrayIndicesEnumerator"/> that iterate over all valid indices in a range of bounds
    /// </summary>
    /// <param name="lowerBounds">
    /// The lower inclusive bounds of each Dimension
    /// </param>
    /// <param name="upperBounds">
    /// The upper <b>inclusive</b> bounds of each Dimension
    /// </param>
    /// <returns>
    /// <see cref="ArrayIndicesEnumerator"/> that iterate over all valid indices in the bounds
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="lowerBounds"/> or <paramref name="upperBounds"/> is invalid
    /// </exception>
    public static ArrayIndicesEnumerator Range(int[] lowerBounds, int[] upperBounds)
    {
        Throw.IfNull(lowerBounds);
        Throw.IfNull(upperBounds);
        int dimensions = lowerBounds.Length;
        if (dimensions == 0)
            throw new ArgumentException("Lower Bounds must have at least one dimension", nameof(lowerBounds));
        if (upperBounds.Length != dimensions)
            throw new ArgumentException("Upper Bounds must have the same dimensions as lower bounds", nameof(upperBounds));
        for (int d = 0; d < dimensions; d++)
        {
            int lower = lowerBounds[d];
            if (lower == int.MinValue)
                throw new ArgumentException($"Dimension {d} has an unsupported lower bound of int.MinValue ({int.MinValue})", nameof(lowerBounds));
            int upper = upperBounds[d];
            if (upper < lower)
                throw new ArgumentException($"Dimension {d} has an upper bound {upper} lower than its lower bound {lower}", nameof(upperBounds));
        }

        return new(lowerBounds, upperBounds);
    }

    public static ArrayIndicesEnumerator FromLengths(params int[] dimensionLengths)
    {
        Throw.IfNull(dimensionLengths);
        int dimensions = dimensionLengths.Length;
        if (dimensions == 0)
            throw new ArgumentException("Lengths must include at least one dimension", nameof(dimensionLengths));

        int[] lowerBounds = new int[dimensions];
        int[] upperBounds = new int[dimensions];
        for (int d = 0; d < dimensions; d++)
        {
            lowerBounds[d] = 0;
            int length = dimensionLengths[0];
            if (length <= 0)
                throw new ArgumentException($"Dimension {d} has an unsupported length zero or less", nameof(dimensionLengths));
            upperBounds[d] = length - 1;
        }

        return new(lowerBounds, upperBounds);
    }


    private readonly int[] _lowerBounds;
    private readonly int[] _upperBounds;
    private int[] _indices;

    object IEnumerator.Current => _indices;
    public int[] Current => _indices;

    public int Dimensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _indices.Length;
    }

    internal ArrayIndicesEnumerator(int[] lowerBounds, int[] upperBounds)
    {
        _lowerBounds = lowerBounds;
        _upperBounds = upperBounds;
        _indices = new int[lowerBounds.Length];
        Reset();
    }

    void IDisposable.Dispose()
    {
        /* Do nothing */
    }

    /// <summary>
    /// Try to increment <see cref="_indices"/> by rolling forward exactly one item
    /// </summary>
    /// <returns>
    /// <c>true</c> if <see cref="_indices"/> was incremented by one<br/>
    /// <c>false</c> if it was not
    /// </returns>
    /// <remarks>
    /// <para>
    /// <b>1D</b> example:<br/>
    /// T[4] = MultiIndex([0],[4]) = [-1]<br/>
    /// Can increment 4 times, producing <c>[0], [1], [2], [3]</c>
    /// </para>
    /// <para>
    /// <b>2D</b> example:<br/>
    /// T[2,3] = MultiIndex([0,2],[0,3]) = [0, -1]<br/>
    /// Can increment 6 times, producing <c>[0,0], [0,1], [0,2], [1,0], [1,1], [1,2]</c>
    /// </para>
    /// </remarks>
    private bool TryIncrement()
    {
        int[] indices = _indices;
        int[] lowerBounds = _lowerBounds;
        int[] upperBounds = _upperBounds;

        // rightmost index to leftmost
        int endDimension = Dimensions - 1;

        for (int d = endDimension; d >= 0; d--)
        {
            int index = indices[d];

            // Can we increment this index?
            if (index < upperBounds[d])
            {
                // Increment
                indices[d] = index + 1;

                // if we can, reset the index to the right to its lowest setting
                if (d < endDimension)
                {
                    indices[d + 1] = lowerBounds[d + 1];
                }

                return true;
            }

            // Try to increment the next dimension
        }

        // Could not increment any dimension, we're at the end
        return false;
    }

    public bool MoveNext() => TryIncrement();

    public Result<int[]> TryMoveNext()
    {
        if (TryIncrement())
        {
            return Ok(_indices);
        }
        else
        {
            return new InvalidOperationException();
        }
    }

    public void Reset()
    {
        int[] indices = _indices;
        int[] lowerBounds = _lowerBounds;
        int d = Dimensions - 1;

        // rightmost / last index is one lower than minimum (un-moved, as per IEnumerator)
        indices[d] = lowerBounds[d] - 1;
        // the remaining are at their lowest bound
        d--;
        while (d >= 0)
        {
            indices[d] = lowerBounds[d];
            d--;
        }
    }

    public ArrayIndicesEnumerator GetEnumerator() => this;
}
