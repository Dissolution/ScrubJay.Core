#pragma warning disable CA1715, CA1716




namespace ScrubJay.Validation;

/// <summary>
/// Methods that throw an <seealso cref="Exception"/> if certain conditions are met
/// </summary>
/// <remarks>
/// All methods follow a similar pattern:<br/>
/// (void | T)  - Return type, will be a T if T was expensive to construct<br/>
/// IfXXX       - Method name, will describe the <em>final</em> validation performed<br/>
/// T instance  - The instance | argument being validated<br/>
/// N (0+)      - Additional arguments needed for validation, they <em>will not</em> be validated<br/>
/// <c>string? info = null</c> - Additional information that can be provided to any thrown <see cref="Exception"/>'s <see cref="Exception.Message"/><br/>
/// <c>[CallerArgumentExpression(nameof(instance))] string? instanceName = null</c> - Automatically captured name of the instance being validated
/// </remarks>
[PublicAPI]
[StackTraceHidden]
public static partial class Throw
{
#region IfNull

    /// <summary>
    /// Throw an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is <c>null</c>
    /// </summary>
    public static void IfNull<T>(
        [NotNull, NoEnumeration] T? argument,
        InterpolatedTextBuilder info = default,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
        where T : class?
    {
        if (argument is null)
            throw Ex.ArgNull(argument, info, null, argumentName);
    }

    /// <summary>
    /// Throw an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is <c>null</c>
    /// </summary>
    public static void IfNull<T>(
        // ReSharper disable once ConvertNullableToShortForm
        [NotNull] Nullable<T> argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
        where T : struct
    {
        if (!argument.HasValue)
            throw Ex.ArgNull(argument, info, null, argumentName);
    }

#endregion


#region Equatable

    public static void IfEqual<T>(T? argument, T? other,
        IEqualityComparer<T>? comparer = null,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        if (!Equate.Values(argument, other, comparer))
            return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,
            $"must not be equal to `{other:@}`");


        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfNotEqual<T>(T? argument, T? other,
        IEqualityComparer<T>? comparer = null,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        if (Equate.Values(argument, other, comparer))
            return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,
            $"must be equal to `{other:@}`");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

#endregion

#region Comparable

    public static void IfLessThan<T>(T argument, T other,
        IComparer<T>? comparer = null,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        if (Compare.Values(argument, other, comparer) >= 0)
            return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,
            $"must be >= {other:@}");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfLessOrEqualThan<T>(T argument, T other,
        IComparer<T>? comparer = null,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        if (Compare.Values(argument, other, comparer) > 0)
            return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,
            $"must be <= {other:@}");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfGreaterThan<T>(T argument, T other,
        IComparer<T>? comparer = null,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        if (Compare.Values(argument, other, comparer) <= 0)
            return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,
            $"must be > {other:@}");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfGreaterOrEqualThan<T>(T argument, T other,
        IComparer<T>? comparer = null,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        if (Compare.Values(argument, other, comparer) < 0)
            return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,
            $"must be >= {other:@}");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

#endregion

#region Numeric Types

#if !NET7_0_OR_GREATER
    public static void IfZero<U>(U argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
        where U : unmanaged
    {
        if (Notsafe.Unmanaged.IsZero(argument))
        {
            string message = Ex.GetArgExceptionMessage(
                argument, argumentName, info,
                "must be non-zero");
            throw new ArgumentOutOfRangeException(argumentName, argument, message);
        }
    }

#else
    public static void IfZero<N>(
        N argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
        where N : INumberBase<N>
    {
        if (!N.IsZero(argument)) return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,"must be non-zero");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfNegative<N>(
        N argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
        where N : INumberBase<N>
    {
        if (!N.IsNegative(argument)) return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,"must be positive");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfNegativeOrZero<N>(
        N argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
        where N : INumberBase<N>
    {
        if (!N.IsNegative(argument) && !N.IsZero(argument)) return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,"must be positive and non-zero");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfPositive<N>(
        N argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
        where N : INumberBase<N>
    {
        if (!N.IsPositive(argument)) return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,"must be negative");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfPositiveOrZero<N>(
        N argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
        where N : INumberBase<N>
    {
        if (!N.IsPositive(argument) && !N.IsZero(argument)) return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info, "must be negative and non-zero");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }
#endif

#endregion

#region Collections

#region IfEmpty

    public static void IfEmpty<T>([NotNull] T[]? argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        IfNull(argument, info, argumentName);

        if (argument.Length > 0) return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info, "must not be empty");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfEmpty<T>(ReadOnlySpan<T> argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        if (argument.Length > 0) return;

#if NET9_0_OR_GREATER
        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,"must not be empty");
#else
        string message = Ex.GetArgExceptionMessage(
            typeof(ReadOnlySpan<T>), argument.Render(), argumentName, info,
            "must not be empty");
#endif

        throw new ArgumentOutOfRangeException(argumentName, typeof(ReadOnlySpan<T>).Render(), message);
    }

    public static void IfEmpty<T>(Span<T> argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        if (argument.Length > 0) return;

#if NET9_0_OR_GREATER
        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info, "must not be empty");
#else
        string message = Ex.GetArgExceptionMessage(
            typeof(Span<T>), argument.Render(), argumentName, info,
            "must not be empty");
#endif
        throw new ArgumentOutOfRangeException(argumentName, typeof(Span<T>).Render(), message);
    }

    public static void IfEmpty<T>([NotNull] ICollection<T>? argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        IfNull(argument, info, argumentName);

        if (argument.Count > 0) return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info, "must not be empty");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

#endregion

#region IfNotDistinct

    public static HashSet<T> IfNotDistinct<T>(
        scoped ReadOnlySpan<T> argument,
        IEqualityComparer<T>? itemComparer = null,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        var set = new HashSet<T>(itemComparer);
        for (int i = argument.Length - 1; i >= 0; i--)
        {
            if (!set.Add(argument[i]))
            {
#if NET9_0_OR_GREATER
                string message = Ex.GetArgExceptionMessage(
                    argument, argumentName, info, "must be distinct");
#else
                string message = Ex.GetArgExceptionMessage(
                    typeof(ReadOnlySpan<T>), argument.Render(), argumentName, info,
                    "must be distinct");
#endif
                throw Ex.Argument(argument, message, null, argumentName);
            }
        }

        return set;
    }

    public static HashSet<T> IfNotDistinct<T>(
        Span<T> argument,
        IEqualityComparer<T>? itemComparer = null,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        var set = new HashSet<T>(itemComparer);
        for (int i = argument.Length - 1; i >= 0; i--)
        {
            if (!set.Add(argument[i]))
            {
#if NET9_0_OR_GREATER
                string message = Ex.GetArgExceptionMessage(
                    argument, argumentName, info, "must be distinct");
#else
                string message = Ex.GetArgExceptionMessage(
                    typeof(Span<T>), argument.Render(), argumentName, info,
                    "must be distinct");
#endif
                throw Ex.Argument(argument, message, null, argumentName);
            }
        }

        return set;
    }

    public static HashSet<T> IfNotDistinct<T>(
        T[]? argument,
        IEqualityComparer<T>? itemComparer = null,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        IfNull(argument, info, argumentName);

        var set = new HashSet<T>(itemComparer);
        for (int i = argument.Length - 1; i >= 0; i--)
        {
            if (!set.Add(argument[i]))
            {
                string message = Ex.GetArgExceptionMessage(
                    argument, argumentName, info, "must be distinct");
                throw Ex.Argument(argument, message, null, argumentName);
            }
        }

        return set;
    }

    public static HashSet<T> IfNotDistinct<T>(
        IEnumerable<T> argument,
        IEqualityComparer<T>? itemComparer = null,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        IfNull(argument, info, argumentName);

        var set = new HashSet<T>(itemComparer);
        foreach (var value in argument)
        {
            if (!set.Add(value))
            {
                string message = Ex.GetArgExceptionMessage(
                    argument, argumentName, info, "must be distinct");
                throw Ex.Argument(argument, message, null, argumentName);
            }
        }

        return set;
    }

#endregion

#endregion

#region Text

    public static void IfEmpty([NotNull] string? argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        IfNull(argument, info, argumentName);

        if (argument.Length > 0) return;

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info, "must not be empty");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfEmpty(text argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        if (argument.Length != 0) return;

#if NET9_0_OR_GREATER
        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info, "must not be empty");
#else
        string message = Ex.GetArgExceptionMessage(
            typeof(text), argument.AsString(), argumentName, info,
            "must not be empty");
#endif

        throw new ArgumentOutOfRangeException(argumentName, argument.AsString(), message);
    }


    public static void IfWhitespace([NotNull] string? argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        IfNull(argument, info, argumentName);
        IfEmpty(argument, info, argumentName);

        for (int i = argument.Length - 1; i >= 0; i--)
        {
            if (!char.IsWhiteSpace(argument[i]))
                return;
        }

        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,
            "must contain at least one non-whitespace character");

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }

    public static void IfWhitespace(text argument,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        IfEmpty(argument, info, argumentName);

        for (int i = 0; i < argument.Length; i++)
        {
            if (!char.IsWhiteSpace(argument[i]))
                return;
        }

#if NET9_0_OR_GREATER
        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info,
            "must contain at least one non-whitespace character");
#else
        string message = Ex.GetArgExceptionMessage(
            typeof(text), argument.AsString(), argumentName, info,
            "must contain at least one non-whitespace character");
#endif
        throw new ArgumentOutOfRangeException(argumentName, argument.AsString(), message);
    }

#endregion

#region Bad Index/Range

    /// <summary>
    ///
    /// </summary>
    /// <param name="index"></param>
    /// <param name="available"></param>
    /// <param name="info"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static int IfBadIndex(
        int index,
        int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if ((uint)index < (uint)available)
            return index;

        string message = Ex.GetArgExceptionMessage(
            index, indexName, info,
            $"must be in [0..{available})");

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static int IfBadIndex(
        Index index,
        int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(available);

        if ((uint)offset < (uint)available)
            return offset;

        string message = Ex.GetArgExceptionMessage(
            index, indexName, info,
            $"must be in [0..{available})");

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static int IfBadInsertIndex(
        int index, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if ((uint)index <= (uint)available)
            return index;

        string message = Ex.GetArgExceptionMessage(
            index, indexName, info,
            $"must be in [0..{available}]");

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static int IfBadInsertIndex(
        Index index, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(available);

        if ((uint)offset <= (uint)available)
            return offset;

        string message = Ex.GetArgExceptionMessage(
            index, indexName, info,
            $"must be in [0..{available}]");

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static (int Offset, int Length) IfBadRange(
        int index,
        int length,
        int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null,
        [CallerArgumentExpression(nameof(length))]
        string? lengthName = null)
    {
        IfBadInsertIndex(index, available, info, indexName);
        IfLessOrEqualThan(length, 0, info: info, argumentName: lengthName);

        if ((index + length) <= available)
            return (index, length);

        string message = Ex.GetArgExceptionMessage(
            index, indexName, info,
            $"must be in [0..{available}]");

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static (int Offset, int Length) IfBadRange(
        Index index, int length, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null,
        [CallerArgumentExpression(nameof(length))]
        string? lengthName = null)
    {
        int offset = IfBadInsertIndex(index, available, info, indexName);
        IfLessOrEqualThan(length, 0, info: info, argumentName: lengthName);

        if ((offset + length) <= available)
            return (offset, length);

        string message = Ex.GetArgExceptionMessage(
            index, indexName, info,
            $"must be in [0..{available}]");

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static (int Offset, int Length) IfBadRange(
        Range range, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(range))]
        string? rangeName = null)
    {
        int start = range.Start.GetOffset(available);
        int s = IfBadInsertIndex(start, available, info, rangeName);

        int end = range.End.GetOffset(available);
        if ((end >= s) && (end <= available))
            return (s, end - s);

        string message = Ex.GetArgExceptionMessage(
            range, rangeName, info,
            $"must be in [0..{available})");

        throw new ArgumentOutOfRangeException(rangeName, range, message);
    }

#endregion

    public static T IfNotBetween<T>(T argument,
        T lowerBound,
        T upperBound,
        bool lowerBoundIsInclusive = true,
        bool upperBoundIsInclusive = false,
        string? info = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
        where T : IComparable<T>
    {
        int l = Comparer<T>.Default.Compare(argument, lowerBound);
        if (lowerBoundIsInclusive)
        {
            if (l < 0)
                goto FAIL;
        }
        else
        {
            if (l <= 0)
                goto FAIL;
        }

        int u = Comparer<T>.Default.Compare(argument, upperBound);
        if (upperBoundIsInclusive)
        {
            if (u > 0)
                goto FAIL;
        }
        else
        {
            if (u >= 0)
                goto FAIL;
        }

        return argument;

        FAIL:

        // string message = GetArgumentExceptionMessage(argument, info, argumentName, tb =>
        // {
        //     tb.Append("must be in ")
        //         .If(lowerBoundIsInclusive, '[', '(')
        //         .Format(upperBound)
        //         .Append("..")
        //         .If(upperBoundIsInclusive, ']', ')');
        // });
        string message = Ex.GetArgExceptionMessage(
            argument, argumentName, info);

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }


    /// <summary>
    /// Throw a <see cref="ObjectDisposedException"/> if a <paramref name="condition"/> indicates disposal of an <paramref name="argument"/>
    /// </summary>
    [StackTraceHidden]
    public static void IfDisposed<T>([DoesNotReturnIf(true)] bool condition, T? argument)
    {
        if (condition)
        {
            string? objectName;
            if (argument is null)
            {
                objectName = $"({typeof(T).Render()})null";
            }
            else
            {
                objectName = argument.GetType().Render();
            }

            throw new ObjectDisposedException(objectName);
        }
    }

#region Enumeration

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> that indicates an <see cref="IEnumerator{T}"/> has deviated from its source
    /// </summary>
    /// <param name="versionHasChanged"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [StackTraceHidden]
    public static void IfBadEnumerationVersion(
        [DoesNotReturnIf(true)] bool versionHasChanged)
    {
        if (versionHasChanged)
        {
            throw Ex.Invalid("Enumeration Failed: Source has changed");
        }
    }

    [StackTraceHidden]
    public static void IfBadEnumerationVersion(int oldVersion, int newVersion)
    {
        if (newVersion != oldVersion)
        {
            throw Ex.Invalid("Enumeration Failed: Source has changed");
        }
    }

    [StackTraceHidden]
    public static void IfBadEnumerationState(
        [DoesNotReturnIf(true)] bool badState)
    {
        if (badState)
            throw Ex.Invalid("Enumeration has not yet started or has finished");
    }

    [StackTraceHidden]
    public static void IfBadEnumerationState(
        [DoesNotReturnIf(true)] bool notYetStarted,
        [DoesNotReturnIf(true)] bool hasFinished)
    {
        if (notYetStarted)
            throw Ex.Invalid("Enumeration has not yet started");
        if (hasFinished)
            throw Ex.Invalid("Enumeration has finished");
    }

#endregion
}