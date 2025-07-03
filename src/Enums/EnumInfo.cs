#pragma warning disable CA1819 // Properties should not return arrays

using System.Reflection;
using ScrubJay.Parsing;

namespace ScrubJay.Enums;

[PublicAPI]
public abstract record class EnumInfo
{
    public Attribute[] Attributes { get; init; } = [];
    public required Type EnumType { get; init; }
    public required Type UnderlyingType { get; init; }
    public bool IsFlags { get; init; }

    protected EnumInfo()
    {
    }

    [SetsRequiredMembers]
    protected EnumInfo(Type enumType)
    {
        Debug.Assert(enumType is not null);
        Debug.Assert(enumType!.IsEnum);
        EnumType = enumType;
        UnderlyingType = Enum.GetUnderlyingType(enumType);
        Attributes = Attribute.GetCustomAttributes(enumType);
        IsFlags = Attributes.Has<FlagsAttribute>();
    }
}

public sealed record class EnumInfo<E> : EnumInfo
    where E : struct, Enum
{
    public static EnumInfo<E> Default { get; } = new();

    private static readonly Dictionary<E, EnumMemberInfo<E>> _members = [];

    [SetsRequiredMembers]
    private EnumInfo() : base(typeof(E))
    {
        var members = EnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var memberField in members)
        {
            var enumMemberInfo = new EnumMemberInfo<E>(memberField);
            if (!_members.TryAdd(enumMemberInfo.Value, enumMemberInfo))
            {
                // This is an alias
                _members[enumMemberInfo.Value].AddAliases(enumMemberInfo);
            }
        }
    }

    public static Result<E> TryParse(string? str, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null)
            return new ArgumentNullException(nameof(str));

        int len = str.Length;
        if (len == 0)
        {
            return ParseException.Create<E>(str, "String was empty");
        }

        foreach (var (e, info) in _members)
        {
            if (info.Matches(str, comparison))
                return e;
        }

        return ParseException.Create<E>(str, "String was invalid");
    }
}