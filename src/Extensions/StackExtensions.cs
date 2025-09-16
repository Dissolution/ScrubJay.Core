namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Stack{T}"/>
/// </summary>
[PublicAPI]
public static class StackExtensions
{
// #if NETFRAMEWORK || NETSTANDARD2_0
//     public static bool TryPeek<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T value)
//     {
//         if (stack.Count > 0)
//         {
//             value = stack.Peek();
//             return true;
//         }
//
//         value = default;
//         return false;
//     }
//
//     public static bool TryPop<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T value)
//     {
//         if (stack.Count > 0)
//         {
//             value = stack.Pop();
//             return true;
//         }
//
//         value = default;
//         return false;
//     }
// #endif

    public static T PeekOr<T>(this Stack<T> stack, T defaultValue) => stack.Count > 0 ? stack.Peek() : defaultValue;

    public static T PeekOrPush<T>(this Stack<T> stack, T pushValue)
    {
        if (stack.Count > 0)
            return stack.Peek();
        stack.Push(pushValue);
        return pushValue;
    }

    public static T PopOr<T>(this Stack<T> stack, T defaultValue) => stack.Count > 0 ? stack.Pop() : defaultValue;
}
