// ReSharper disable once CheckNamespace
namespace JetBrains.Annotations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
internal sealed class MustDisposeResourceAttribute : Attribute
{
    public MustDisposeResourceAttribute()
    {
        Value = true;
    }

    public MustDisposeResourceAttribute(bool value)
    {
        Value = value;
    }

    /// <summary>
    /// When set to <c>false</c>, disposing of the resource is not obligatory.
    /// The main use-case for explicit <c>[MustDisposeResource(false)]</c> annotation is to loosen inherited annotation.
    /// </summary>
    public bool Value { get; }
}