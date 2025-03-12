namespace ScrubJay.Text;

/// <summary>
/// Indicates that this instance can write a representation of itself to a <see cref="TextBuilderBase{B}"/>
/// </summary>
[PublicAPI]
public interface IWriteable
{
    /// <summary>
    /// Write a representation of this <see cref="IWriteable"/> to a <see cref="TextBuilderBase{B}"/>
    /// </summary>
    void WriteTo<TBuilder>(TBuilder textBuilder)
        where TBuilder : TextBuilderBase<TBuilder>;
}
