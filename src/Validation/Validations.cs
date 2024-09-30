﻿#pragma warning disable CA1710, CA1031

using ScrubJay.Collections;

namespace ScrubJay.Validation;

/// <summary>
/// A collection of sequential validations
/// </summary>
/// <remarks>
/// Every call to an <c>Add</c> method will invoke immediately
/// If an <see cref="Exception"/> is caught or an Error returned,
/// execution of further <c>Adds</c> will be skipped
/// and the <see cref="Exception"/> will be the return from <see cref="GetResult"/>
/// </remarks>
public sealed class Validations : IEnumerable<Unit>, IEnumerable
{
    public static Validations New => new();
    
    private Option<Exception> _hasException;

    public void Add(Action? action)
    {
        if (_hasException || action is null)
            return;
        
        try
        {
            action();
        }
        catch (Exception ex)
        {
            _hasException = Some(ex);
        }
    }

    public void Add<T>(Func<T>? func)
    {
        if (_hasException || func is null)
            return;
        
        try
        {
            _ = func();
        }
        catch (Exception ex)
        {
            _hasException = Some(ex);
        }
    }

    public void Add(Result<Unit, Exception> result)
    {
        if (_hasException)
            return;

        if (result.IsError(out var exception))
        {
            _hasException = Some(exception);
        }
    }
    
    public void Add<T>(Result<T, Exception> result)
    {
        if (_hasException)
            return;

        if (result.IsError(out var exception))
        {
            _hasException = Some(exception);
        }
    }

    public void Add(Func<Result<Unit, Exception>>? getResult)
    {
        if (_hasException || getResult is null)
            return;
        
        if (getResult().IsError(out var exception))
        {
            _hasException = Some(exception);
        }
    }

    public void Add<T>(Func<Result<T, Exception>>? getResult)
    {
        if (_hasException || getResult is null)
            return;
        
        if (getResult().IsError(out var exception))
        {
            _hasException = Some(exception);
        }
    }

    public Result<Unit, Exception> GetResult()
    {
        if (_hasException.IsSome(out var ex))
            return ex;
        return Ok();
    }

    public Result<TOk, Exception> GetResult<TOk>(TOk okValue)
    {
        if (_hasException.IsSome(out var ex))
            return ex;
        return okValue;
    }
    
    IEnumerator IEnumerable.GetEnumerator() => EmptyEnumerator<Unit>.Instance;
    IEnumerator<Unit> IEnumerable<Unit>.GetEnumerator() => EmptyEnumerator<Unit>.Instance;
}