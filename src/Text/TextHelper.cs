#pragma warning disable CA1034, CA1045, CA1062, CA1200
#pragma warning disable IDE0060, IDE0022
#pragma warning disable CA1307
// ReSharper disable EntityNameCapturedOnly.Global
// ReSharper disable ArrangeMethodOrOperatorBody
// ReSharper disable InvokeAsExtensionMethod

using static ScrubJay.Utilities.Notsafe.Text;

namespace ScrubJay.Text;

/// <summary>
/// Methods for efficiently working on textual types (<see cref="char"/>, <see cref="string"/>, <see cref="text">ReadOnlySpan&lt;char&gt;</see>
/// </summary>
[PublicAPI]
public static class TextHelper
{
#region TryCopyTo
    /* All *Copy methods validate inputs before calling Notsafe.Text.CopyBlock
     * Source Types: char, string?, ReadOnlySpan<char>, Span<char>, char[]?
     * Dest.  Types: Span<char>, char[]?
     */

    public static bool TryCopyTo(char source, Span<char> destination)
    {
        if (destination.Length == 0)
            return false;
        destination[0] = source;
        return true;
    }

    public static bool TryCopyTo(char source, char[]? destination)
    {
        if (destination is null || (destination.Length == 0))
            return false;
        destination[0] = source;
        return true;
    }

