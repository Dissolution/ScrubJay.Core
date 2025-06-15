global using text = System.ReadOnlySpan<char>;

#pragma warning disable CA1715

namespace ScrubJay.Functional;

// <summary>
// Global helper methods to support <see cref="Option{T}"/> and <see cref="Result{TOk, TError}"/>
// </summary>
// <remarks>
// To make these methods available everywhere in a single <c>.cs</c> file,
// add the following line in the <c>usings</c> section:
// <code>
// using static ScrubJay.GlobalHelper;
// </code>
// To make them available for an entire project,
// add the following lines to that project's <c>.csproj</c> file:
// <code>
// &lt;ItemGroup&gt;
//     &lt;Using Include="ScrubJay.GlobalHelper" Static="true"/&gt;
// &lt;/ItemGroup&gt;
// </code>
// </remarks>
[PublicAPI]
public static class Prelude
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Unit Unit()
        => default(Unit);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static None None()
        => default(None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>()
        => Option<T>.None();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value)
        => Option<T>.Some(value);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok<T>(T value)
        => Result<T>.Ok(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, E> Ok<T, E>(T value)
        => Result<T, E>.Ok(value);

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static Error<E> Error<E>(E error)
    //     where E : Exception
    //     => new Error<E>(error);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Error<T>(Exception error)
        => Result<T>.Error(error);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, E> Error<T, E>(E error)
        => Result<T, E>.Error(error);
}
