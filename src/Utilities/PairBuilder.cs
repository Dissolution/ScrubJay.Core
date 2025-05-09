#pragma warning disable MA0025

namespace ScrubJay.Utilities;

[PublicAPI]
[InterpolatedStringHandler]
public ref struct PairBuilder<TKey, TValue>
{
    private enum Step
    {
        Invalid = 0,

        PreStartParen,
        PreKey,
        PreComma,
        PreValue,
        PreEndParen,
        Finished,
    }


    private Option<TKey> _key = default;
    private Option<TValue> _value = default;
    private Step _step = Step.PreStartParen;
    private object? _error = null;

    public Result<Pair<TKey, TValue>> TryGetPair()
    {
        if (_error is Exception ex)
            return ex;
        if (_error is List<Exception> exs)
            return new AggregateException(exs);
        if (!_key.IsSome(out var key))
            return new InvalidOperationException("No key has been specified");
        if (!_value.IsSome(out var value))
            return new InvalidOperationException("No value has been specified");
        return Ok(Pair.New(key, value));
    }

    private void AddError(Exception error)
    {
        switch (_error)
        {
            case null:
                _error = error;
                break;
            case Exception exception:
                _error = new List<Exception>
                {
                    exception,
                    error,
                };
                break;
            case List<Exception> errors:
                errors.Add(error);
                break;
            default:
                throw new UnreachableException();
        }
    }

    public PairBuilder(int formattedLength, int argumentCount)
    {
        if ((argumentCount != 2) || (formattedLength < 3))
        {
            AddError(new InvalidOperationException($"A {typeof(Pair<TKey, TValue>)} must be specified as `(Key,Value)` (with optional whitespace)"));
        }
    }

    public void AppendLiteral(string str)
    {
        text text = str.AsSpan().Trim();
        if (text.Length == 1)
        {
            char ch = text[0];
            if (ch == '(')
            {
                if (_step != Step.PreStartParen)
                    throw new InvalidOperationException();
                _step = Step.PreKey;
            }
            else if (ch == ')')
            {
                if (_step != Step.PreEndParen)
                    throw new InvalidOperationException();
                _step = Step.Finished;
            }
            else if (ch == ',')
            {
                if (_step != Step.PreComma)
                    throw new InvalidOperationException();
                _step = Step.PreValue;
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public void AppendFormatted(TKey key)
    {
        if (_key.IsSome())
            throw new InvalidOperationException();
        if (_step != Step.PreKey)
            throw new InvalidOperationException();
        _key = Some(key);
        _step = Step.PreComma;
    }

    public void AppendFormatted(TValue value)
    {
        if (_value.IsSome())
            throw new InvalidOperationException();
        if (_step != Step.PreValue)
            throw new InvalidOperationException();
        _value = Some(value);
        _step = Step.PreEndParen;
    }
}
