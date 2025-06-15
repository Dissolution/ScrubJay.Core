namespace ScrubJay.Extensions;

[PublicAPI]
public static class AttributeExtensions
{
    public static Option<A> Has<A>(this Attribute[]? attributes)
        where A : Attribute
    {
        if (attributes is not null)
        {
            foreach (var attr in attributes)
            {
                if (attr is A attribute)
                    return Some(attribute);
            }
        }
        return None();
    }
}