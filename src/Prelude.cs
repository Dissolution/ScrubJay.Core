#pragma warning disable CA1715


// alias `ReadOnlySpan<char>` to `text`
global using text = System.ReadOnlySpan<char>;
// prevent attribute name conflict with `JetBrains.Annotations.NotNullAttribute`
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

using ScrubJay.Functional.IMPL;


namespace ScrubJay;

/// <summary>
/// Global helper methods
/// </summary>
/// <remarks>
/// To include these methods in a single <c>.cs</c> file, add to its <c>usings</c> section:<br/>
/// <code>
/// using static ScrubJay.Prelude
/// </code><br/>
/// To include them in an entire project, add to its <c>.csproj</c> file:<br/>
/// <code>
/// &lt;ItemGroup&gt;
///     &lt;Using Include="ScrubJay.Prelude" Static="true"/&gt;
/// &lt;/ItemGroup&gt;
/// </code><br/>
/// </remarks>
[PublicAPI]
public static class Prelude
{
    public static None None { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ok<T> Ok<T>(T value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => new Ok<T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error<T> Error<T>(T error)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => new Error<T>(error);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Unit Unit() => default(Unit);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Build(ref InterpolatedTextBuilder interpolatedTextBuilder)
    {
        return interpolatedTextBuilder.ToStringAndDispose();
    }
}