#pragma warning disable S3247

// ReSharper disable UseSwitchCasePatternVariable
// ReSharper disable MergeCastWithTypeCheck
namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="object"/> and <c>T</c>
/// </summary>
[PublicAPI]
public static class ObjectExtensions
{
    extension<T>(T? value)
    {
        public Option<T> NotNull()
        {
            if (value is not null)
                return Some<T>(value);
            return None;
        }

    }

    extension(object? obj)
    {
        public bool Is<T>([NotNullWhen(true)] out T? value)
        {
            if (obj is T)
            {
                value = (T)obj;
                return true;
            }

            value = default;
            return false;
        }

        public Result<T> Is<T>()
        {
            if (obj is T)
            {
                return Ok<T>((T)obj);
            }

            if (obj is null)
            {
                return new ArgumentNullException(nameof(obj));
            }

            return Ex.Arg(obj, $"Object `{obj:@}` is not a {typeof(T):@} instance");
        }

        public bool As<T>([NotNullIfNotNull(nameof(obj))] out T? value)
        {
            if (obj is T)
            {
                value = (T)obj;
                return true;
            }

            if (obj is null && typeof(T).CanBeNull)
            {
                value = default!;
                return true;
            }

            value = default;
            return false;
        }

        public Result<T?> As<T>()
        {
            if (obj is T)
            {
                return Ok((T?)obj);
            }

            if (obj is null)
            {
                if (typeof(T).CanBeNull)
                {
                    return Ok(default(T));
                }

                return new ArgumentNullException(nameof(obj));
            }

            return Ex.Arg(obj, $"Object `{obj:@}` is not a {typeof(T):@} instance");
        }
    }
}
