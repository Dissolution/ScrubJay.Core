// ReSharper disable CommentTypo
#pragma warning disable CA1716

using System.Globalization;

namespace ScrubJay.Text;

[PublicAPI]
public enum Case
{
    /// <summary>
    /// Default naming convention
    /// </summary>
    /// <remarks>
    /// "membername" -> "membername"<br/>
    /// "memberName" -> "memberName"<br/>
    /// "MemberName" -> "MemberName"<br/>
    /// "MEMBERNAME" -> "MEMBERNAME"<br/>
    /// </remarks>
    Default = 0,

    /// <summary>
    /// Lowercase naming
    /// </summary>
    /// <remarks>
    /// "membername" -> "membername"<br/>
    /// "memberName" -> "membername"<br/>
    /// "MemberName" -> "membername"<br/>
    /// "MEMBERNAME" -> "membername"<br/>
    /// </remarks>
    Lower,

    /// <summary>
    /// Uppercase naming
    /// </summary>
    /// <remarks>
    /// "membername" -> "MEMBERNAME"<br/>
    /// "memberName" -> "MEMBERNAME"<br/>
    /// "MemberName" -> "MEMBERNAME"<br/>
    /// "MEMBERNAME" -> "MEMBERNAME"<br/>
    /// </remarks>
    Upper,

    /// <summary>
    /// Camel-cased naming
    /// </summary>
    /// <remarks>
    /// "membername" -> "membername"<br/>
    /// "memberName" -> "memberName"<br/>
    /// "MemberName" -> "memberName"<br/>
    /// "MEMBERNAME" -> "mEMBERNAME"<br/>
    /// </remarks>
    Camel,

    /// <summary>
    /// Pascal-cased naming
    /// </summary>
    /// <remarks>
    /// "membername" -> "Membername"<br/>
    /// "memberName" -> "MemberName"<br/>
    /// "MemberName" -> "MemberName"<br/>
    /// "MEMBERNAME" -> "MEMBERNAME"<br/>
    /// </remarks>
    Pascal,

    Title,

    /// <summary>
    /// Snake case naming
    /// </summary>
    /// <remarks>
    /// "membername" -> "membername"<br/>
    /// "memberName" -> "member_Name"<br/>
    /// "MemberName" -> "Member_Name"<br/>
    /// "MEMBERNAME" -> "MEMBERNAME"<br/>
    /// </remarks>
    Snake,
}

/// <summary>
/// Extensions related to <see cref="Case"/>
/// </summary>
public static class CaseExtensions
{
    private static readonly TextInfo _textInfo = CultureInfo.CurrentCulture.TextInfo;

    [return: NotNullIfNotNull(nameof(text))]
    public static string? Cased(this string? text, Case naming)
    {
        if (text is null)
            return null;
        switch (naming)
        {
            case Case.Lower:
                return _textInfo.ToLower(text);
            case Case.Upper:
                return _textInfo.ToUpper(text);
            case Case.Camel:
            {
                int len = text.Length;
                if (len == 0)
                    return "";
                if (char.IsLower(text[0]))
                    return text;

                Span<char> nameBuffer = stackalloc char[len];
                nameBuffer[0] = _textInfo.ToLower(text[0]);
                Sequence.CopyTo(text.AsSpan(1), nameBuffer.Slice(1));
                return nameBuffer.ToString();
            }
            case Case.Pascal:
            {
                int len = text.Length;
                if (len == 0)
                    return "";
                if (char.IsUpper(text[0]))
                    return text;

                Span<char> nameBuffer = stackalloc char[len];
                nameBuffer[0] = _textInfo.ToUpper(text[0]);
                text.AsSpan(1).CopyTo(nameBuffer.Slice(1));
                return nameBuffer.ToString();
            }
            case Case.Title:
                return _textInfo.ToTitleCase(text);
            case Case.Snake:
            {
                // todo: https://github.com/efcore/EFCore.NamingConventions/blob/main/EFCore.NamingConventions/Internal/SnakeCaseNameRewriter.cs
                int len = text.Length;
                if (len < 2)
                    return text;
                SpanWriter<char> nameBuffer = new(stackalloc char[len * 2]); // Aggressive
                // First char
                _ = nameBuffer.TryWrite(text[0]);
                // The rest
                for (var i = 1; i < len; i++)
                {
                    char ch = text[i];
                    if (char.IsUpper(ch))
                    {
                        _ = nameBuffer.TryWrite('_');
                    }

                    _ = nameBuffer.TryWrite(ch);
                }

                return nameBuffer.Written.ToString();
            }
            case Case.Default:
            default:
                return text;
        }
    }
}