    public static bool TryCopyTo(string? source, Span<char> destination)
    {
        if (source is null)
            return true;
        int count = source.Length;
        if (count <= destination.Length)
        {
            CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopyTo(string? source, char[]? destination)
    {
        if (source is null)
            return true;
        if (destination is null)
            return false;
        int count = source.Length;
        if (count <= destination.Length)
        {
            CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopyTo(text source, Span<char> destination)
    {
        int count = source.Length;
        if (count <= destination.Length)
        {
            CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopyTo(text source, char[]? destination)
    {
        if (destination is null)
            return false;
        int count = source.Length;
        if (count <= destination.Length)
        {
            CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopyTo(Span<char> source, Span<char> destination)
    {
        int count = source.Length;
        if (count <= destination.Length)
        {
            CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopyTo(Span<char> source, char[]? destination)
    {
        if (destination is null)
            return false;
        int count = source.Length;
        if (count <= destination.Length)
        {
            CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopyTo(char[]? source, Span<char> destination)
    {
        if (source is null)
            return true;
        int count = source.Length;
        if (count <= destination.Length)
        {
            CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopyTo(char[]? source, char[]? destination)
    {
        if (source is null)
            return true;
        if (destination is null)
            return false;
        int count = source.Length;
        if (count <= destination.Length)
        {
            CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }
#endregion

    /* Compare + Equate get the same rotation of types:
     * char, string?, char[]?, text
     */

#region Compare
#region Ordinal
    public static int Compare(char left, char right)
        => left.CompareTo(right);

    public static int Compare(char left, string? right)
        => Compare(left.AsSpan(), right.AsSpan());

    public static int Compare(char left, char[]? right)
        => Compare(left.AsSpan(), right.AsSpan());

    public static int Compare(char left, text right)
        => Compare(left.AsSpan(), right);

    public static int Compare(string? left, char right)
        => Compare(left.AsSpan(), right.AsSpan());

    public static int Compare(string? left, string? right)
        => string.CompareOrdinal(left, right);

    public static int Compare(string? left, char[]? right)
        => Compare(left.AsSpan(), right.AsSpan());

    public static int Compare(string? left, text right)
        => Compare(left.AsSpan(), right);

    public static int Compare(char[]? left, char right)
        => Compare(left.AsSpan(), right.AsSpan());

    public static int Compare(char[]? left, string? right)
        => Compare(left.AsSpan(), right.AsSpan());

    public static int Compare(char[]? left, char[]? right)
        => Compare(left.AsSpan(), right.AsSpan());

    public static int Compare(char[]? left, text right)
        => Compare(left.AsSpan(), right);

    public static int Compare(text left, char right)
        => Compare(left, right.AsSpan());

    public static int Compare(text left, string? right)
        => Compare(left, right.AsSpan());

    public static int Compare(text left, char[]? right)
        => Compare(left, right.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(text left, text right)
        => MemoryExtensions.SequenceCompareTo<char>(left, right);
#endregion

#region with StringComparison
    public static int Compare(char left, char right, StringComparison comparison)
        => Compare(left.AsSpan(), right.AsSpan(), comparison);

    public static int Compare(char left, string? right, StringComparison comparison)
        => Compare(left.AsSpan(), right.AsSpan(), comparison);

    public static int Compare(char left, char[]? right, StringComparison comparison)
        => Compare(left.AsSpan(), right.AsSpan(), comparison);

    public static int Compare(char left, text right, StringComparison comparison)
        => Compare(left.AsSpan(), right, comparison);

    public static int Compare(string? left, char right, StringComparison comparison)
        => Compare(left.AsSpan(), right.AsSpan(), comparison);

    public static int Compare(string? left, string? right, StringComparison comparison)
        => Compare(left.AsSpan(), right.AsSpan(), comparison);

    public static int Compare(string? left, char[]? right, StringComparison comparison)
        => Compare(left.AsSpan(), right.AsSpan(), comparison);

    public static int Compare(string? left, text right, StringComparison comparison)
        => Compare(left.AsSpan(), right, comparison);

    public static int Compare(char[]? left, char right, StringComparison comparison)
        => Compare(left.AsSpan(), right.AsSpan(), comparison);

    public static int Compare(char[]? left, string? right, StringComparison comparison)
        => Compare(left.AsSpan(), right.AsSpan(), comparison);

    public static int Compare(char[]? left, char[]? right, StringComparison comparison)
        => Compare(left.AsSpan(), right.AsSpan(), comparison);

    public static int Compare(char[]? left, text right, StringComparison comparison)
        => Compare(left.AsSpan(), right, comparison);

    public static int Compare(text left, char right, StringComparison comparison)
        => Compare(left, right.AsSpan(), comparison);

    public static int Compare(text left, string? right, StringComparison comparison)
        => Compare(left, right.AsSpan(), comparison);

    public static int Compare(text left, char[]? right, StringComparison comparison)
        => Compare(left, right.AsSpan(), comparison);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(text left, text right, StringComparison comparison)
        => MemoryExtensions.CompareTo(left, right, comparison);
#endregion
#endregion

#region Equate
#region Ordinal
    public static bool Equate(this char left, char right)
        => left.Equals(right);

    public static bool Equate(this char left, string? right)
        => Equate(left.AsSpan(), right.AsSpan());

    public static bool Equate(this char left, char[]? right)
        => Equate(left.AsSpan(), right.AsSpan());

    public static bool Equate(this char left, text right)
        => Equate(left.AsSpan(), right);

    public static bool Equate(this string? left, char right)
        => Equate(left.AsSpan(), right.AsSpan());

    public static bool Equate(this string? left, string? right)
        => string.Equals(left, right, StringComparison.Ordinal);

    public static bool Equate(this string? left, char[]? right)
        => Equate(left.AsSpan(), right.AsSpan());

    public static bool Equate(this string? left, text right)
        => Equate(left.AsSpan(), right);

    public static bool Equate(this char[]? left, char right)
        => Equate(left.AsSpan(), right.AsSpan());

    public static bool Equate(this char[]? left, string? right)
        => Equate(left.AsSpan(), right.AsSpan());

    public static bool Equate(this char[]? left, char[]? right)
        => Equate(left.AsSpan(), right.AsSpan());

    public static bool Equate(this char[]? left, text right)
        => Equate(left.AsSpan(), right);

    public static bool Equate(this text left, char right)
        => Equate(left, right.AsSpan());

    public static bool Equate(this text left, string? right)
        => Equate(left, right.AsSpan());

    public static bool Equate(this text left, char[]? right)
        => Equate(left, right.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equate(this text left, text right)
        => MemoryExtensions.SequenceEqual<char>(left, right);
#endregion

#region with StringComparison
    public static bool Equate(this char left, char right, StringComparison comparison)
        => Equate(left.AsSpan(), right.AsSpan(), comparison);

    public static bool Equate(this char left, string? right, StringComparison comparison)
        => Equate(left.AsSpan(), right.AsSpan(), comparison);

    public static bool Equate(this char left, char[]? right, StringComparison comparison)
        => Equate(left.AsSpan(), right.AsSpan(), comparison);

    public static bool Equate(this char left, text right, StringComparison comparison)
        => Equate(left.AsSpan(), right, comparison);

    public static bool Equate(this string? left, char right, StringComparison comparison)
        => Equate(left.AsSpan(), right.AsSpan(), comparison);

    public static bool Equate(this string? left, string? right, StringComparison comparison)
        => Equate(left.AsSpan(), right.AsSpan(), comparison);

    public static bool Equate(this string? left, char[]? right, StringComparison comparison)
        => Equate(left.AsSpan(), right.AsSpan(), comparison);

    public static bool Equate(this string? left, text right, StringComparison comparison)
        => Equate(left.AsSpan(), right, comparison);

    public static bool Equate(this char[]? left, char right, StringComparison comparison)
        => Equate(left.AsSpan(), right.AsSpan(), comparison);

    public static bool Equate(this char[]? left, string? right, StringComparison comparison)
        => Equate(left.AsSpan(), right.AsSpan(), comparison);

    public static bool Equate(this char[]? left, char[]? right, StringComparison comparison)
        => Equate(left.AsSpan(), right.AsSpan(), comparison);

    public static bool Equate(this char[]? left, text right, StringComparison comparison)
        => Equate(left.AsSpan(), right, comparison);

    public static bool Equate(this text left, char right, StringComparison comparison)
        => Equate(left, right.AsSpan(), comparison);

    public static bool Equate(this text left, string? right, StringComparison comparison)
        => Equate(left, right.AsSpan(), comparison);

    public static bool Equate(this text left, char[]? right, StringComparison comparison)
        => Equate(left, right.AsSpan(), comparison);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equate(this text left, text right, StringComparison comparison)
        => MemoryExtensions.Equals(left, right, comparison);
#endregion
#endregion

#region Contains
    public static bool Contains(string? str, char match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null)
            return false;
        return str.Contains(match, comparison);
    }

    public static bool Contains(string? str, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null || match is null)
            return false;
        return MemoryExtensions.Contains(str.AsSpan(), match.AsSpan(), comparison);
    }

    public static bool Contains(string? str, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null)
            return false;
        return MemoryExtensions.Contains(str.AsSpan(), match, comparison);
    }

    public static bool Contains(text text, char match, StringComparison comparison = StringComparison.Ordinal)
    {
        return MemoryExtensions.Contains(text, match.AsSpan(), comparison);
    }

    public static bool Contains(text text, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (match is null)
            return false;
        return MemoryExtensions.Contains(text, match.AsSpan(), comparison);
    }

    public static bool Contains(text text, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        return MemoryExtensions.Contains(text, match, comparison);
    }
#endregion

#region StartsWith
    public static bool StartsWith(string? str, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null || match is null)
            return false;
        return MemoryExtensions.StartsWith(str.AsSpan(), match.AsSpan(), comparison);
    }

    public static bool StartsWith(string? str, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null)
            return false;
        return MemoryExtensions.StartsWith(str.AsSpan(), match, comparison);
    }

    public static bool StartsWith(text text, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (match is null)
            return false;
        return MemoryExtensions.StartsWith(text, match.AsSpan(), comparison);
    }

    public static bool StartsWith(text text, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        return MemoryExtensions.StartsWith(text, match, comparison);
    }
#endregion

#region EndsWith
    public static bool EndsWith(string? str, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null || match is null)
            return false;
        return MemoryExtensions.EndsWith(str.AsSpan(), match.AsSpan(), comparison);
    }

    public static bool EndsWith(string? str, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null)
            return false;
        return MemoryExtensions.EndsWith(str.AsSpan(), match, comparison);
    }

    public static bool EndsWith(text text, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (match is null)
            return false;
        return MemoryExtensions.EndsWith(text, match.AsSpan(), comparison);
    }

    public static bool EndsWith(text text, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        return MemoryExtensions.EndsWith(text, match, comparison);
    }
#endregion


#region Misc.
    public static string Repeat(int count, char ch)
    {
        if (count < 0)
            return string.Empty;
        return new string(ch, count);
    }

    public static string Repeat(int count, scoped text text)
    {
        int textLength = text.Length;
        int totalLength = count * textLength;
        if (totalLength <= 0)
            return string.Empty;
        Span<char> buffer = stackalloc char[totalLength];
        int i = 0;
        do
        {
            CopyBlock(text, buffer[i..], textLength);
            i += textLength;
        } while (i < totalLength);
        return buffer.AsString();
    }

    public static bool TryUnboxText(object? obj, out text text)
    {
        if (obj is char)
        {
            ref char ch = ref Notsafe.UnboxRef<char>(obj);
            text = ch.AsSpan();
            return true;
        }
        if (obj is string)
        {
            text = ((string)obj).AsSpan();
            return true;
        }
        if (obj is char[])
        {
            text = ((char[])obj).AsSpan();
            return true;
        }

        text = default;
        return false;
    }

#endregion

    public static int LevenshteinDistance(text source, text target)
    {
        int sourceLen = source.Length;
        int targetLen = target.Length;

        // Fast check for either being empty
        if (sourceLen == 0)
            return targetLen;
        if (targetLen == 0)
            return sourceLen;

//        // remove any common starting ending characters
//        var startIndex = 0;
//        while (startIndex < sourceLen &&
//            startIndex < targetLen &&
//            source[startIndex] == target[startIndex])
//        {
//            startIndex++;
//        }
//
//        // fast check for ending
//        if (startIndex >= sourceLen || startIndex >= targetLen)
//        {
//            return Math.Abs(sourceLen - targetLen);
//        }
//
//        // remove any common ending characters
//        while (startIndex < sourceLen &&
//            startIndex < targetLen &&
//            source[sourceLen - 1] == target[targetLen - 1])
//        {
//            sourceLen--;
//            targetLen--;
//        }
//
//        if (sourceLen)
//
//        var sourceLength = sourceLen - startIndex;
//        var targetLength = targetLen - startIndex;
//
//        source = source.Slice(startIndex, sourceLength);
//        target = target.Slice(startIndex, targetLength);

        Span<int> previousRow = stackalloc int[target.Length + 1];

        for (int i = 1; i <= targetLen; ++i)
        {
            previousRow[i] = i;
        }

        for (int i = 1; i <= sourceLen; ++i)
        {
            int previousDiagonal = previousRow[0];
            int previousColumn = previousRow[0]++;
            char sourceChar = source[i - 1];

            for (int j = 1; j <= targetLen; ++j)
            {
                int localCost = previousDiagonal;
                int deletionCost = previousRow[j];
                if (sourceChar != target[j - 1])
                {
                    // The conditional jumps associated with Math.Min only execute
                    // if the source character is not equal to the target character.
                    localCost = Math.Min(previousColumn, localCost);
                    localCost = Math.Min(deletionCost, localCost);
                    localCost++;
                }
                previousColumn = localCost;
                previousRow[j] = localCost;
                previousDiagonal = deletionCost;
            }
        }

        return previousRow[target.Length];
    }
}