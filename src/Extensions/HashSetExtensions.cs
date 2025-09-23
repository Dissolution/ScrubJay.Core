namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="HashSet{T}"/>
/// </summary>
[PublicAPI]
public static class HashSetExtensions
{
    extension<T>(HashSet<T> set)
    {
        public void AddMany(IEnumerable<T>? items)
        {
            if (items is not null)
            {
                set.UnionWith(items);
            }
        }
    }
}