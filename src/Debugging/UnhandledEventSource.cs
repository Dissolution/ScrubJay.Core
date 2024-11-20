#pragma warning disable CA1200

namespace ScrubJay.Debugging;

public enum UnhandledEventSource
{
    Unknown = 0,

    /// <summary>
    /// <see cref="E:AppDomain.CurrentDomain.UnhandledException"/>
    /// </summary>
    AppDomain = 1,

    /// <summary>
    /// <see cref="E:TaskScheduler.UnobservedTaskException"/>
    /// </summary>
    TaskScheduler = 2,

    /// <summary>
    ///
    /// </summary>
    Unbreakable = 3,
}