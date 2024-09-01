// ReSharper disable once CheckNamespace
namespace JetBrains.Annotations;

/// <inheritdoc cref="JetBrains.Annotations.MustDisposeResourceAttribute"/>
/// <remarks>
/// This version of <see cref="MustDisposeResourceAttribute"/> supports struct targets
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
internal sealed class MustDisposeResourceAttribute : Attribute
{
    public bool Value { get; }
    
    public MustDisposeResourceAttribute()
    {
        this.Value = true;
    }

    public MustDisposeResourceAttribute(bool value)
    {
        this.Value = value;
    }
}