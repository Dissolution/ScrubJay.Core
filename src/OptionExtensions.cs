namespace ScrubJay;

public static class OptionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="option"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.flatten"/>
    public static Option<T> Flatten<T>(this Option<Option<T>> option)
    {
        if (option.IsSome(out var subOpt) && subOpt.IsSome(out var value))
            return Some(value);
        return Option<T>.None;
    }
}