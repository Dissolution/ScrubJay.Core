namespace ScrubJay.Extensions;

[PublicAPI]
public static class AttributeExtensions
{
    public static bool Has<A>(this Attribute[]? attributes)
        where A : Attribute
    {
        if (attributes is not null)
        {
            foreach (Attribute attribute in attributes)
            {
                if (attribute is A)
                    return true;
            }
        }

        return false;
    }

    public static bool Has<A>(this Attribute[]? attributes, [NotNullWhen(true)] out A? attr)
        where A : Attribute
    {
        if (attributes is not null)
        {
            foreach (Attribute attribute in attributes)
            {
                if (attribute.Is<A>(out attr))
                    return true;
            }
        }

        attr = null;
        return false;
    }
}