// Identifiers should not match keywords
// - `Return` is a fine method name
#pragma warning disable CA1716

namespace ScrubJay.Pooling;

/// <summary>
/// A pool of reusable <typeparamref name="T"/> instances
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of instances stored in this <see cref="IInstancePool{T}"/><br/>
/// This is restricted to <c>class</c> types
/// </typeparam>
/// <remarks>
/// - The primary purpose of an <see cref="IInstancePool{T}"/> is to store, <see cref="Rent"/>, and <see cref="Return"/> <typeparamref name="T"/> instances,
/// rather than the usual process of <c>new</c> up new instances when required and letting them be garbage collected.<br/>
/// That process will cause GC churn and potential memory use inflation, whereas a Pool re-uses those instances to reduce allocations<br/>
/// <b>Note:</b><br/>
/// - Not all returned instances will be kept<br/>
///   - The pool is not meant for <i>storage</i> (short nor long)<br/>
///   - If there is no space in the pool, extra returned instances will be disposed<br/>
/// - If an instance is <see cref="Rent">Rented</see> from a pool, the caller will <see cref="Return"/> it back in a relatively short time<br/>
///   - Keeping Rented instances for long durations is <i>ok</i>, but it reduces the usefulness of pooling<br/>
///   - Not Returning instances to the pool in not detrimental to its operations, but is a bad practice<br/>
///   - If you do not intend on Returning instances, do not use a <see cref="IInstancePool{T}"/><br/>
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
