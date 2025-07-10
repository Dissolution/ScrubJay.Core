namespace ScrubJay.Text.Rendering;

[PublicAPI]
[AttributeUsage(AttributeTargets.All)]
public sealed class RenderAsAttribute : Attribute
{
    public string Name { get; }

    public RenderAsAttribute(string name)
    {
        this.Name = name;
    }
}