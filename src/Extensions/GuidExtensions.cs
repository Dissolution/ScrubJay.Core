#pragma warning disable CA1720

using System.Globalization;

namespace ScrubJay.Extensions;

[PublicAPI]
public static class GuidExtensions
{
    public static string ToUpperDigitsString(this Guid guid)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        string str = guid.ToString("N");
        str = str.ToUpper(CultureInfo.InvariantCulture);
        return str;
#else
        Span<char> buffer = stackalloc char[32];
#if DEBUG
        bool wrote = guid.TryFormat(buffer, out int charsWritten, format: "N");
        Debug.Assert(wrote);
        Debug.Assert(charsWritten == 32);
#else
        _ = guid.TryFormat(buffer, out _, format: "N");
#endif
        for (var i = 0; i < 32; i++)
        {
            buffer[i] = char.ToUpper(buffer[i], CultureInfo.InvariantCulture);
        }

        return buffer.AsString();
#endif
    }
}

