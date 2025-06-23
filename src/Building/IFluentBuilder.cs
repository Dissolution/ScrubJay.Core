#pragma warning disable CA1040 // avoid empty interfaces

namespace ScrubJay.Building;

/// <summary>
/// Indicates that this instance is a Fluent Builder
/// </summary>
/// <typeparam name="S">
/// The <see cref="Type"/> of the instance (self)
/// </typeparam>
/// <remarks>
/// This is a marker interface for <see cref="FluentBuilderExtensions"/>
/// </remarks>
/// <seealso href="https://en.wikipedia.org/wiki/Fluent_interface"/>
/// <seealso href="https://en.wikipedia.org/wiki/Builder_pattern"/>
[PublicAPI]
public interface IFluentBuilder<S>
    where S : IFluentBuilder<S>
{
    S Self { get; }
}