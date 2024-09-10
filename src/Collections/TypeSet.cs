namespace ScrubJay.Collections;

/// <summary>
/// A <see cref="HashSet{T}"/> containing <see cref="Type"/> values
/// </summary>
public class TypeSet : HashSet<Type>
{
    public bool Add<T>() => base.Add(typeof(T));

    public bool Remove<T>() => base.Remove(typeof(T));
    
    public bool Contains<T>() => base.Contains(typeof(T));
}