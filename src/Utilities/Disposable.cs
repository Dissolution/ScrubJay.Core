using System.Reflection;
using ScrubJay.Expressions;
using ScrubJay.Functional.Linq;

#pragma warning disable CA1031, MA0048, CA1045

namespace ScrubJay.Utilities;

#if !NETFRAMEWORK && !NETSTANDARD2_0
/// <summary>
/// Helper utility for working with <see cref="IDisposable"/> and <see cref="IAsyncDisposable"/>
/// </summary>
#else
/// <summary>
/// Helper utility for working with <see cref="IDisposable"/>
/// </summary>
#endif
[PublicAPI]
public static class Disposable
{
    /// <summary>
    /// Gets an <see cref="IDisposable"/> that will invoke an <see cref="Action"/> when it is disposed
    /// </summary>
    /// <param name="onDispose">
    /// The <see cref="Action"/> that will be invoked during disposal
    /// </param>
    [MustDisposeResource]
    public static IDisposable Action(Action onDispose)
    {
        Throw.IfNull(onDispose);
        return new ActionDisposable(onDispose);
    }

#if !NETFRAMEWORK && !NETSTANDARD2_0
    /// <summary>
    /// Gets an <see cref="IDisposable"/> that will asynchronously invoke an <see cref="Action"/> when it is disposed
    /// </summary>
    /// <param name="onDispose">
    /// The <c>async</c> <see cref="Action"/> that will be invoked during disposal
    /// </param>
    [MustDisposeResource]
    public static IAsyncDisposable AsyncAction(Func<ValueTask> onDispose)
    {
        Throw.IfNull(onDispose);
        return new ActionAsyncDisposable(onDispose);
    }
#endif

    public static void Dispose(
        [HandlesResourceDisposal]
        this IEnumerator enumerator)
    {
        if (enumerator is IDisposable)
        {
            ((IDisposable)enumerator).Dispose();
        }
    }


    public static void DisposeVal<T>([HandlesResourceDisposal] T? value)
        => Disposable<T>.Dispose(value);

    public static void Dispose([HandlesResourceDisposal] IDisposable? disposable)
        => disposable?.Dispose();

    public static bool TryDisposeAndDefaultRef<TDisposable>(
        [HandlesResourceDisposal]
        ref TDisposable? disposable)
        where TDisposable : class, IDisposable
    {
        // Acquire and immediately set to null
        var localDisposable = Interlocked.Exchange<TDisposable?>(ref disposable, null);
        if (localDisposable is not null)
        {
            try
            {
                localDisposable.Dispose();
                return true;
            }
            catch (Exception)
            {
                // Suppress any errors
                return false;
            }
            finally
            {
                // always set to default
                disposable = default;
            }
        }

        return true;
    }
}

public static class Disposable<T>
{
    private static readonly Action<T> _dispose;

    static Disposable()
    {
        var type = typeof(T);

        var disposeMethod = type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(static method => (method.Name == "Dispose") && (method.ReturnType == typeof(void)) && (method.GetParameters().Length == 0))
            .OneOrDefault();

        if (disposeMethod is null)
        {
            _dispose = DoNothing;
        }
        else
        {
            _dispose = new LambdaBuilder<Action<T>>()
                .ParamName(0, "this")
                .Body(b => b.Call(0, disposeMethod))
                .TryCompile()
                .OkOrThrow();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DoNothing(T _) { }

    public static void Dispose([HandlesResourceDisposal] T? disposable)
    {
        if (disposable is not null)
        {
            _dispose(disposable);
        }
    }
}

internal sealed class ActionDisposable : IDisposable
{
    private readonly Action _onDispose;

    internal ActionDisposable(Action onDispose)
    {
        _onDispose = onDispose;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => _onDispose();
}

#if !NETFRAMEWORK && !NETSTANDARD2_0
internal sealed class ActionAsyncDisposable : IAsyncDisposable
{
    private readonly Func<ValueTask> _onAsyncDispose;

    public ActionAsyncDisposable(Func<ValueTask> onAsyncDispose)
    {
        _onAsyncDispose = onAsyncDispose;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask DisposeAsync() => _onAsyncDispose(); // no await, direct pass-through
}
#endif
