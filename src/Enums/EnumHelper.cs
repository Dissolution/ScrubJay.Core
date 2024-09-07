namespace ScrubJay.Enums;

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