namespace ScrubJay.Text.Rendering;

[PublicAPI]
[AttributeUsage(AttributeTargets.Field)]
public class DisplayAsAttribute : Attribute
{
    public string Rendered { get;  }

    public DisplayAsAttribute(string rendered)
    {
        this.Rendered = rendered;
    }
}