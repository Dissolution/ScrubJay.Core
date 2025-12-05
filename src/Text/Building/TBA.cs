namespace ScrubJay.Text;

/// <summary>
/// Common <b>T</b>ext<b>B</b>uilder <b>A</b>ctions
/// </summary>
[PublicAPI]
public static class TBA
{
    public static Action<TextBuilder> None { get; } = static tb => { };

    public static Action<TextBuilder> NewLine { get; } = static tb => tb.NewLine();

    public static readonly string Render = "@";
}