namespace ScrubJay.Text;

/// <summary>
/// An instance of a <see cref="TextBuilderBase{B}"/>
/// </summary>
[PublicAPI]
[MustDisposeResource]
public sealed class TextBuilder : TextBuilderBase<TextBuilder>, IDisposable
{
    /// <summary>
    /// Gets a <c>new</c> <see cref="TextBuilder"/> instance
    /// </summary>
    public static TextBuilder New
    {
        [MustDisposeResource]
        get => new();
    }

    /// <summary>
    /// Builds a <see cref="string"/> with a temporary <see cref="TextBuilder"/> instance
    /// </summary>
    /// <param name="build">
    /// The <see cref="Action{T}"/> to perform on the <see cref="TextBuilder"/> instance that builds the result <see cref="string"/>
    /// </param>
    /// <returns>
    /// The <see cref="string"/> returned from the <see cref="TextBuilder"/> instance before it is returned
    /// </returns>
    public static string Build(Action<TextBuilder>? build) => New.Invoke(build).ToStringAndDispose();

    /// <summary>
    /// Construct a new, empty <see cref="TextBuilder"/>
    /// </summary>
    public TextBuilder() : base() { }

    /// <summary>
    /// Construct a new, empty <see cref="TextBuilder"/> with a minimum starting capacity
    /// </summary>
    /// <param name="minCapacity"></param>
    public TextBuilder(int minCapacity) : base(minCapacity)
    {

    }
}
