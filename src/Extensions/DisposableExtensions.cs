namespace ScrubJay.Extensions;

public static class DisposableExtensions
{
    [HandlesResourceDisposal]
    public static void SafeDispose<T>(this T? disposable)
        where T : IDisposable
    {
        if (disposable is not null)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                // Swallow
            }
        }
    }
}