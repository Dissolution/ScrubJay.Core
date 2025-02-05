#pragma warning disable CA1716

namespace ScrubJay.Buffers;

/// <summary>
/// A pool of <typeparamref name="T"/> instances
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of <c>class</c> instances stored in this <see cref="IObjectPool{T}"/>
/// </typeparam>
/// <remarks>
/// - The main purpose of an <see cref="IObjectPool{T}"/> is to help re-use a limited number of
/// <typeparamref name="T"/> instances rather than continuously <c>new</c>-ing them up.<br/>
/// - It is not the goal to keep all returned instances.<br/>
///   - The pool is not meant for storage (short nor long).<br/>
///   - If there is no space in the pool, extra returned instances will be disposed.<br/>
/// - It is implied that if an instance is obtained from a pool, the caller will return it back in a relatively short time.<br/>
///   - Keeping checked out instances for long durations is _ok_, but it reduces the usefulness of pooling.<br/>
///   - Not returning instances to the pool in not detrimental to its work, but is a bad practice.<br/>
///   - If there is no intent to return or re-use the instance, do not use a pool.<br/>
/// - When this pool is Disposed, all instances will also be disposed.<br/>
///   - Any further returned instances will be cleaned, disposed, and discarded.<br/>
/// </remarks>
/// <seealso href="https://github.dev/dotnet/aspnetcore/blob/main/src/ObjectPool/src/DefaultObjectPool.cs"/>
[PublicAPI]
public interface IObjectPool<T>
    where T : class
{
    /// <summary>
    /// Rent a <typeparamref name="T"/> instance from this pool
    /// </summary>
    /// <remarks>
    /// <see cref="Return"/> the instance to this pool when you are finished with it
    /// </remarks>
    T Rent();

    /// <summary>
    /// Returns a <typeparamref name="T"/> instance to this pool
    /// </summary>
    /// <param name="instance">
    /// The <typeparamref name="T"/> instance that will be returned to the pool<br/>
    /// It should not be used again after calling this method
    /// </param>
    void Return(T? instance);
}
