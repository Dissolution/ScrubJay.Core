#pragma warning disable CA1008

namespace ScrubJay.Text;

/// <summary>
/// The alignment of text within a space
/// </summary>
/// <remarks>
/// This is a flagged enum so that a bias can be indicated for <see cref="Center"/>:<br/>
/// <see cref="Right"/> is the default alignment<br/>
/// <see cref="Left"/> is the opposite (<c>Left &amp; Right == Left</c>)<br/>
/// <see cref="Center"/> defaults to a right-bias when there is an odd space<br/>
/// <c>Center | Left</c> indicates a center alignment, with a <b>left</b>-bias when there is an odd space<br/>
/// </remarks>
[PublicAPI]
[Flags]
public enum Alignment
{
    Right = 0,
    Left = 1 << 0,
    Center = 1 << 1,
    // CenterRight = Center,
    // CenterLeft = Center | Left,
}
