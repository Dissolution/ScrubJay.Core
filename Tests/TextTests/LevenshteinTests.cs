using ScrubJay.Maths;
using ScrubJay.Text;
using ScrubJay.Utilities;

namespace ScrubJay.Tests.TextTests;

public class LevenshteinTests
{
    public static TheoryData<string, string, int> TestData { get; } = new()
    {
        // insert
        { "will", "william", 3 }, // back
        { "port", "support", 3 }, // front
        { "ant", "aunt", 1 }, // middle
        { "", "14710", 5 },

        // remove
        { "helicopter", "helic", 5 }, // back
        { "principal", "pal", 6 }, // front
        { "together", "get", 5 }, // middle
        { "14710", "", 5 },

        // replace
        { "quest", "quint", 2 }, // back
        { "sister", "mister", 1 }, // front
        { "brazen", "broken", 2 }, // middle
        { "fast", "cats", 3 },

        // combination
        { "kitten", "sitting", 3 },
        { "we", "they", 3 },
        { "ghoti", "fish", 5 },

        // same
        { "TJ", "TJ", 0 },
        { "yogurt", "yogurt", 0 },
        { "", "", 0 },
    };

    [Theory]
    [MemberData(nameof(TestData))]
    public void RecursiveWorks(string s1, string s2, int expectedDistance)
    {
        var levdist = LevenshteinImplementations.LevenshteinRecursive(s1, s2);
        Assert.Equal(expectedDistance, levdist);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void FullMatrixWorks(string s1, string s2, int expectedDistance)
    {
        var levdist = LevenshteinImplementations.LevenshteinFullMatrix(s1, s2);
        Assert.Equal(expectedDistance, levdist);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void TwoMatrixRowsWorks(string s1, string s2, int expectedDistance)
    {
        var levdist = LevenshteinImplementations.LevenshteinTwoMatrixRows(s1, s2);
        Assert.Equal(expectedDistance, levdist);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void DistanceWorks(string s1, string s2, int expectedDistance)
    {
        var levdist = LevenshteinImplementations.Distance(s1, s2);
        Assert.Equal(expectedDistance, levdist);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void CalculateDistanceAWorks(string s1, string s2, int expectedDistance)
    {
        var levdist = LevenshteinImplementations.CalculateDistanceA(s1, s2);
        Assert.Equal(expectedDistance, levdist);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void CalculateDistanceBWorks(string s1, string s2, int expectedDistance)
    {
        var levdist = LevenshteinImplementations.CalculateDistanceB(s1, s2);
        Assert.Equal(expectedDistance, levdist);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void CalculateDistanceCWorks(string s1, string s2, int expectedDistance)
    {
        var levdist = LevenshteinImplementations.CalculateDistanceC(s1, s2);
        Assert.Equal(expectedDistance, levdist);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void CalculateDistanceDWorks(string s1, string s2, int expectedDistance)
    {
        var levdist = LevenshteinImplementations.CalculateDistanceD(s1.AsSpan(), s2.AsSpan());
        Assert.Equal(expectedDistance, levdist);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void TextHelperLevenshteinDistanceWorks(string s1, string s2, int expectedDistance)
    {
        var levdist = TextHelper.LevenshteinDistance(s1.AsSpan(), s2.AsSpan());
        Assert.Equal(expectedDistance, levdist);
    }
}

internal static class LevenshteinImplementations
{
#region Levenshtein
    public static int LevenshteinRecursive(string str1, string str2)
    {
        return LevenshteinRecursive(str1, str2, str1.Length, str2.Length);
    }

    /// <summary>
    /// Recursive function to calculate Levenshtein Distance
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <param name="m"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <remarks>
    /// Time complexity:        O(3^(m+n))
    /// Auxiliary complexity:   O(m+n)
    /// </remarks>
    private static int LevenshteinRecursive(string str1, string str2, int m, int n)
    {
        // If str1 is empty, the distance is the length of str2
        if (m == 0)
        {
            return n;
        }

        // If str2 is empty, the distance is the length of str1
        if (n == 0)
        {
            return m;
        }

        // If the last characters of the strings are the same
        if (str1[m - 1] == str2[n - 1])
        {
            return LevenshteinRecursive(str1, str2, m - 1, n - 1);
        }

        // Calculate the minimum of three operations:
        // Insert
        var insertLen = LevenshteinRecursive(str1, str2, m, n - 1);
        // Remove
        var removeLen = LevenshteinRecursive(str1, str2, m - 1, n);
        // Replace
        var replaceLen = LevenshteinRecursive(str1, str2, m - 1, n - 1);

        return 1 + MathHelper.Min(insertLen, removeLen, replaceLen);
    }


    /// <summary>
    /// Function to calculate Levenshtein Distance using a full matrix approach
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <returns></returns>
    /// <remarks>
    /// Time complexity:        O(m*n)
    /// Auxiliary complexity:   O(m*n)
    /// </remarks>
    public static int LevenshteinFullMatrix(string str1, string str2)
    {
        int m = str1.Length;
        int n = str2.Length;

        // Create a matrix to store distances
        // for all i and j, d[i,j] will hold the Levenshtein distance between
        // the first i characters of s and the first j characters of t
        int[,] dp = new int[m + 1, n + 1];

        // Initialize the first row and column of the matrix
        for (int i = 0; i <= m; i++)
        {
            dp[i, 0] = i; // Number of insertions required for str1 to become an empty string
        }

        for (int j = 0; j <= n; j++)
        {
            dp[0, j] = j; // Number of insertions required for an empty string to become str2
        }

        // Fill in the matrix with minimum edit distances
        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (str1[i - 1] == str2[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1]; // Characters match, no operation needed
                }
                else
                {
                    // Choose the minimum of insert, delete, or replace operations
                    dp[i, j] = 1 + MathHelper.Min(
                        dp[i, j - 1], // Insertion
                        dp[i - 1, j], // Deletion
                        dp[i - 1, j - 1] // Replacement
                    );
                }
            }
        }
        return dp[m, n]; // Return the final edit distance
    }


    /// <summary>
    /// Function to calculate Levenshtein distance between two strings
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    /// <remarks>
    /// Time complexity:        O(m*n)
    /// Auxiliary complexity:   O(n)
    /// </remarks>
    public static int LevenshteinTwoMatrixRows(string s, string t)
    {
        int m = s.Length;
        int n = t.Length;

        // two work buffers of integer distances
        Span<int> v0 = stackalloc int[n + 1];
        Span<int> v1 = stackalloc int[n + 1];

        // initialize v0 (the previous row of distances)
        // this row is A[0][i]: edit distance from an empty "s" to "t";
        // that distance is the number of characters to append to "s" to make "t"
        for (int j = 0; j <= n; j++)
        {
            v0[j] = j;
        }

        for (int i = 0; i < m; i++)
        {
            // calculate v1 (current row distances) from the previous row v0

            // first element of v1 is A[i + 1][0]
            //   edit distance is delete (i + 1) chars from s to match empty t
            v1[0] = i + 1;

            // use formula to fill in the rest of the row
            for (int j = 0; j < n; j++)
            {
                // calculating costs for A[i + 1][j + 1]
                var deletionCost = v0[j + 1] + 1;
                var insertionCost = v1[j] + 1;
                int substitutionCost;
                if (s[i] == t[j])
                {
                    substitutionCost = v0[j];
                }
                else
                {
                    substitutionCost = v0[j] + 1;
                }

                v1[j + 1] = MathHelper.Min(deletionCost, insertionCost, substitutionCost);

//                // If characters are the same, no operation is needed
//                if (s[i - 1] == t[j - 1])
//                {
//                    v1[j] = v0[j - 1];
//                }
//                else
//                {
//                    // Choose the minimum of three operations:
//                    // insert, remove, or replace
//                    v1[j] = 1 + MathHelper.Min(
//                        // Insert
//                        v1[j - 1],
//                        // Remove
//                        v0[j],
//                        // Replace
//                        v0[j - 1]);
//                }
            }

            // copy v1 (current row) to v0 (previous row) for next iteration
            // since data in v1 is always invalidated, a swap without copy could be more efficient
            Reference.Exchange<int>(ref v0, ref v1);
        }

        // after the last swap, the results of v1 are now in v0
        return v0[n];
    }



    /// <summary>
    /// Compares the two values to find the minimum Levenshtein distance.
    /// Thread safe.
    /// </summary>
    /// <returns>Difference. 0 complete match.</returns>
    public static int Distance(string value1, string value2)
    {
        if (value2.Length == 0)
        {
            return value1.Length;
        }

        int[] costs = new int[value2.Length];

        // Add indexing for insertion to first row
        for (int i = 0; i < costs.Length;)
        {
            costs[i] = ++i;
        }

        for (int i = 0; i < value1.Length; i++)
        {
            // cost of the first index
            int cost = i;
            int previousCost = i;

            // cache value for inner loop to avoid index lookup and bonds checking, profiled this is quicker
            char value1Char = value1[i];

            for (int j = 0; j < value2.Length; j++)
            {
                int currentCost = cost;

                // assigning this here reduces the array reads we do, improvement of the old version
                cost = costs[j];

                if (value1Char != value2[j])
                {
                    if (previousCost < currentCost)
                    {
                        currentCost = previousCost;
                    }

                    if (cost < currentCost)
                    {
                        currentCost = cost;
                    }

                    ++currentCost;
                }

                /*
                 * Improvement on the older versions.
                 * Swapping the variables here results in a performance improvement for modern intel CPU’s, but I have no idea why?
                 */
                costs[j] = currentCost;
                previousCost = currentCost;
            }
        }

        return costs[costs.Length - 1];
    }

    public static int CalculateDistanceA(string source, string target)
    {
        var costMatrix = Enumerable
            .Range(0, source.Length + 1)
            .Select(line => new int[target.Length + 1])
            .ToArray();

        for (var i = 1; i <= source.Length; ++i)
        {
            costMatrix[i][0] = i;
        }

        for (var i = 1; i <= target.Length; ++i)
        {
            costMatrix[0][i] = i;
        }

        for (var i = 1; i <= source.Length; ++i)
        {
            for (var j = 1; j <= target.Length; ++j)
            {
                var insert = costMatrix[i][j - 1] + 1;
                var delete = costMatrix[i - 1][j] + 1;
                var edit = costMatrix[i - 1][j - 1] + (source[i - 1] == target[j - 1] ? 0 : 1);

                costMatrix[i][j] = Math.Min(Math.Min(insert, delete), edit);
            }
        }

        return costMatrix[source.Length][target.Length];
    }

    public static int CalculateDistanceB(string source, string target)
    {
        var costMatrix = Enumerable
            .Range(0, 2)
            .Select(line => new int[target.Length + 1])
            .ToArray();

        for (var i = 1; i <= target.Length; ++i)
        {
            costMatrix[0][i] = i;
        }

        for (var i = 1; i <= source.Length; ++i)
        {
            costMatrix[i % 2][0] = i;

            for (var j = 1; j <= target.Length; ++j)
            {
                var insert = costMatrix[i % 2][j - 1] + 1;
                var delete = costMatrix[(i - 1) % 2][j] + 1;
                var edit = costMatrix[(i - 1) % 2][j - 1] + (source[i - 1] == target[j - 1] ? 0 : 1);

                costMatrix[i % 2][j] = Math.Min(Math.Min(insert, delete), edit);
            }
        }

        return costMatrix[source.Length % 2][target.Length];
    }

    public static int CalculateDistanceC(string source, string target)
    {
        var previousRow = new int[target.Length + 1];

        for (var i = 1; i <= target.Length; ++i)
        {
            previousRow[i] = i;
        }

        for (var i = 1; i <= source.Length; ++i)
        {
            var previousDiagonal = previousRow[0];
            var previousColumn = previousRow[0]++;

            for (var j = 1; j <= target.Length; ++j)
            {
                var insertOrDelete = Math.Min(previousColumn, previousRow[j]) + 1;
                var edit = previousDiagonal + (source[i - 1] == target[j - 1] ? 0 : 1);

                previousColumn = Math.Min(insertOrDelete, edit);
                previousDiagonal = previousRow[j];
                previousRow[j] = previousColumn;
            }
        }

        return previousRow[target.Length];
    }

    public static int CalculateDistanceD(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        var startIndex = 0;
        var sourceEnd = source.Length;
        var targetEnd = target.Length;

        while (startIndex < sourceEnd && startIndex < targetEnd && source[startIndex] == target[startIndex])
        {
            startIndex++;
        }
        while (startIndex < sourceEnd && startIndex < targetEnd && source[sourceEnd - 1] == target[targetEnd - 1])
        {
            sourceEnd--;
            targetEnd--;
        }

        var sourceLength = sourceEnd - startIndex;
        var targetLength = targetEnd - startIndex;

        source = source.Slice(startIndex, sourceLength);
        target = target.Slice(startIndex, targetLength);

        Span<int> previousRow = stackalloc int[target.Length + 1];

        for (var i = 1; i <= target.Length; ++i)
        {
            previousRow[i] = i;
        }

        for (var i = 1; i <= source.Length; ++i)
        {
            var previousDiagonal = previousRow[0];
            var previousColumn = previousRow[0]++;
            var sourceChar = source[i - 1];

            for (var j = 1; j <= target.Length; ++j)
            {
                var localCost = previousDiagonal;
                var deletionCost = previousRow[j];
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
#endregion
}