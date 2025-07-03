#pragma warning disable CA1819 // Properties should not return arrays

#if !NETFRAMEWORK && !NETSTANDARD
using System.ComponentModel.DataAnnotations;
#endif
using System.Reflection;

namespace ScrubJay.Enums;

[PublicAPI]
public abstract record class EnumMemberInfo
{
    private readonly HashSet<string> _aliases = new HashSet<string>(StringComparer.Ordinal);

    public required Attribute[] Attributes { get; init; }
    public required string Name { get; init; }

    public IReadOnlyCollection<string> Aliases => _aliases;

    protected EnumMemberInfo()
    {
    }

    [SetsRequiredMembers]
    protected EnumMemberInfo(FieldInfo memberField)
    {
        Debug.Assert(memberField is not null);
        Debug.Assert(memberField!.IsStatic);

        Attributes = Attribute.GetCustomAttributes(memberField);
        Name = memberField.Name;

        if (Attributes.Has<AliasesAttribute>().IsSome(out var aliasesAttr))
        {
            AddAliases(aliasesAttr.Aliases);
        }

        if (Attributes.Has<DescriptionAttribute>().IsSome(out var descriptionAttr))
        {
            AddAlias(descriptionAttr.Description);
        }

#if !NETFRAMEWORK && !NETSTANDARD
        if (Attributes.Has<DisplayAttribute>().IsSome(out var displayAttr))
        {
            AddAlias(displayAttr.Name);
            AddAlias(displayAttr.ShortName);
            AddAlias(displayAttr.Description);
        }
#endif

        _aliases.TrimExcess();
    }

    protected void AddAlias(string? alias)
    {
        if (alias is not null)
            _aliases.Add(alias);
    }

    protected void AddAliases(IEnumerable<string>? aliases)
    {
        if (aliases is not null)
        {
            foreach (var alias in aliases)
            {
                AddAlias(alias);
            }
        }
    }

    internal void AddAliases(EnumMemberInfo info)
    {
        AddAlias(info.Name);
        AddAliases(info._aliases);
        _aliases.TrimExcess();
    }

    public bool Matches(string? str, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null) return false;
        if (str.Equals(Name, comparison))
            return true;
        if (comparison == StringComparison.Ordinal)
        {
            return _aliases.Contains(str);
        }

        foreach (var alias in _aliases)
        {
            if (str.Equals(alias, comparison))
                return true;
        }

        return false;
    }

    public override string ToString() => Name;
}

[PublicAPI]
public sealed record class EnumMemberInfo<E> : EnumMemberInfo
    where E : struct, Enum
{
    public required E Value { get; init; }

    [SetsRequiredMembers]
    internal EnumMemberInfo(FieldInfo memberField) : base(memberField)
    {
        object? obj = memberField.GetValue(null);
        Value = obj.ThrowIfNot<E>();
    }

    public EnumMemberInfo() : base()
    {
    }
}