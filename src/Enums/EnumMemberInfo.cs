#pragma warning disable CA1819

#if !NETFRAMEWORK && !NETSTANDARD
using System.ComponentModel.DataAnnotations;
#endif
using System.Reflection;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Enums;

[PublicAPI]
public abstract class EnumMemberInfo :
#if NET7_0_OR_GREATER
    IEqualityOperators<EnumMemberInfo, EnumMemberInfo, bool>,
#endif
    IEquatable<EnumMemberInfo>,
    IRenderable
{
    public static bool operator ==(EnumMemberInfo? left, EnumMemberInfo? right)
        => Equate.Values(left, right);

    public static bool operator !=(EnumMemberInfo? left, EnumMemberInfo? right)
        => !Equate.Values(left, right);

    protected static void AddAlias(string? alias, HashSet<string> aliases, ref string render)
    {
        if (!string.IsNullOrWhiteSpace(alias))
        {
            if (aliases.Add(alias!))
            {
                render = alias!;
            }
        }
    }

    public static EnumMemberInfo? For(Enum @enum)
    {
        return EnumInfo.For(@enum).GetMemberInfo(@enum);
    }

    public static EnumMemberInfo<E>? For<E>(E @enum)
        where E : struct, Enum
    {
        return EnumInfo.For<E>().GetMemberInfo(@enum);
    }

    private readonly HashSet<string> _aliases = [];
    private readonly string _display;

    public EnumInfo EnumInfo { get; }
    public Attribute[] Attributes { get; }
    public string Name { get; }
    public string Display => _display;
    public IReadOnlyCollection<string> Aliases => _aliases;
    public object Member { get; }
    public long I64Value { get; }

    protected EnumMemberInfo(EnumInfo enumInfo, FieldInfo memberField)
    {
        Debug.Assert(enumInfo is not null);
        Debug.Assert(memberField is not null);
        Debug.Assert(memberField!.IsStatic);
        Debug.Assert(memberField.DeclaringType == enumInfo!.EnumType);

        EnumInfo = enumInfo;
        Attributes = Attribute.GetCustomAttributes(memberField);
        Name = memberField.Name;
        Member = memberField.GetValue(null).ThrowIfNull();
        I64Value = Convert.ToInt64(Member);

        // default render is name
        _display = Name;
        _aliases = new(StringComparer.Ordinal);

        // In order of least important to most (overwrite)
#if !NETFRAMEWORK && !NETSTANDARD
        if (Attributes.TryGet<DisplayAttribute>(out var displayAttr))
        {
            AddAlias(displayAttr.Name, _aliases, ref _display);
            AddAlias(displayAttr.ShortName, _aliases, ref _display);
            AddAlias(displayAttr.Description, _aliases, ref _display);
        }
#endif

        if (Attributes.TryGet<DescriptionAttribute>(out var descriptionAttr))
        {
            AddAlias(descriptionAttr.Description, _aliases, ref _display);
        }

        if (Attributes.TryGet<DisplayAsAttribute>(out var displayAsAttr))
        {
            AddAlias(displayAsAttr.Rendered, _aliases, ref _display);
        }

        // shrink aliases to save memory
        _aliases.TrimExcess();
    }

    public bool HasAlias(string? str)
    {
        if (str is null)
            return false;

        if (str == Name)
            return true;

        return _aliases.Contains(str);
    }

    public bool Equals(EnumMemberInfo? enumMemberInfo)
    {
        return enumMemberInfo is not null &&
               enumMemberInfo.EnumInfo == EnumInfo &&
               enumMemberInfo.Name == Name;
    }

    public bool Equals(Enum e)
    {
        return e.GetType() == EnumInfo.EnumType &&
               e.ToString() == Name;
    }

    public override bool Equals(object? obj)
    {
        if (obj is EnumMemberInfo enumMemberInfo)
            return Equals(enumMemberInfo);
        if (obj is Enum e)
            return Equals(e);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.HashMany(EnumInfo, Name);
    }

    public void RenderTo(TextBuilder builder)
    {
        builder.Write(_display);
    }

    public override string ToString() => Name;
}