namespace ScrubJay.Text.Rendering;

[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RendererPriorityAttribute : Attribute
{
    public int Priority { get; } = 0;

    public RendererPriorityAttribute()
    {

    }

    public RendererPriorityAttribute(int priority)
    {
        Priority = priority;
    }
}