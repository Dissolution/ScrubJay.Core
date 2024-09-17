using ObjectDisposedException = System.ObjectDisposedException;

namespace ScrubJay.Validation;

public static partial class Validate
{
    public static void ThrowIfNull<T>(
        [AllowNull, NotNull] T value, 
        [CallerArgumentExpression(nameof(value))] string? valueName = null)
        where T : class?
    {
        if (value is null)
            throw new ArgumentNullException(valueName);
    }
    
    
#pragma warning disable CA1513
    [StackTraceHidden]
    public static void ThrowIfDisposed([DoesNotReturnIf(true)] bool condition, object instance)
    {
        if (condition)
        {
            throw new ObjectDisposedException(instance.GetType().FullName);
        }
    }
#pragma warning restore CA1513

}