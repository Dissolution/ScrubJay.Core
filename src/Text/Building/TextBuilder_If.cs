namespace ScrubJay.Text;

partial class TextBuilder
{
#region If (bool)

#region Only the True Condition

#if NET9_0_OR_GREATER
    public TextBuilder If<T>(bool condition, T onTrue, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
    {
        if (condition)
        {
            return Append<T>(onTrue, _);
        }
        else
        {
            return this;
        }
    }

    public TextBuilder If<T>(bool condition, T onTrue, char format, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
    {
        if (condition)
        {
            return Append<T>(onTrue, format, _);
        }
        else
        {
            return this;
        }
    }
#endif

    public TextBuilder If<T>(bool condition, T onTrue)
    {
        if (condition)
        {
            return Append<T>(onTrue);
        }
        else
        {
            return this;
        }
    }

    public TextBuilder If<T>(bool condition, T onTrue, char format, IFormatProvider? provider = null)
    {
        if (condition)
        {
            return Append<T>(onTrue, format, provider);
        }
        else
        {
            return this;
        }
    }

    public TextBuilder If<T>(bool condition, T onTrue, scoped text format, IFormatProvider? provider = null)
    {
        if (condition)
        {
            return Append<T>(onTrue, format, provider);
        }
        else
        {
            return this;
        }
    }

    public TextBuilder If<T>(bool condition, T onTrue, string? format, IFormatProvider? provider = null)
    {
        if (condition)
        {
            return Append<T>(onTrue, format, provider);
        }
        else
        {
            return this;
        }
    }

    public TextBuilder If(bool condition, Action<TextBuilder>? onTrue)
    {
        if (condition)
        {
            return Invoke(onTrue);
        }
        else
        {
            return this;
        }
    }

#endregion / only true

#region True + False Conditions

#if NET9_0_OR_GREATER
    public TextBuilder If<T, F>(bool condition, T onTrue, F onFalse,
        GenericTypeConstraint.AllowsRefStruct<T> _ = default,
        GenericTypeConstraint.AllowsRefStruct<F> __ = default)
        where T : allows ref struct
        where F : allows ref struct
    {
        if (condition)
        {
            return Append<T>(onTrue);
        }
        else
        {
            return Append<F>(onFalse);
        }
    }

    public TextBuilder If<T, F>(bool condition, T onTrue, F onFalse,
        char format,
        GenericTypeConstraint.AllowsRefStruct<T> _ = default,
        GenericTypeConstraint.AllowsRefStruct<F> __ = default)
        where T : allows ref struct
        where F : allows ref struct
    {
        if (condition)
        {
            return Append<T>(onTrue, format, _);
        }
        else
        {
            return Append<F>(onFalse, format, __);
        }
    }
#endif

    public TextBuilder If<T, F>(bool condition, T onTrue, F onFalse)
    {
        if (condition)
        {
            return Append<T>(onTrue);
        }
        else
        {
            return Append<F>(onFalse);
        }
    }

    public TextBuilder If<T, F>(bool condition, T onTrue, F onFalse, char format, IFormatProvider? provider = null)
    {
        if (condition)
        {
            return Append<T>(onTrue, format, provider);
        }
        else
        {
            return Append<F>(onFalse, format, provider);
        }
    }

    public TextBuilder If<T, F>(bool condition, T onTrue, F onFalse, scoped text format, IFormatProvider? provider = null)
    {
        if (condition)
        {
            return Append<T>(onTrue, format, provider);
        }
        else
        {
            return Append<F>(onFalse, format, provider);
        }
    }

    public TextBuilder If<T, F>(bool condition, T onTrue, F onFalse, string? format, IFormatProvider? provider = null)
    {
        if (condition)
        {
            return Append<T>(onTrue, format, provider);
        }
        else
        {
            return Append<F>(onFalse, format, provider);
        }
    }

    public TextBuilder If(bool condition, Action<TextBuilder>? onTrue, Action<TextBuilder>? onFalse)
    {
        if (condition)
        {
            return Invoke(onTrue);
        }
        else
        {
            return Invoke(onFalse);
        }
    }

    public TextBuilder If<T>(bool condition, T onTrue, Action<TextBuilder>? onFalse)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (condition)
        {
            return Append<T>(onTrue);
        }
        else
        {
            return Invoke(onFalse);
        }
    }

    public TextBuilder If<F>(bool condition, Action<TextBuilder>? onTrue, F onFalse)
#if NET9_0_OR_GREATER
        where F : allows ref struct
#endif
    {
        if (condition)
        {
            return Invoke(onTrue);
        }
        else
        {
            return Append<F>(onFalse);
        }
    }

#endregion / only true

#endregion

    public TextBuilder If<T>(Option<T> option, Action<TextBuilder, T>? onSome = null,
        Action<TextBuilder>? onNone = null)
    {
        if (option.IsSome(out var some))
        {
            onSome?.Invoke(this, some);
        }
        else
        {
            onNone?.Invoke(this);
        }

        return this;
    }

    public TextBuilder If<T>(Result<T> result, Action<TextBuilder, T>? onOk = null,
        Action<TextBuilder, Exception>? onError = null)
    {
        if (result.IsOk(out var ok, out var error))
        {
            onOk?.Invoke(this, ok);
        }
        else
        {
            onError?.Invoke(this, error);
        }

        return this;
    }

    public TextBuilder If<T>(T value,
        Func<T, bool> predicate,
        Action<TextBuilder, T>? onTrue = null,
        Action<TextBuilder, T>? onFalse = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Throw.IfNull(predicate);
        if (predicate(value))
        {
            onTrue?.Invoke(this, value);
        }
        else
        {
            onFalse?.Invoke(this, value);
        }

        return this;
    }

#region IfNotNull

    public TextBuilder IfNotNull<T>(T? value, Action<TextBuilder, T>? onNotNull = null,
        Action<TextBuilder>? onNull = null)
    {
        if (value is not null)
        {
            onNotNull?.Invoke(this, value);
        }
        else
        {
            onNull?.Invoke(this);
        }

        return this;
    }

    // ReSharper disable once ConvertNullableToShortForm
    public TextBuilder IfNotNull<T>(Nullable<T> nullable,
        Action<TextBuilder, T>? onNotNull = null,
        Action<TextBuilder>? onNull = null)
        where T : struct
    {
        if (nullable.TryGetValue(out var value))
        {
            onNotNull?.Invoke(this, value);
        }
        else
        {
            onNull?.Invoke(this);
        }

        return this;
    }

#endregion

#region IfNotEmpty

    public TextBuilder IfNotEmpty(string? str,
        Action<TextBuilder, string>? onNotEmpty = null,
        Action<TextBuilder>? onEmpty = null)
    {
        if (!string.IsNullOrEmpty(str))
        {
            onNotEmpty?.Invoke(this, str!);
        }
        else
        {
            onEmpty?.Invoke(this);
        }

        return this;
    }

#if !NET9_0_OR_GREATER
    public delegate void BuildWithSpan<T>(TextBuilder builder, ReadOnlySpan<T> span);

    public delegate void BuildWithInterpolatedText(TextBuilder builder, InterpolatedTextBuilder interpolatedTextBuilder);
#endif

    public TextBuilder IfNotEmpty<T>(scoped ReadOnlySpan<T> span,
#if NET9_0_OR_GREATER
        Action<TextBuilder, ReadOnlySpan<T>>? onNotEmpty = null,
#else
        BuildWithSpan<T>? onNotEmpty = null,
#endif
        Action<TextBuilder>? onEmpty = null)
    {
        if (!span.IsEmpty)
        {
            onNotEmpty?.Invoke(this, span);
        }
        else
        {
            onEmpty?.Invoke(this);
        }

        return this;
    }

    public TextBuilder IfNotEmpty<T>(T[]? array,
        Action<TextBuilder, T[]>? onNotEmpty = null,
        Action<TextBuilder>? onEmpty = null)
    {
        if (array is not null && array.Length > 0)
        {
            onNotEmpty?.Invoke(this, array);
        }
        else
        {
            onEmpty?.Invoke(this);
        }

        return this;
    }

    public TextBuilder IfNotEmpty<T>(ICollection<T>? collection,
        Action<TextBuilder, ICollection<T>>? onNotEmpty = null,
        Action<TextBuilder>? onEmpty = null)
    {
        if (collection is not null && collection.Count > 0)
        {
            onNotEmpty?.Invoke(this, collection);
        }
        else
        {
            onEmpty?.Invoke(this);
        }

        return this;
    }

    public TextBuilder IfNotEmpty<T>(IEnumerable<T>? enumerable,
        Action<TextBuilder, IEnumerable<T>>? onNotEmpty = null,
        Action<TextBuilder>? onEmpty = null)
    {
        if (enumerable is not null)
        {
            if (enumerable.TryGetNonEnumeratedCount(out var count))
            {
                if (count != 0)
                {
                    onNotEmpty?.Invoke(this, enumerable);
                }
                else
                {
                    onEmpty?.Invoke(this);
                }
            }
        }

        return this;
    }

    public TextBuilder IfNotEmpty(
        [HandlesResourceDisposal] InterpolatedTextBuilder interpolatedTextBuilder,
#if NET9_0_OR_GREATER
        Action<TextBuilder, InterpolatedTextBuilder>? onNotEmpty,
#else
        BuildWithInterpolatedText? onNotEmpty,
#endif
        Action<TextBuilder>? onEmpty = null)
    {
        if (interpolatedTextBuilder.Length > 0)
        {
            onNotEmpty?.Invoke(this, interpolatedTextBuilder);
        }
        else
        {
            onEmpty?.Invoke(this);
        }

        return this;
    }

    public TextBuilder IfNotEmpty(
        [HandlesResourceDisposal] InterpolatedTextBuilder interpolatedTextBuilder)
    {
        if (interpolatedTextBuilder.Length > 0)
        {
            return Append(ref interpolatedTextBuilder);
        }

        return this;
    }

    public TextBuilder IfNotEmpty(
        [HandlesResourceDisposal] InterpolatedTextBuilder interpolatedTextBuilder, Action<TextBuilder>? onEmpty)
    {
        if (interpolatedTextBuilder.Length > 0)
        {
            return Append(ref interpolatedTextBuilder);
        }

        onEmpty?.Invoke(this);
        return this;
    }

#endregion

/*

#region FormatSome, FormatOk, FormatError, RenderSome, RenderOk, RenderError

    public TextBuilder FormatSome<T>(Option<T> option, string? format = null, IFormatProvider? provider = null)
    {
        if (option.IsSome(out var value))
            return Format<T>(value, format, provider);
        return this;
    }

    public TextBuilder FormatOk<T>(Result<T> result, string? format = null, IFormatProvider? provider = null)
    {
        if (result.IsOk(out var value))
            return Format<T>(value, format, provider);
        return this;
    }

    public TextBuilder FormatOk<T, E>(Result<T, E> result, string? format = null, IFormatProvider? provider = null)
    {
        if (result.IsOk(out var value))
            return Format<T>(value, format, provider);
        return this;
    }

    public TextBuilder FormatError<T>(Result<T> result, string? format = null, IFormatProvider? provider = null)
    {
        if (result.IsError(out var error))
            return Format<Exception>(error, format, provider);
        return this;
    }

    public TextBuilder FormatError<T, E>(Result<T, E> result, string? format = null, IFormatProvider? provider = null)
    {
        if (result.IsError(out var error))
            return Format<E>(error, format, provider);
        return this;
    }

    public TextBuilder RenderSome<T>(Option<T> option)
    {
        if (option.IsSome(out var value))
            return Render<T>(value);
        return this;
    }

    public TextBuilder RenderOk<T>(Result<T> result)
    {
        if (result.IsOk(out var value))
            return Render<T>(value);
        return this;
    }

    public TextBuilder RenderOk<T, E>(Result<T, E> result)
    {
        if (result.IsOk(out var value))
            return Render<T>(value);
        return this;
    }

    public TextBuilder RenderError<T>(Result<T> result)
    {
        if (result.IsError(out var error))
            return Render<Exception>(error);
        return this;
    }

    public TextBuilder RenderError<T, E>(Result<T, E> result)
    {
        if (result.IsError(out var error))
            return Render<E>(error);
        return this;
    }

#endregion


#region Enumerate

    public TextBuilder Enumerate<T>(scoped ReadOnlySpan<T> values, Action<TextBuilder, T> buildValue)
    {
        foreach (var t in values)
        {
            buildValue(this, t);
        }

        return this;
    }

    public TextBuilder Enumerate<T>(scoped Span<T> values, Action<TextBuilder, T> buildValue)
    {
        foreach (var t in values)
        {
            buildValue(this, t);
        }

        return this;
    }

    public TextBuilder Enumerate<T>(T[]? values, Action<TextBuilder, T> buildValue)
    {
        if (values is not null)
        {
            foreach (var t in values)
            {
                buildValue(this, t);
            }
        }

        return this;
    }

    public TextBuilder Enumerate<T>(IEnumerable<T>? values, Action<TextBuilder, T> buildValue)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                buildValue(this, value);
            }
        }

        return this;
    }

    public TextBuilder Enumerate(string? str, Action<TextBuilder, char> buildValue)
    {
        if (str is not null)
        {
            foreach (char ch in str)
            {
                buildValue(this, ch);
            }
        }

        return this;
    }

    public TextBuilder Enumerate<T>(SpanSplitter<T> splitSpan, BuildSegment<T> buildSegment)
        where T : IEquatable<T>
    {
        while (splitSpan.MoveNext())
        {
            buildSegment(this, splitSpan.Current);
        }

        return this;
    }

#endregion

#region EnumerateFormat, EnumerateRender

    public TextBuilder EnumerateFormat<T>(scoped ReadOnlySpan<T> values,
        string? format = null,
        IFormatProvider? provider = null)
        => Enumerate<T>(values, (tb, value) => tb.Append<T>(value, format, provider));

    public TextBuilder EnumerateFormat<T>(scoped Span<T> values,
        string? format = null,
        IFormatProvider? provider = null)
        => Enumerate<T>(values, (tb, value) => tb.Append<T>(value, format, provider));

    public TextBuilder EnumerateFormat<T>(T[]? values,
        string? format = null,
        IFormatProvider? provider = null)
        => Enumerate<T>(values, (tb, value) => tb.Append<T>(value, format, provider));

    public TextBuilder EnumerateFormat<T>(IEnumerable<T>? values,
        string? format = null,
        IFormatProvider? provider = null)
        => Enumerate<T>(values, (tb, value) => tb.Append<T>(value, format, provider));

    public TextBuilder EnumerateRender<T>(scoped ReadOnlySpan<T> values)
        => Enumerate<T>(values, static (tb, value) => tb.Render<T>(value));

    public TextBuilder EnumerateRender<T>(scoped Span<T> values)
        => Enumerate<T>(values, static (tb, value) => tb.Render<T>(value));

    public TextBuilder EnumerateRender<T>(T[]? values)
        => Enumerate<T>(values, static (tb, value) => tb.Render<T>(value));

    public TextBuilder EnumerateRender<T>(IEnumerable<T>? values)
        => Enumerate<T>(values, static (tb, value) => tb.Render<T>(value));

#endregion


#region Iterate

    public TextBuilder Iterate<T>(
        scoped ReadOnlySpan<T> values,
        Action<TextBuilder, T, int> buildTextWithValueIndex)
    {
        for (int i = 0; i < values.Length; i++)
        {
            buildTextWithValueIndex(this, values[i], i);
        }

        return this;
    }

    public TextBuilder Iterate<T>(
        scoped Span<T> values,
        Action<TextBuilder, T, int> buildTextWithValueIndex)
    {
        for (int i = 0; i < values.Length; i++)
        {
            buildTextWithValueIndex(this, values[i], i);
        }

        return this;
    }

    public TextBuilder Iterate<T>(
        T[]? values,
        Action<TextBuilder, T, int> buildTextWithValueIndex)
    {
        if (values is not null)
        {
            for (int i = 0; i < values.Length; i++)
            {
                buildTextWithValueIndex(this, values[i], i);
            }
        }

        return this;
    }

    public TextBuilder Iterate<T>(
        IList<T>? values,
        Action<TextBuilder, T, int> buildTextWithValueIndex)
    {
        if (values is not null)
        {
            for (int i = 0; i < values.Count; i++)
            {
                buildTextWithValueIndex(this, values[i], i);
            }
        }

        return this;
    }

    public TextBuilder Iterate<T>(
        IEnumerable<T>? values,
        Action<TextBuilder, T, int> buildTextWithValueIndex)
    {
        if (values is not null)
        {
            int index = 0;
            foreach (var value in values)
            {
                buildTextWithValueIndex(this, value, index);
                index++;
            }
        }

        return this;
    }

#endregion

*/
}