namespace ScrubJay.Text;

/// <summary>
/// Indicates the alignment of text within a space
/// </summary>
/// <remarks>
/// <see cref="Alignment"/> has <c>[Flags]</c> so that a bias can be indicated for <see cref="Center"/> operations<br/>
/// <i>e.g.</i> <c>Left | Center</c> for 'x' in width 4 results in "_x__"<br/>
/// whereas <c>Right | Center</c> for 'x' in width 4 results in "__x_"<br/>
/// <c>Left | Right</c> is treated the same as <see cref="Center"/>
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
