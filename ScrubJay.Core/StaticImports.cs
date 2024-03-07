global using static ScrubJay.StaticImports;

namespace ScrubJay;

// ReSharper disable once InconsistentNaming
public static class StaticImports
{
    /// <inheritdoc cref="Option{T}.None"/>
    public static Option<T> None<T>() => Option<T>.None;
    
    /// <inheritdoc cref="Option{T}.Some"/>
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);

    public static Option<T> NotNull<T>(T? value) => Option<T>.NotNull(value);
    
    /// <inheritdoc cref="Result{T}.Ok"/>
    public static Result Ok() => Result.Ok();

    /// <inheritdoc cref="Result{T}.Error"/>
    public static Exception Error(Exception? error) => error ?? new Exception();

    /// <inheritdoc cref="Result{T}.Ok"/>
    public static Result<TValue> Ok<TValue>(TValue value) => Result<TValue>.Ok(value);
    
    /// <inheritdoc cref="Result{T}.Error"/>
    public static Result<TValue> Error<TValue>(Exception? error) => Result<TValue>.Error(error);
}