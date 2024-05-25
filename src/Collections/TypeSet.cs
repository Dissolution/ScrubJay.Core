namespace ScrubJay.Collections;

public class TypeSet : HashSet<Type>
{
    public TypeSet() : base() { }

    public bool Add<T>() => base.Add(typeof(T));

    public bool Remove<T>() => base.Remove(typeof(T));
    
    public bool Contains<T>() => base.Contains(typeof(T));
}