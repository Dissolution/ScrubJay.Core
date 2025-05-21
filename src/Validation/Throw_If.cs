#pragma warning disable CA1715, CA1716

using ScrubJay.Text.Rendering;


namespace ScrubJay.Validation;

/// <summary>
/// Methods that throw an <seealso cref="Exception"/> if certain conditions are met
/// </summary>
[PublicAPI]
[StackTraceHidden]
public static partial class Throw
{
    private static string GetMessage<T>(
        T instance,
        string? info,
        string? instanceName,
        string message)
    {
        return TextBuilder.New
            .Render(typeof(T))
            .Append(" '")
            .Append(instanceName)
            .Append("' `")
            .Render(instance)
            .Append("` ")
            .Append(message)
            .IfNotNull(info,
                static (tb, nfo) => tb.Append(": ").Append(nfo))
            .ToStringAndDispose();
    }

    private static string GetMessage<T>(
        ReadOnlySpan<T> span,
        string? info,
        string? instanceName,
        string message)
    {
        return TextBuilder.New
            .Render(typeof(T))
            .Append(" '")
            .Append(instanceName)
            .Append("' `")
            .Render(span)
            .Append("` ")
            .Append(message)
            .IfNotNull(info,
                static (tb, nfo) => tb.Append(": ").Append(nfo))
            .ToStringAndDispose();
    }

    private static string GetMessage<T>(
        T instance,
        string? info,
        string? instanceName,
        Action<TextBuilder> appendMessage)
    {
        return TextBuilder.New
            .Render(typeof(T))
            .Append(" '")
            .Append(instanceName)
            .Append("' `")
            .Render(instance)
            .Append("` ")
            .Invoke(appendMessage)
            .IfNotNull(info,
                static (tb, nfo) => tb.Append(": ").Append(nfo))
            .ToStringAndDispose();
    }


