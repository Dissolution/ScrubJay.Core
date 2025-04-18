namespace ScrubJay.Building;

/// <summary>
/// Indicates that this is an implementation of the Builder Pattern
/// </summary>
/// <typeparam name="S">
/// The <see cref="Type"/> of the implementation (<b>S</b>elf)
/// </typeparam>
/// <see href="https://en.wikipedia.org/wiki/Builder_pattern"/>
public interface IBuilder<S>
    where S : IBuilder<S>
{
    /// <summary>
    /// Invoke an <see cref="Action{S}"/> on this <typeparamref name="S"/> instance and then returns this instance
    /// </summary>
    S Invoke(Action<S>? instanceAction);

    /// <summary>
    /// Invoke a fluent <see cref="Func{S,S}"/> on this <typeparamref name="S"/> instance and then returns this instance
    /// </summary>
    /// <remarks>
    /// </remarks>
    S Invoke(Func<S, S>? instanceFluentFunc);
}