#pragma warning disable MA0025

namespace ScrubJay.Utilities;

[PublicAPI]
#if NET6_0_OR_GREATER
[InterpolatedStringHandler]
#endif
public ref struct PairBuilder<K, V>
{
    private enum Step
    {
        PreStartParen,
        PreKey,
        PreComma,
        PreValue,
        PreEndParen,
        Finished,
    }


    private Option<K> _key = default;
    private Option<V> _value = default;
    private Step _step = Step.PreStartParen;
    private object? _error = null;

    public Result<Pair<K, V>> TryGetPair()
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
            AddError(new InvalidOperationException($"A {typeof(Pair<K, V>)} must be specified as `(Key,Value)` (with optional whitespace)"));
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

    public void AppendFormatted(K key)
    {
        if (_key.IsSome())
            throw new InvalidOperationException();
        if (_step != Step.PreKey)
            throw new InvalidOperationException();
        _key = Some(key);
        _step = Step.PreComma;
    }

    public void AppendFormatted(V value)
    {
        if (_value.IsSome())
            throw new InvalidOperationException();
        if (_step != Step.PreValue)
            throw new InvalidOperationException();
        _value = Some(value);
        _step = Step.PreEndParen;
    }
}
