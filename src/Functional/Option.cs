namespace ScrubJay.Functional;

public static class Option
{
#region Constructors

    public static IMPL.None None() => default;
    public static Option<T> None<T>() => default;
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);

    public static Option<T> NotNull<T>(T? value)
    {
        if (value is not null)
            return Option<T>.Some(value);
        return Option<T>.None;
    }


    // ReSharper disable once ConvertNullableToShortForm
    public static Option<T> NotNull<T>(Nullable<T> nullableValue)
        where T : struct
    {
        if (nullableValue.HasValue)
        {
            return Some(nullableValue.GetValueOrDefault());
        }

        return default;
    }

#endregion

#region Extensions

    public static Option<T> Flatten<T>(this Option<Option<T>> nestedOptions)
    {
        if (nestedOptions.IsSome(out var option))
            return option;
        return default;
    }

    public static Option<T> Insert<T>(this ref Option<T> option, T value)
    {
        if (option.IsSome(out var existingValue))
        {
            option = Some(value);
            return Some(existingValue);
        }
        else
        {
            option = Some(value);
            return default;
        }
    }

    public static T GetOrInsert<T>(this ref Option<T> option, T value)
    {
        if (option.IsSome(out var existingValue))
            return existingValue;
        option = Some(value);
        return value;
    }
    
    
    public static T GetOrInsert<T>(this ref Option<T> option, Func<T> valueFactory)
    {
        if (option.IsSome(out var value))
            return value;
        value = valueFactory();
        option = Some(value);
        return value;
    }

    public static Option<T> Take<T>(this ref Option<T> option)
    {
        if (option.IsSome(out var existingValue))
        {
            option = default;
            return Some(existingValue);
        }
        else
        {
            option = default;
            return default;
        }
    }

    public static void Swap<T>(this ref Option<T> left, ref Option<T> right)
    {
        if (left.IsSome(out var leftValue))
        {
            if (right.IsSome(out var rightValue))
            {
                left = Some(rightValue);
                right = Some(leftValue);
            }
            else // right is None
            {
                left = default;
                right = Some(leftValue);
            }
        }
        else // left is None
        {
            if (right.IsSome(out var rightValue))
            {
                left = Some(rightValue);
                right = default;
            }
            else // right is None
            {
                // no need to do anything
            }
        }
    }

#endregion
}