namespace ScrubJay.Exceptions;

[PublicAPI]
[StackTraceHidden]
public static class Ex
{
    [StackTraceHidden]
    public static InvalidOperationException Invalid(ref InterpolatedTextBuilder interpolatedMessage)
    {
        string message = interpolatedMessage.ToStringAndDispose();
        return new InvalidOperationException(message);
    }
}