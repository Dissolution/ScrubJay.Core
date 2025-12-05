namespace ScrubJay.Rendering;

[PublicAPI]
[AttributeUsage(AttributeTargets.All, Inherited = false)]
public sealed class RenderAsAttribute : Attribute
{
    public string Display { get;  }

    public RenderAsAttribute(string display)
    {
        this.Display = display;
    }
}