    /// <summary>
    /// Throw an <see cref="ArgumentNullException"/> if <paramref name="value"/> is <c>null</c>
    /// </summary>
    [StackTraceHidden]
    public static void IfNull<T>(
        [NotNull, NoEnumeration] T? value,
        string? message = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        where T : class?
    {
        if (value is null)
            throw new ArgumentNullException(valueName, message);
    }

    public static void IfNull<T>(
        // ReSharper disable once ConvertNullableToShortForm
        [NotNull] Nullable<T> nullable,
        string? message = null,
        [CallerArgumentExpression(nameof(nullable))]
        string? nullableName = null)
        where T : struct
    {
        if (!nullable.HasValue)
            throw new ArgumentNullException(nullableName, message);
    }

#region Equatable
    public static void IfEqual<T>(T? value, T? other,
        string? info = null,
        IEqualityComparer<T>? comparer = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        comparer ??= EqualityComparer<T>.Default;
        if (!comparer.Equals(value!, other!)) return;

        string message = GetMessage(value, info, valueName, tb =>
        {
            tb.Append("must not be equal to `")
                .Render(other)
                .Append('`');
        });

        throw new ArgumentOutOfRangeException(valueName, value, message);
    }

    public static void IfNotEqual<T>(T? value, T? other,
        string? info = null,
        IEqualityComparer<T>? comparer = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        comparer ??= EqualityComparer<T>.Default;
        if (comparer.Equals(value!, other!)) return;

        string message = GetMessage(value, info, valueName, tb =>
        {
            tb.Append("must be equal to `")
                .Render(other)
                .Append('`');
        });

        throw new ArgumentOutOfRangeException(valueName, value, message);
    }
#endregion

#region Comparable
    public static void IfLessThan<T>(T value, T other,
        string? info = null,
        IComparer<T>? comparer = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        comparer ??= Comparer<T>.Default;
        int c = comparer.Compare(value, other);
        if (c >= 0) return;

        string message = GetMessage(value, info, valueName, tb =>
        {
            tb.Append("must be greater than or equal to `")
                .Render(other)
                .Append('`');
        });

        throw new ArgumentOutOfRangeException(valueName, value, message);
    }

    public static void IfLessOrEqualThan<T>(T value, T other,
        string? info = null,
        IComparer<T>? comparer = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        comparer ??= Comparer<T>.Default;
        int c = comparer.Compare(value, other);
        if (c > 0) return;

        string message = GetMessage(value, info, valueName, tb =>
        {
            tb.Append("must be greater than `")
                .Render(other)
                .Append('`');
        });

        throw new ArgumentOutOfRangeException(valueName, value, message);
    }

    public static void IfGreaterThan<T>(T value, T other,
        string? info = null,
        IComparer<T>? comparer = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        comparer ??= Comparer<T>.Default;
        int c = comparer.Compare(value, other);
        if (c <= 0) return;

        string message = GetMessage(value, info, valueName, tb =>
        {
            tb.Append("must be less than or equal to `")
                .Render(other)
                .Append('`');
        });

        throw new ArgumentOutOfRangeException(valueName, value, message);
    }

    public static void IfGreaterOrEqualThan<T>(T value, T other,
        string? info = null,
        IComparer<T>? comparer = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        comparer ??= Comparer<T>.Default;
        int c = comparer.Compare(value, other);
        if (c < 0) return;

        string message = GetMessage(value, info, valueName, tb =>
        {
            tb.Append("must be less than `")
                .Render(other)
                .Append('`');
        });

        throw new ArgumentOutOfRangeException(valueName, value, message);
    }
#endregion




#region Numeric Types
#if !NET7_0_OR_GREATER
    public static void IfZero<U>(U value,
        string? info = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        where U : unmanaged
    {
        if (Notsafe.Unmanaged.IsZero(value))
        {
            string message = GetMessage(value, info, valueName, "must be non-zero");
            throw new ArgumentOutOfRangeException(valueName, value, message);
        }
    }

#else
    public static void IfZero<N>(
        N number,
        string? info = null,
        [CallerArgumentExpression(nameof(number))]
        string? argumentName = null)
        where N : INumberBase<N>
    {
        if (!N.IsZero(number)) return;

        string message = GetMessage(number, info, argumentName, "must be non-zero");

        throw new ArgumentOutOfRangeException(argumentName, number, message);
    }

    public static void IfNegative<N>(
        N number,
        string? info = null,
        [CallerArgumentExpression(nameof(number))]
        string? argumentName = null)
        where N : INumberBase<N>
    {
        if (!N.IsNegative(number)) return;

        string message = GetMessage(number, info, argumentName, "must be positive");

        throw new ArgumentOutOfRangeException(argumentName, number, message);
    }

    public static void IfNegativeOrZero<N>(
        N number,
        string? info = null,
        [CallerArgumentExpression(nameof(number))]
        string? argumentName = null)
        where N : INumberBase<N>
    {
        if (!N.IsNegative(number) && !N.IsZero(number)) return;

        string message = GetMessage(number, info, argumentName, "must be positive and non-zero");

        throw new ArgumentOutOfRangeException(argumentName, number, message);
    }

    public static void IfPositive<N>(
        N number,
        string? info = null,
        [CallerArgumentExpression(nameof(number))]
        string? argumentName = null)
        where N : INumberBase<N>
    {
        if (!N.IsPositive(number)) return;

        string message = GetMessage(number, info, argumentName, "must be negative");

        throw new ArgumentOutOfRangeException(argumentName, number, message);
    }

    public static void IfPositiveOrZero<N>(
        N number,
        string? info = null,
        [CallerArgumentExpression(nameof(number))]
        string? argumentName = null)
        where N : INumberBase<N>
    {
        if (!N.IsPositive(number) && !N.IsZero(number)) return;

        string message = GetMessage(number, info, argumentName, "must be negative and non-zero");

        throw new ArgumentOutOfRangeException(argumentName, number, message);
    }
#endif
#endregion

#region Collections
    public static void IfEmpty<T>([NotNull] T[]? array,
        string? info = null,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null)
    {
        IfNull(array, info, arrayName);

        if (array.Length > 0) return;

        string message = GetMessage(array, info, arrayName, "must not be empty");

        throw new ArgumentOutOfRangeException(arrayName, array, message);
    }

    public static void IfEmpty<T>(ReadOnlySpan<T> span,
        string? info = null,
        [CallerArgumentExpression(nameof(span))]
        string? spanName = null)
    {
        if (span.Length > 0) return;

        string message = GetMessage(span, info, spanName, "must not be empty");

        throw new ArgumentOutOfRangeException(spanName, typeof(ReadOnlySpan<T>).NameOf(), message);
    }

    public static void IfEmpty<T>(Span<T> span,
        string? info = null,
        [CallerArgumentExpression(nameof(span))]
        string? spanName = null)
    {
        if (span.Length > 0) return;

        string message = GetMessage((ReadOnlySpan<T>)span, info, spanName, "must not be empty");

        throw new ArgumentOutOfRangeException(spanName, typeof(Span<T>).NameOf(), message);
    }

    public static void IfEmpty<T>([NotNull] ICollection<T>? collection,
        string? info = null,
        [CallerArgumentExpression(nameof(collection))]
        string? collectionName = null)
    {
        IfNull(collection, info, collectionName);

        if (collection.Count > 0) return;

        string message = GetMessage(collection, info, collectionName, "must not be empty");

        throw new ArgumentOutOfRangeException(collectionName, collection, message);
    }
#endregion

#region Text
    public static void IfEmpty([NotNull] string? str,
        string? info = null,
        [CallerArgumentExpression(nameof(str))]
        string? strName = null)
    {
        IfNull(str, info, strName);

        if (str.Length > 0) return;

        string message = GetMessage(str, info, strName, "must not be empty");

        throw new ArgumentOutOfRangeException(strName, str, message);
    }

    public static void IfEmpty(text text,
        string? info = null,
        [CallerArgumentExpression(nameof(text))]
        string? textName = null)
    {
        if (text.Length != 0) return;

        string message = GetMessage(text, info, textName, "must not be empty");

        throw new ArgumentOutOfRangeException(textName, text.AsString(), message);
    }


    public static void IfWhitespace([NotNull] string? str,
        string? info = null,
        [CallerArgumentExpression(nameof(str))]
        string? strName = null)
    {
        IfNull(str, info, strName);
        IfEmpty(str, info, strName);

        for (int i = str.Length - 1; i >= 0; i--)
        {
            if (!char.IsWhiteSpace(str[i]))
                return;
        }

        string message = GetMessage(str, info, strName, "must contain at least one non-whitespace character");

        throw new ArgumentOutOfRangeException(strName, str, message);
    }

    public static void IfWhitespace(text text,
        string? info = null,
        [CallerArgumentExpression(nameof(text))]
        string? textName = null)
    {
        int len = text.Length;
        if (len == 0)
            throw new ArgumentOutOfRangeException(textName, text.AsString(), info ?? "Argument must not be empty");


        for (int i = 0; i < len; i++)
        {
            if (!char.IsWhiteSpace(text[i]))
                return;
        }

        throw new ArgumentOutOfRangeException(textName, text.AsString(),
            info ?? "Argument must contain at least one non-whitespace character");
    }
#endregion

#region If Bad
    public static int IfBadIndex(int index, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if ((uint)index < (uint)available)
            return index;

        string message = GetMessage(index, info, indexName, tb =>
        {
            tb.Append("must be in [0..")
                .Append(available)
                .Append(')');
        });

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static int IfBadIndex(Index index, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(available);

        if ((uint)offset < (uint)available)
            return offset;

        string message = GetMessage(index, info, indexName, tb =>
        {
            tb.Append("must be in [0..")
                .Append(available)
                .Append(')');
        });

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static int IfBadInsertIndex(int index, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if ((uint)index <= (uint)available)
            return index;

        string message = GetMessage(index, info, indexName, tb =>
        {
            tb.Append("must be in [0..")
                .Append(available)
                .Append(']');
        });

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static int IfBadInsertIndex(Index index, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(available);

        if ((uint)offset <= (uint)available)
            return offset;

        string message = GetMessage(index, info, indexName, tb =>
        {
            tb.Append("must be in [0..")
                .Append(available)
                .Append(']');
        });

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static (int Offset, int Length) IfBadRange(int index, int length, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null,
        [CallerArgumentExpression(nameof(length))]
        string? lengthName = null)
    {
        IfBadInsertIndex(index, available, info, indexName);
        IfLessOrEqualThan(length, 0, info, valueName: lengthName);

        if ((index + length) <= available)
            return (index, length);

        string message = GetMessage(length, info, lengthName,
            tb => { tb.Append($"+ '{indexName}' `{index}` must be in [0..{available}]"); });

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static (int Offset, int Length) IfBadRange(Index index, int length, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null,
        [CallerArgumentExpression(nameof(length))]
        string? lengthName = null)
    {
        int offset = IfBadInsertIndex(index, available, info, indexName);
        IfLessOrEqualThan(length, 0, info, valueName: lengthName);

        if ((offset + length) <= available)
            return (offset, length);

        string message = GetMessage(length, info, lengthName,
            tb => { tb.Append($"+ '{indexName}' `{index}` must be in [0..{available}]"); });

        throw new ArgumentOutOfRangeException(indexName, index, message);
    }

    public static (int Offset, int Length) IfBadRange(Range range, int available,
        string? info = null,
        [CallerArgumentExpression(nameof(range))]
        string? rangeName = null)
    {
        int start = range.Start.GetOffset(available);
        int s = IfBadInsertIndex(start, available, info, rangeName);

        int end = range.End.GetOffset(available);
        if ((end >= s) && (end <= available))
            return (s, end - s);

        string message = GetMessage(range, info, rangeName,
            tb => { tb.Append($"must be in [0..{available})"); });

        throw new ArgumentOutOfRangeException(rangeName, range, message);
    }
#endregion

    public static void IfNotBetween<T>(T argument,
        T lowerBound,
        T upperBound,
        string? info = null,
        bool lowerBoundIsInclusive = true,
        bool upperBoundIsInclusive = false,
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

        return;

        FAIL:

        string message = GetMessage(argument, info, argumentName, tb =>
        {
            tb.Append("must be in ")
                .AppendIf(lowerBoundIsInclusive, '[', '(')
                .Append(upperBound)
                .Append("..")
                .AppendIf(upperBoundIsInclusive, ']', ')');
        });

        throw new ArgumentOutOfRangeException(argumentName, argument, message);
    }


    /// <summary>
    /// Throw a <see cref="ObjectDisposedException"/> if a <paramref name="condition"/> indicates disposal of an <paramref name="instance"/>
    /// </summary>
    [StackTraceHidden]
    public static void IfDisposed<T>([DoesNotReturnIf(true)] bool condition, T? instance)
    {
        if (condition)
        {
            string? objectName;
            if (instance is null)
            {
                objectName = $"({typeof(T).NameOf()})null";
            }
            else
            {
                objectName = instance.GetType().NameOf();
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
            throw new InvalidOperationException("Enumeration Failed: Source has changed");
        }
    }

    [StackTraceHidden]
    public static void IfBadEnumerationVersion(int oldVersion, int newVersion)
    {
        if (newVersion != oldVersion)
        {
            throw new InvalidOperationException("Enumeration Failed: Source has changed");
        }
    }

    [StackTraceHidden]
    public static void IfBadEnumerationState(
        [DoesNotReturnIf(true)] bool badState)
    {
        if (badState)
            throw new InvalidOperationException("Enumeration has not yet started or has finished");
    }

    [StackTraceHidden]
    public static void IfBadEnumerationState(
        [DoesNotReturnIf(true)] bool notYetStarted,
        [DoesNotReturnIf(true)] bool hasFinished)
    {
        if (notYetStarted)
            throw new InvalidOperationException("Enumeration has not yet started");
        if (hasFinished)
            throw new InvalidOperationException("Enumeration has finished");
    }
#endregion
}