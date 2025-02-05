#pragma warning disable S2743, CA1000

using System.Reflection;
using ScrubJay.Parsing;
using ScrubJay.Text;

namespace ScrubJay.Enums;

[PublicAPI]
public abstract class EnumInfo
{
    public static Result<TEnum, ParseException> TryParse<TEnum>(ReadOnlySpan<char> text)
        where TEnum : struct, Enum
        => EnumInfo<TEnum>.TryParse(text);

    public static Result<TEnum, ParseException> TryParse<TEnum>(string? str)
        where TEnum : struct, Enum
        => EnumInfo<TEnum>.TryParse(str);

    public Attribute[] Attributes { get; }
    public string Name { get; }
    public bool IsFlags { get; }
    public Type EnumType { get; }
    public Type UnderlyingType { get; }

    protected EnumInfo(Type enumType)
    {
        Throw.IfNull(enumType);
        if (!enumType.IsEnum)
            throw new ArgumentException("Type is not an enum type", nameof(enumType));
        EnumType = enumType;
        Name = enumType.Name;
        UnderlyingType = Enum.GetUnderlyingType(enumType);
        Attributes = Attribute.GetCustomAttributes(enumType, true);
        IsFlags = Attributes.OfType<FlagsAttribute>().Any();
    }
}

[PublicAPI]
public class EnumInfo<TEnum> : EnumInfo
    where TEnum : struct, Enum
{
    private static readonly Dictionary<TEnum, EnumMemberInfo<TEnum>> _memberInfos = [];

    public static EnumInfo<TEnum> Instance { get; } = new();

    private static Option<TEnum> TryParseMember(ReadOnlySpan<char> text)
    {
        foreach (var pair in _memberInfos)
        {
            if (pair.Value.TryParse(text))
                return pair.Key;
        }
        return None();
    }

    public static Result<TEnum, ParseException> TryParse(ReadOnlySpan<char> text)
    {
        text = text.Trim();
        if (text.Length == 0)
            return ParseException.Create<TEnum>(text, "Empty text");

        if (TryParseMember(text).HasSome(out var @enum))
            return @enum;

        if (Instance.IsFlags)
        {
            @enum = default;

            var splitText = SpanSplitter<char>.SplitAny(text, [',', '|', ' '], SpanSplitterOptions.IgnoreEmpty);
            foreach (var segment in splitText)
            {
                if (TryParseMember(segment).HasSome(out var flag))
                {
                    @enum.AddFlag(flag);
                    continue;
                }
                return ParseException.Create<TEnum>(splitText.Current);
            }

            if (@enum.IsDefault())
                return ParseException.Create<TEnum>(text, "Could not parse into a valid set of flags");
            return @enum;
        }

        return ParseException.Create<TEnum>(text);
    }

    public static Result<TEnum, ParseException> TryParse(string? str) => TryParse(str.AsSpan());

    private EnumInfo() : base(typeof(TEnum))
    {
        var type = typeof(TEnum);
        var memberFields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in memberFields)
        {
            TEnum member = (TEnum)field.GetValue(null)!;
            _memberInfos[member] = new EnumMemberInfo<TEnum>(this, field);
        }
    }

    public string AsString(TEnum @enum)
    {
        if (_memberInfos.TryGetValue(@enum, out var memberInfo))
            return memberInfo.DisplayName;
        if (IsFlags)
        {
            var flags = @enum.GetFlags();
            if (flags.Length == 0)
                return @enum.ToString();

            var buffer = new Buffer<char>();
            TEnum flag = flags[0];
            string flagString = _memberInfos.TryGetValue(flag, out memberInfo) ? memberInfo.DisplayName : flag.ToString();
            buffer.Write(flagString);
            for (var i = 1; i < flags.Length; i++)
            {
                buffer.Write(" | ");
                flag = flags[i];
                flagString = _memberInfos.TryGetValue(flag, out memberInfo) ? memberInfo.DisplayName : flag.ToString();
                buffer.Write(flagString);
            }
            return buffer.ToStringAndDispose();
        }

        return @enum.ToString();
    }

}


[PublicAPI]
public abstract class EnumMemberInfo
{
    public EnumInfo EnumInfo { get; }

    public Attribute[] Attributes { get; }
    public string Name { get; }
    public string DisplayName { get; }
    public string[] ParseNames { get; }

    protected EnumMemberInfo(EnumInfo enumInfo, FieldInfo enumField)
    {
        EnumInfo = enumInfo;
        Name = enumField.Name;
        Attributes = Attribute.GetCustomAttributes(enumField, true);

        string? display = null;
        List<string> parseNames = [Name];

        foreach (var attribute in Attributes)
        {
            if (attribute is DescriptionAttribute description)
            {
                display ??= description.Description;
                parseNames.Add(display);
            }
        }
        this.DisplayName = display ?? Name;
        this.ParseNames = parseNames.ToArray();
    }
}

[PublicAPI]
public class EnumMemberInfo<TEnum> : EnumMemberInfo
    where TEnum : struct, Enum
{
    internal EnumMemberInfo(EnumInfo<TEnum> enumInfo, FieldInfo enumField)
        : base(enumInfo, enumField)
    {
    }

    internal bool TryParse(ReadOnlySpan<char> text)
    {
        foreach (var parseName in ParseNames)
        {
            if (Equate.Text(parseName, text, StringComparison.Ordinal))
                return true;
        }
        return false;
    }
}
