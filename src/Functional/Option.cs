#pragma warning disable CA1716

namespace ScrubJay.Functional;

/// <summary>
/// A static utility class for constructing <see cref="Option{T}"/> instances
/// </summary>
[PublicAPI]
public static class Option
{
#region Constructors

    public static IMPL.None None() => default;
    
    public static Option<T> None<T>() => default;
    
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);

    /// <summary>
    /// Returns <see cref="Option{T}.Some"/> if <paramref name="value"/> is not <c>null</c>,<br/>
    /// otherwise returns <see cref="Option{T}.None"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> NotNull<T>(T? value)
    {
        if (value is not null)
            return Option<T>.Some(value);
        return Option<T>.None;
    }

    /// <summary>
    /// Returns <see cref="Option{T}.Some"/> if <paramref name="value"/> is not <c>null</c>,<br/>
    /// otherwise returns <see cref="Option{T}.None"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once ConvertNullableToShortForm
    public static Option<T> NotNull<T>(Nullable<T> value)
        where T : struct
    {
        if (value.HasValue)
        {
            return Option<T>.Some(value.GetValueOrDefault());
        }

        return Option<T>.None;
    }

#endregion

#region Extensions

    public static Option<T> Flatten<T>(this Option<Option<T>> nestedOptions)
    {
        if (nestedOptions.IsSome(out var option))
            return option;
        return default;
    }

    /// <summary>
    /// Extensions on a <c>ref Option&lt;T&gt;</c> that can add/remove/exchange values (similar to rust)
    /// </summary>
    /// <param name="option"></param>
    /// <typeparam name="T"></typeparam>
    extension<T>(ref Option<T> option)
    {
        public Option<T> Insert(T value)
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

        public T GetOrInsert(T value)
        {
            if (option.IsSome(out var existingValue))
                return existingValue;
            option = Some(value);
            return value;
        }

        public T GetOrInsert(Func<T> valueFactory)
        {
            if (option.IsSome(out var value))
                return value;
            value = valueFactory();
            option = Some(value);
            return value;
        }

        public Option<T> Take()
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

        public void Swap(ref Option<T> right)
        {
            if (option.IsSome(out var leftValue))
            {
                if (right.IsSome(out var rightValue))
                {
                    option = Some(rightValue);
                    right = Some(leftValue);
                }
                else // right is None
                {
                    option = default;
                    right = Some(leftValue);
                }
            }
            else // left is None
            {
                if (right.IsSome(out var rightValue))
                {
                    option = Some(rightValue);
                    right = default;
                }
                else // right is None
                {
                    // no need to do anything
                }
            }
        }
    }

    #endregion
}