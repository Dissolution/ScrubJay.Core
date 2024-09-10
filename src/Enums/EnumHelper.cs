#pragma warning disable S2743
// ReSharper disable StaticMemberInGenericType

using System.Reflection;
using ScrubJay.Text;

namespace ScrubJay.Enums;

[PublicAPI]
public static class EnumHelper
{
    public static readonly char[] Separators = [',', '|', ' '];

    public static Result<TEnum, Exception> TryParse<TEnum>(string? text)
        where TEnum : struct, Enum
    {
        if (text is null)
            return new ArgumentNullException(nameof(text));

        if (Enum.TryParse<TEnum>(text, true, out var @enum))
        {
            return @enum;
        }

#if NET6_0_OR_GREATER
        var segments = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        @enum = default;
        foreach (var segment in segments)
        {
            if (Enum.TryParse<TEnum>(segment, true, out var flag))
            {
                @enum.AddFlag(flag);
            }
            else
            {
                return new ArgumentException($"Could not parse segment '{segment}' to a '{typeof(TEnum).Name}'", nameof(text));
            }
        }

        return @enum;
#else
        var segments = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
        @enum = default;
        foreach (var segment in segments)
        {
            var trimmed = segment.Trim();
            
            if (Enum.TryParse<TEnum>(trimmed, true, out var flag))
            {
                @enum.AddFlag(flag);
            }
            else
            {
                return new ArgumentException($"Could not parse segment '{trimmed}' to a '{typeof(TEnum).Name}'", nameof(text));
            }
        }

        return @enum;
#endif
    }
}


[PublicAPI]
public static class EnumHelper<TEnum>
    where TEnum : struct, Enum
{
    private static readonly Dictionary<TEnum, string> _names = [];

    public static readonly bool IsFlags;

    static EnumHelper()
    {
        var type = typeof(TEnum);
        IsFlags = type.GetCustomAttribute<FlagsAttribute>() is not null;

        var memberFields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in memberFields)
        {
            object member = field.GetValue(null)!;
            string? name = null;

            var descAttr = field.GetCustomAttribute<DescriptionAttribute>();

            if (descAttr is not null)
            {
                name = descAttr.Description;
            }

            name ??= Enum.GetName(type, member);
            name ??= field.Name;

            _names[(TEnum)member] = name;
        }
    }

    public static string GetName(TEnum @enum)
    {
        if (_names.TryGetValue(@enum, out var name))
            return name;

        var flags = @enum.GetFlags();
        if (flags.Length == 0 || flags.Length == 1)
            return @enum.ToString();

        var text = new TextBuffer();
        text.Append(GetName(flags[0]));
        for (var i = 1; i < flags.Length; i++)
        {
            text.Append(" | ");
            text.Append(GetName(flags[i]));
        }

        return text.ToStringAndDispose();
    }
}