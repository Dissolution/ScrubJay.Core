namespace ScrubJay.Rendering;

internal sealed class GenericTypeDefinitionEqualityComparer : EqualityComparer<Type>
{
    public override bool Equals(Type? x, Type? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;
        Debug.Assert(x.IsGenericType);
        Debug.Assert(y.IsGenericType);
        return x.GetGenericTypeDefinition() == y.GetGenericTypeDefinition();
    }

    public override int GetHashCode(Type type)
    {
        Debug.Assert(type.IsGenericType);
        return type.GetGenericTypeDefinition().GetHashCode();
    }
}