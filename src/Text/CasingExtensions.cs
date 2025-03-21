// Flags Enum default should be None

#pragma warning disable CA1008

using System.Globalization;

namespace ScrubJay.Text;

[PublicAPI]
public enum Casing
{
    Default = 0,
    Lower,
    Upper,
    Camel,
    Pascal,
    Title,
}

[PublicAPI]
public static class TextCaseExtensions
{
    private static readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    private static readonly TextInfo _textInfo = _culture.TextInfo;

    private static string ToSnakeCase(string str, Casing casing)
    {
        using var text = new TextBuilder();
        Option<UnicodeCategory> previousCategory = default;

        int len = str.Length;
        Debug.Assert(len > 0);

        char ch = str[0];

        if (ch == '_')
        {

        }
        else if (casing is Casing.Camel or Casing.Lower)
        {
            ch = char.ToLower(ch, _culture);
        }
        else if (casing is (Casing.Upper or Casing.Pascal or Casing.Title))
        {
            ch = char.ToUpper(ch, _culture);
        }

        text.Append(ch);

        // The rest
        for (int i = 1; i < len; i++)
        {
            ch = str[i];
            if (ch == '_')
            {
                text.Append('_');
                previousCategory = default;
                continue;
            }

            // transform
            if (casing is Casing.Lower)
            {
                ch = char.ToLower(ch, _culture);
            }
            else if (casing is Casing.Upper)
            {
                ch = char.ToUpper(ch, _culture);
            }

            // Do we need to insert an underscore?
            var category = char.GetUnicodeCategory(ch);
            #pragma warning disable IDE0010
            switch (category)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                {
                    if ((previousCategory == UnicodeCategory.SpaceSeparator) ||
                        (previousCategory == UnicodeCategory.LowercaseLetter) ||
                        ((previousCategory != UnicodeCategory.DecimalDigitNumber) &&
                        previousCategory.IsSome && (i > 0) && ((i + 1) < len) && char.IsLower(str[i + 1])))
                    {
                        text.Append('_');
                        if (casing == Casing.Title)
                        {
                            ch = char.ToUpper(ch, _culture);
                        }
                    }
                    break;
                }
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                {
                    if (previousCategory == UnicodeCategory.SpaceSeparator)
                    {
                        text.Append('_');
                        if (casing == Casing.Title)
                        {
                            ch = char.ToUpper(ch, _culture);
                        }
                    }
                    break;
                }
                default:
                {
                    if (previousCategory.IsNone)
                    {
                        previousCategory = Some(UnicodeCategory.SpaceSeparator);
                    }
                    continue;
                }
            }

            text.Append(ch);
            previousCategory = Some(category);
        }

        return text.ToString();
    }


    [return: NotNullIfNotNull(nameof(text))]
    public static string? ToCase(this string? str, Casing caseConvention, bool snake = false)
    {
        if (str is null)
            return null;

        int len = str!.Length;
        if (len == 0)
            return string.Empty;

        if (snake)
            return ToSnakeCase(str, caseConvention);

        switch (caseConvention)
        {
            case Casing.Lower:
                return _textInfo.ToLower(str);
            case Casing.Upper:
                return _textInfo.ToUpper(str);
            case Casing.Camel:
            {
                Span<char> buffer = stackalloc char[len];
                buffer[0] = char.ToLower(str[0], _culture);
                Notsafe.Text.CopyBlock(str.AsSpan(1), buffer.Slice(1), len - 1);
                return buffer.AsString();
            }
            case Casing.Pascal:
            {
                Span<char> buffer = stackalloc char[len];
                buffer[0] = char.ToUpper(str[0], _culture);
                Notsafe.Text.CopyBlock(str.AsSpan(1), buffer.Slice(1), len - 1);
                return buffer.AsString();
            }
            case Casing.Title:
                return _textInfo.ToTitleCase(str);
            case Casing.Default:
            default:
                return str;
        }
    }
}
