#if NET48_OR_GREATER || NETSTANDARD2_0

namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Stack{T}"/> to bring it up to parity with <c>.net 8.0</c>
/// </summary>
public static class StackExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryPeek<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T value)
    {
        if (stack.Count > 0)
        {
            value = stack.Peek();
            return true;
        }

        value = default;
        return false;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryPop<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T value)
    {
        if (stack.Count > 0)
        {
            value = stack.Pop();
            return true;
        }

        value = default;
        return false;
    }
}
#endif