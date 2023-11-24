namespace ScrubJay;

// ReSharper disable once InconsistentNaming
public static class StaticImports
{
    public static Option<T> None<T>() => Option<T>.None;

    public static Option<T> Some<T>(T value) => Option<T>.Some(value);


    public static Result Ok()
        => Result.Ok();

    public static Result Error(Exception? error)
        => Result.Error(error);


    public static Result<TValue> Ok<TValue>(TValue value)
        => Result<TValue>.Ok(value);

    public static Result<TValue> Error<TValue>(Exception? error)
        => Result<TValue>.Error(error);


    public static Result<TOk, TError> Ok<TOk, TError>(TOk okValue)
        => Result<TOk, TError>.Ok(okValue);

    public static Result<TOk, TError> Error<TOk, TError>(TError? errorValue)
        => Result<TOk, TError>.Error(errorValue);
}