using System.Collections.Concurrent;
using ScrubJay.Collections;

namespace ScrubJay.Concurrency;

public class ConcurrentHashSet<T> :
    ConcurrentDictionary<T, Nothing>,
    IQuasiSet<T>,
    ICollection<T>, IReadOnlyCollection<T>,
    IEnumerable<T>
    where T : notnull
{
    bool ICollection<T>.IsReadOnly => false;

#if NET6_0_OR_GREATER
    public IEqualityComparer<T> EqualityComparer => base.Comparer;
#else
    public IEqualityComparer<T> EqualityComparer { get; }
#endif

    public ConcurrentHashSet()
    {
#if !NET6_0_OR_GREATER
        this.EqualityComparer = EqualityComparer<T>.Default;
#endif
    }

    public ConcurrentHashSet(IEqualityComparer<T>? comparer)
        : base(comparer ?? EqualityComparer<T>.Default)
    {
#if !NET6_0_OR_GREATER
        this.EqualityComparer = comparer ?? EqualityComparer<T>.Default;
#endif
    }

    public bool TryAdd(T item) => base.TryAdd(item, default);

    void ICollection<T>.Add(T item)
    {
        if (base.TryAdd(item, default))
            throw new ArgumentException($"Cannot add {item}: Already exists", nameof(item));
    }

    public bool Contains(T item) => base.ContainsKey(item);

    public bool TryRemove(T item) => base.TryRemove(item, out _);

    bool ICollection<T>.Remove(T item) => base.TryRemove(item, out _);

    public void CopyTo(T[] array, int arrayIndex = default)
    {
        Throw.CanCopyTo(base.Count, array, arrayIndex);
        base.Keys.CopyTo(array, arrayIndex);
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.GetEnumerator();

    public new IEnumerator<T> GetEnumerator() => base.Keys.GetEnumerator();
}