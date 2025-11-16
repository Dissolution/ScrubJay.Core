namespace ScrubJay.Rendering;

internal sealed class LooseTypeComparer :
    IEqualityComparer<Type>,
    IHasDefault<LooseTypeComparer>
{
    public static LooseTypeComparer Default { get; } = new();

    public bool Equals(Type? x, Type? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;

        if (x.IsGenericParameter)
        {
            if (!y.IsGenericParameter)
                return false;
            return Sequence.Equal(
                x.GetGenericParameterConstraints(),
                y.GetGenericParameterConstraints(),
                this);
        }

        if (x.IsGenericType)
        {
            if (!y.IsGenericType)
                return false;
            return this.Equals(
                x.GetGenericTypeDefinition(),
                y.GetGenericTypeDefinition());
        }

        return x == y;
    }

    public int GetHashCode(Type type)
    {
        var hasher = new Hasher();

        if (type.IsGenericParameter)
        {
            hasher.Add('T');
            hasher.AddMany(type.GetGenericParameterConstraints(), this);
            return hasher.ToHashCode();
        }

        if (type.IsGenericType)
        {
            hasher.Add('T');
            hasher.Add(type.GetGenericTypeDefinition(), this);
            return hasher.ToHashCode();
        }

        hasher.Add(type);
        return hasher.ToHashCode();
    }
}