namespace ScrubJay.Collections.Pooling;

public readonly struct StackIndex :
#if NET7_0_OR_GREATER
    IEqualityOperators<StackIndex, StackIndex, bool>,
#endif
    IEquatable<StackIndex>
{
    public static implicit operator StackIndex(int index) => new(index);
    public static implicit operator StackIndex(Index index) => new(index.Value, index.IsFromEnd);

    public static bool operator ==(StackIndex left, StackIndex right) => left.Equals(right);
    public static bool operator !=(StackIndex left, StackIndex right) => !left.Equals(right);

    private readonly int _index;
    private readonly bool
        _inArrayOrder; // this instead of _popOrder because we want default(StackIndex) to indicate PopOrder = true!

    public bool IsFromEnd => _index < 0;
    public bool InPopOrder => !_inArrayOrder;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StackIndex(int index)
    {
        _index = index;
        _inArrayOrder = false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StackIndex(int index, bool fromEnd = false, bool popOrder = true)
    {
        if (fromEnd)
        {
            _index = ~index;
        }
        else
        {
            _index = index;
        }
        _inArrayOrder = !popOrder;
    }

    public int GetOffset(int size)
    {
        int offset = _index;
        if (_index < 0) // from end
        {
            offset += (size + 1);
        }
        if (!_inArrayOrder)
        {
            offset = ((size - offset) - 1);
        }
        return offset;
    }

    public bool Equals(StackIndex other)
    {
        return (_index == other._index) &&
            (_inArrayOrder == other._inArrayOrder);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is StackIndex stackIndex && Equals(stackIndex);

    public override int GetHashCode() => Hasher.HashMany(_index, _inArrayOrder);

    public override string ToString()
    {
        return TextBuilder.New
            .IfAppend(_inArrayOrder, "arr:", "pop:")
            .IfAppend(IsFromEnd, '^')
            .Format(_index)
            .ToStringAndDispose();
    }
}