namespace ScrubJay.Enums;

[PublicAPI]
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class AliasesAttribute : Attribute
{
    public string[] Aliases { get; }

    public AliasesAttribute(params string[] aliases)
    {
        this.Aliases = aliases;
    }

    public AliasesAttribute(IEnumerable<string> aliases)
    {
        this.Aliases = aliases.ToArray();
    }

    public AliasesAttribute(ReadOnlySpan<string> aliases)
    {
        this.Aliases = aliases.ToArray();
    }
}