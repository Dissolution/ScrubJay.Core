namespace ScrubJay.Text.Rendering;

[PublicAPI]
[AttributeUsage(AttributeTargets.Field)]
public class RenderAsAttribute : Attribute
{
    public string Rendered { get;  }

    public RenderAsAttribute(string rendered)
    {
        this.Rendered = rendered;
    }
}