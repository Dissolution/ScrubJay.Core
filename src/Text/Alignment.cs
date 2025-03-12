namespace ScrubJay.Text;

/// <summary>
/// Indicates the alignment of text within a space
/// </summary>
/// <remarks>
/// This is a <see cref="FlagsAttribute"/> <see cref="Enum"/> so that a bias can be indicated for centering operations<br/>
/// <i>e.g.</i> <c>Left | Center</c> is a left-bias when there is an uneven amount of spacing<br/>
/// <c>Left | Right</c> is considered an invalid <see cref="Alignment"/>
/// </remarks>
[PublicAPI]
[Flags]
public enum Alignment
{
    None = 0,
    Left = 1 << 0,
    Right = 1 << 1,
    Center = 1 << 2,
}
