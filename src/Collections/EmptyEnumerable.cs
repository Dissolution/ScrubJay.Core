namespace ScrubJay.Collections;

public sealed class EmptyEnumerable<T> : IEnumerable<T>
{
    public static readonly EmptyEnumerable<T> Instance = new();

    IEnumerator IEnumerable.GetEnumerator() => EmptyEnumerator<T>.Instance;

    IEnumerator<T> IEnumerable<T>.GetEnumerator()=> EmptyEnumerator<T>.Instance;
    
    public EmptyEnumerator<T> GetEnumerator() => EmptyEnumerator<T>.Instance;
}