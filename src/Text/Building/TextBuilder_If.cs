namespace ScrubJay.Text;

partial class TextBuilder
{
#region If(bool)

    public TextBuilder If(bool condition,
        Action<TextBuilder>? onTrue = null,
        Action<TextBuilder>? onFalse = null)
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

    public TextBuilder If<T>(bool condition,
        T trueValue,
        Action<TextBuilder>? onFalse = null)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
    {
        if (condition)
        {
            return Append<T>(trueValue);
        }
        else
        {
            return Invoke(onFalse);
        }
    }

    public TextBuilder If<F>(bool condition,
        Action<TextBuilder>? onTrue,
        F falseValue)
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
            return Append<F>(falseValue);
        }
    }

    public TextBuilder If<T,F>(bool condition,
        T trueValue,
        F falseValue)
#if NET9_0_OR_GREATER
    where T : allows ref struct
    where F : allows ref struct
#endif
    {
        if (condition)
        {
            return Append<T>(trueValue);
        }
        else
        {
            return Append<F>(falseValue);
        }
    }


    public TextBuilder If(bool condition, txt trueText = default, txt falseText = default)
    {
        if (condition)
        {
            return Append(trueText);
        }
        else
        {
            return Append(falseText);
        }
    }

#endregion /if(bool)

    public TextBuilder If<T>(Option<T> option,
        Action<TextBuilder, T>? onSome = null,
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

    public TextBuilder If<T>(RefOption<T> option,
        Action<TextBuilder, T>? onSome = null,
        Action<TextBuilder>? onNone = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
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

    public TextBuilder If(Result result,
        Action<TextBuilder>? onOk = null,
        Action<TextBuilder, Exception>? onError = null)
    {
        if (result.IsError(out var error))
        {
            onError?.Invoke(this, error);
        }
        else
        {
            onOk?.Invoke(this);
        }

        return this;
    }

    public TextBuilder If<T>(Result<T> result,
        Action<TextBuilder, T>? onOk = null,
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

    public TextBuilder If<T>(RefResult<T> result,
        Action<TextBuilder, T>? onOk = null,
        Action<TextBuilder, Exception>? onError = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
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

    public TextBuilder If<T, E>(Result<T, E> result,
        Action<TextBuilder, T>? onOk = null,
        Action<TextBuilder, E>? onError = null)
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

    public TextBuilder IfNotNull<T>(T? value,
        Action<TextBuilder, T>? onNotNull = null,
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
}