namespace ScrubJay.Text.Rendering;

[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RendererPriorityAttribute : Attribute
{
    public int Priority { get; }

    public RendererPriorityAttribute()
    {
        Priority = int.MinValue;
    }

    public RendererPriorityAttribute(int priority)
    {
        this.Priority = priority;
    }
}