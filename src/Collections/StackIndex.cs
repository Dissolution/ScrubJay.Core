namespace ScrubJay.Collections;

/// <summary>
/// Represents an index in a Stack-like collection.
/// </summary>
[PublicAPI]
public readonly struct StackIndex :
#if NET7_0_OR_GREATER
    IEqualityOperators<StackIndex, StackIndex, bool>,
#endif
    IEquatable<StackIndex>
{
    public static implicit operator StackIndex(int index) => new(index, false);
    public static implicit operator StackIndex(Index index) => new(index);

    public static bool operator ==(StackIndex left, StackIndex right) => left.Equals(right);
    public static bool operator !=(StackIndex left, StackIndex right) => !left.Equals(right);

    public static readonly StackIndex Start = new StackIndex(0);
    public static readonly StackIndex End = new StackIndex(~0);

    public static StackIndex FromStart(int offset)
    {
        if (offset < 0)
            throw Ex.ArgRange(offset);
        return new StackIndex(offset);
    }

    public static StackIndex FromEnd(int offset)
    {
        if (offset < 0)
            throw Ex.ArgRange(offset);
        return new StackIndex(~offset);
    }

    private readonly int _index;

    // this instead of _popOrder because we want default(StackIndex) to indicate PopOrder = true!
    private readonly bool _inArrayOrder;

    public int Value
    {
        get
        {
            if (_index < 0)
                return ~_index;
            else
                return _index;
        }
    }

    public bool IsFromEnd => _index < 0;

    public bool InPopOrder => !_inArrayOrder;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private StackIndex(int index)
    {
        _index = index;
        _inArrayOrder = false;
    }

    public StackIndex(int index, bool fromEnd = false)
    {
        if (index < 0)
            throw Ex.ArgRange(index);

        if (fromEnd)
            _index = ~index;
        else
            _index = index;
        _inArrayOrder = false;
    }

    public StackIndex(Index index)
    {
        if (index.IsFromEnd)
        {
            _index = ~index.Value;
        }
        else
        {
            _index = index.Value;
        }

        _inArrayOrder = false;
    }

    public StackIndex(int index, bool fromEnd = false, bool popOrder = true)
    {
        if (index < 0)
            throw Ex.ArgRange(index);

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
        if (IsFromEnd)
        {
            offset += (size + 1);
        }

        if (!_inArrayOrder)
        {
            offset = size - offset;
        }

        return offset;
    }

    public Result<int> TryGetOffset(int size)
    {
        int offset = GetOffset(size);
        if (offset >= 0 && offset < size)
            return offset;
        return Ex.Index(this, size);
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
            .If(_inArrayOrder, "arr:", "pop:")
            .If(IsFromEnd, '^')
            .Append(Value)
            .ToStringAndDispose();
    }
}