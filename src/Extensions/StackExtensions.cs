namespace ScrubJay.Extensions;

public static class StackExtensions
{
#if NET48 || NETSTANDARD2_0
    public static bool TryPeek<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T value)
    {
        if (stack.Count == 0)
        {
            value = default;
            return false;
        }

        value = stack.Peek();
        return true;
    }

    public static bool TryPop<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T value)
    {
        if (stack.Count == 0)
        {
            value = default;
            return false;
        }

        value = stack.Pop();
        return true;
    }
#endif
}