// Identifiers should not match keywords
#pragma warning disable CA1716

namespace ScrubJay.Collections.Pooling;

/// <summary>
/// A pool of reusable <typeparamref name="T"/> instances
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of instances stored in this <see cref="IInstancePool{T}"/><br/>
/// This is restricted to <c>class</c> types
/// </typeparam>
/// <remarks>
/// - The primary purpose of an <see cref="IInstancePool{T}"/> is to store <typeparamref name="T"/> instances that can be
/// <see cref="Rent">Rented</see> and <see cref="Return">Returned</see><br/>
/// - The typical process of constructing instances and letting them be garbage collected can cause GC churn and excess memory use,
/// whereas an <see cref="IInstancePool{T}"/> allows those instances to be reused<br/>
/// <b>Note:</b><br/>
/// - Not all returned instances will be kept<br/>
///   - The pool is not meant for <i>storage</i> (short nor long)<br/>
///   - If there is no space in the pool, extra returned instances will be discarded<br/>
/// - If an instance is <see cref="Rent">Rented</see> from a pool, the caller will <see cref="Return"/> it back in a relatively short time<br/>
///   - Keeping rented instances for long durations is <i>ok</i>, but it reduces the usefulness of pooling<br/>
///   - Not returning instances to the pool in not detrimental to its operations, but is a bad practice<br/>
///   - If you do not intend on returning instances, do not use a <see cref="IInstancePool{T}"/><br/>
/// </remarks>
[PublicAPI]
public interface IInstancePool<T>
    where T : class
{
    /// <summary>
    /// Rent a <typeparamref name="T"/> instance from this pool
    /// </summary>
    T Rent();

    /// <summary>
    /// Return a <typeparamref name="T"/> instance to this pool
    /// </summary>
    /// <param name="instance">
    /// The Instance that will be returned to this pool<br/>
    /// It should not be referenced by the caller again
    /// </param>
    void Return(T? instance);
}
