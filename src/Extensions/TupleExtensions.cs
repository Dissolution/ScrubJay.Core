namespace ScrubJay.Extensions;

[PublicAPI]
public static class TupleExtensions
{
    [PublicAPI]
    public ref struct TupleEnumerator<T> :
        //IEnumerable<object?>, IEnumerable,
        IEnumerator<object?>, IEnumerator
        where T : ITuple
    {
        private readonly T _tuple;
        private int _index;

        public object? Current
        {
            get
            {
                int i = Throw.IfBadIndex(_index, _tuple.Length);
                return _tuple[i];
            }
        }

        public TupleEnumerator(T tuple)
        {
            _tuple = tuple;
            _index = -1;
        }

        public bool MoveNext()
        {
            int next = _index + 1;
            if (next < _tuple.Length)
            {
                _index = next;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _index = -1;
        }

        public void Dispose()
        {
            // do nothing
        }

        public TupleEnumerator<T> GetEnumerator()
        {
            return this;
        }
    }

    extension<T>(T tuple)
        where T : ITuple
    {
        public TupleEnumerator<T> GetEnumerator()
        {
            return new TupleEnumerator<T>(tuple);
        }
    }
}