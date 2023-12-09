namespace ScrubJay.Debugging;

/// <summary>
/// Utility for assigning unique identifiers to every instance of a <c>class</c>
/// </summary>
public static class InstanceId
{
    /// <summary>
    /// Get this <paramref name="instance"/>'s unique identifier
    /// </summary>
    /// <param name="instance">
    /// The <c>class</c> instance to get the unique identifier for
    /// </param>
    /// <typeparam name="TInstance">
    /// The <see cref="Type"/> of <c>class</c> this <paramref name="instance"/> is
    /// </typeparam>
    /// <returns>
    /// A <c>long</c> identifier unique to this <paramref name="instance"/>
    /// </returns>
    public static long GetInstanceId<TInstance>(this TInstance? instance)
        where TInstance : class
        => InstanceId<TInstance>.GetInstanceId(instance);
}

/// <summary>
/// Static storage for instance counter per Type
/// </summary>
/// <typeparam name="T"></typeparam>
internal static class InstanceId<T>
    where T : class
{
    /// <summary>
    /// A weakly-referenced property on T instances
    /// </summary>
    /// <remarks>
    /// We have to store a <c>byte[]</c> and not a <c>long</c>
    /// directly because of <see cref="ConditionalWeakTable{TKey,TValue}"/>'s requirements 
    /// </remarks>
    private static readonly ConditionalWeakTable<T, byte[]> _instanceIds = new();
    
    // ReSharper disable once StaticMemberInGenericType
    // I want there to be a separate _instanceCount field for every T
    private static long _instanceCount = 0L;

    /// <summary>
    /// Gets the unique identifier for an <paramref name="instance"/>
    /// </summary>
    public static long GetInstanceId(T? instance)
    {
        // Null instances are all the same
        if (instance is null) return 0L;
        
        // Check if we have already assigned an identifier
        if (_instanceIds.TryGetValue(instance, out byte[]? bytes))
        {
            return BitConverter.ToInt64(bytes, 0);
        }
        
        // Get the next id (this will start at 1)
        long id = Interlocked.Increment(ref _instanceCount);
        // Store this identifier for faster lookup
        bytes = BitConverter.GetBytes(id);
        _instanceIds.Add(instance, bytes);
        return id;
    }
}