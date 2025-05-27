namespace ScrubJay.Functional;

[PublicAPI]
public static class OptionExtensions
{
    // https://doc.rust-lang.org/std/option/#modifying-an-option-in-place

    // https://doc.rust-lang.org/std/option/enum.Option.html#method.insert
    public static T Insert<T>(this ref Option<T> option, T value)
    {
        option = Some(value);
        return value;
    }

    // https://doc.rust-lang.org/std/option/enum.Option.html#method.get_or_insert
    public static T GetOrInsert<T>(this ref Option<T> option, T value)
    {
        if (option.IsSome(out var some))
            return some;
        option = Some(value);
        return value;
    }

    // https://doc.rust-lang.org/std/option/enum.Option.html#method.get_or_insert_with
    public static T GetOrInsert<T>(this ref Option<T> option, Func<T> f)
    {
        if (option.IsSome(out var value))
            return value;
        value = f();
        option = Some(value);
        return value;
    }

    // https://doc.rust-lang.org/std/option/enum.Option.html#method.take
    public static Option<T> Take<T>(this ref Option<T> option)
    {
        Option<T> temp = option;
        option = None<T>();
        return temp;
    }

    // https://doc.rust-lang.org/std/option/enum.Option.html#method.replace
    public static Option<T> Replace<T>(this ref Option<T> option, T value)
    {
        Option<T> temp = option;
        option = Some(value);
        return temp;
    }
}