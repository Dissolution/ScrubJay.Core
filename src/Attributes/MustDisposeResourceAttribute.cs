// ReSharper disable once CheckNamespace
namespace JetBrains.Annotations;

/// <inheritdoc cref="JetBrains.Annotations.MustDisposeResourceAttribute"/>
/// <remarks>
/// The only difference is this can target a Struct
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