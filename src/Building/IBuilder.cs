#pragma warning disable CA1040 // avoid empty interfaces

namespace ScrubJay.Building;

/// <summary>
/// Indicates that this is an implementation of the Builder Pattern
/// </summary>
/// <typeparam name="B">
/// The <see cref="Type"/> of the builder implementation
/// </typeparam>
/// <see href="https://en.wikipedia.org/wiki/Builder_pattern"/>
/// <remarks>
/// This is a marker interface for <see cref="BuilderExtensions"/>
/// </remarks>
[PublicAPI]
public interface IBuilder<B>
    where B : IBuilder<B>
{

}