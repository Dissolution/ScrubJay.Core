using ScrubJay.Collections;
using ScrubJay.Comparison;

namespace ScrubJay.Scratch;

public enum InsertionBehavior
{
    None = 0,
    OverwriteExisting,
    ThrowOnExisting,
}

public class UberDict<TKey, TValue> :
    ICollection<Pair<TKey, TValue>>,
    IReadOnlyCollection<Pair<TKey, TValue>>,
    IEnumerable<Pair<TKey, TValue>>,
    IEnumerable,
    ICloneable<UberDict<TKey, TValue>>
{
    private const int START_OF_FREE_LIST = -3;

    private struct DictEntry
    {
        public TKey Key;

        public TValue Value;

        public uint KeyHashCode;

        /// <summary>
        /// 0-based index of NextEntryIndex entry in chain: -1 means end of chain
        /// also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
        /// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
        /// </summary>
        public int NextEntryIndex;
    }

    private static uint GetKeyHash(TKey key, IEqualityComparer<TKey>? keyComparer = null)
    {
        if (key is null)
            return 0U;
        if (keyComparer is null)
            return (uint)key.GetHashCode();
        return (uint)keyComparer.GetHashCode(key);
    }


    private int[] _buckets;
    private DictEntry[] _entries;

    private int _count;
    private int _freeList;
    private int _freeCount;
    private int _version;

    private readonly IEqualityComparer<TKey> _keyEqualityComparer = EqualityComparer<TKey>.Default;
    private UberDictKeys? _keys;
    private UberDictValues? _values;

    bool ICollection<Pair<TKey, TValue>>.IsReadOnly => false;

    public IEqualityComparer<TKey> KeyEqualityComparer
    {
        get => _keyEqualityComparer;
        init => _keyEqualityComparer = value;
    }

    public int Count => _count - _freeCount;

    public UberDictKeys Keys => _keys ??= new UberDictKeys(this);

    public UberDictValues Values => _values ??= new UberDictValues(this);

    public TValue this[TKey key]
    {
        get
        {
            ref TValue value = ref FindValueRef(key);
            if (!Notsafe.IsNullRef(ref value))
            {
                return value;
            }

            Throw.KeyNotFound(key);
            throw new UnreachableException();
        }
        set
        {
            bool modified = TryInsert(key, value, InsertionBehavior.OverwriteExisting);
            Debug.Assert(modified);
        }
    }

#pragma warning disable CS8618
    public UberDict() : this(0)
    {
    }

    public UberDict(int minCapacity)
    {
        Initialize(minCapacity);
        Debug.Assert(_entries is not null);
        Debug.Assert(_buckets is not null);
    }

    public UberDict(IEqualityComparer<TKey> keyEqualityComparer) : this(0, keyEqualityComparer)
    {
    }

    public UberDict(int minCapacity, IEqualityComparer<TKey> keyEqualityComparer)
    {
        Initialize(minCapacity);
        Debug.Assert(_entries is not null);
        Debug.Assert(_buckets is not null);
        this.KeyEqualityComparer = keyEqualityComparer ?? EqualityComparer<TKey>.Default;
    }
#pragma warning restore CS8618


#region NonPublic Methods

    private int Initialize(int minCapacity)
    {
        int size = Hasher.GetPrime(minCapacity);
        int[] buckets = new int[size];
        DictEntry[] entries = new DictEntry[size];

        // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
        _freeList = -1;
        _buckets = buckets;
        _entries = entries;
        return size;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref int GetBucket(uint hashCode)
    {
        int[] buckets = _buckets!;
        return ref buckets[hashCode % buckets.Length];
    }

    private ref TValue FindValueRef(TKey key)
    {
        ref DictEntry entry = ref Notsafe.NullRef<DictEntry>();
        IEqualityComparer<TKey> keyComparer = _keyEqualityComparer;
        uint hashCode = GetKeyHash(key, keyComparer);
        int i = GetBucket(hashCode);
        DictEntry[]? entries = _entries;
        uint collisionCount = 0;
        i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
        do
        {
            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
            // Test in if to drop range check for following array access
            if ((uint)i >= (uint)entries.Length)
            {
                goto ReturnNotFound;
            }

            entry = ref entries[i];
            if (entry.KeyHashCode == hashCode && keyComparer.Equals(entry.Key!, key!))
            {
                goto ReturnFound;
            }

            i = entry.NextEntryIndex;

            collisionCount++;
        }
        while (collisionCount <= (uint)entries.Length);

        // The chain of entries forms a loop; which means a concurrent update has happened.
        // Break out of the loop and throw, rather than looping forever.
        Throw.IfConcurrentOperation(true);

    ReturnFound:
        ref TValue value = ref entry.Value;
    Return:
        return ref value;
    ReturnNotFound:
        value = ref Notsafe.NullRef<TValue>();
        goto Return;
    }


    private bool TryInsert(TKey key, TValue value, InsertionBehavior behavior)
    {
        DictEntry[] entries = _entries;
        IEqualityComparer<TKey> keyComparer = _keyEqualityComparer;
        uint keyHash = GetKeyHash(key, keyComparer);

        uint collisionCount = 0;
        ref int bucket = ref GetBucket(keyHash);
        int i = bucket - 1; // Value in _buckets is 1-based

        while (true)
        {
            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
            // Test uint in if rather than loop condition to drop range check for following array access
            if ((uint)i >= (uint)entries.Length)
            {
                break;
            }

            if (entries[i].KeyHashCode == keyHash && keyComparer.Equals(entries[i].Key!, key!))
            {
                if (behavior == InsertionBehavior.OverwriteExisting)
                {
                    entries[i].Value = value;
                    return true;
                }

                if (behavior == InsertionBehavior.ThrowOnExisting)
                {
                    Throw.DuplicateKeyException(key);
                }

                return false;
            }

            i = entries[i].NextEntryIndex;

            collisionCount++;
            if (collisionCount > (uint)entries.Length)
            {
                // The chain of entries forms a loop; which means a concurrent update has happened.
                // Break out of the loop and throw, rather than looping forever.
                Throw.IfConcurrentOperation(true);
            }
        }

        int index;
        if (_freeCount > 0)
        {
            index = _freeList;
            Debug.Assert((START_OF_FREE_LIST - entries[_freeList].NextEntryIndex) >= -1, "shouldn't overflow because `NextEntryIndex` cannot underflow");
            _freeList = START_OF_FREE_LIST - entries[_freeList].NextEntryIndex;
            _freeCount--;
        }
        else
        {
            int count = _count;
            if (count == entries.Length)
            {
                Resize();
                bucket = ref GetBucket(keyHash);
            }

            index = count;
            _count = count + 1;
            entries = _entries;
        }

        ref DictEntry entry = ref entries[index];
        entry.KeyHashCode = keyHash;
        entry.NextEntryIndex = bucket - 1; // Value in _buckets is 1-based
        entry.Key = key;
        entry.Value = value;
        bucket = index + 1; // Value in _buckets is 1-based
        _version++;

        if (collisionCount > Hasher.HashCollisionThreshold)
        {
            Resize(entries.Length);
        }

        return true;
    }


    private void Resize() => Resize(Hasher.ExpandPrime(_count));

    private void Resize(int newSize)
    {
        Debug.Assert(newSize >= _entries.Length);

        DictEntry[] entries = new DictEntry[newSize];

        int count = _count;
        _entries.AsSpan(0, count).CopyTo(entries);

        // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
        _buckets = new int[newSize];

        for (int i = 0; i < count; i++)
        {
            if (entries[i].NextEntryIndex >= -1)
            {
                ref int bucket = ref GetBucket(entries[i].KeyHashCode);
                entries[i].NextEntryIndex = bucket - 1; // Value in _buckets is 1-based
                bucket = i + 1;
            }
        }

        _entries = entries;
    }

    private void CopyEntries(DictEntry[] entries, int count)
    {
        DictEntry[] newEntries = _entries;
        int newCount = 0;
        for (int i = 0; i < count; i++)
        {
            uint hashCode = entries[i].KeyHashCode;
            if (entries[i].NextEntryIndex >= -1)
            {
                ref DictEntry entry = ref newEntries[newCount];
                entry = entries[i];
                ref int bucket = ref GetBucket(hashCode);
                entry.NextEntryIndex = bucket - 1; // Value in _buckets is 1-based
                bucket = newCount + 1;
                newCount++;
            }
        }

        _count = newCount;
        _freeCount = 0;
    }

#endregion NonPublic Methods


    public void Add(TKey key, TValue value)
    {
        bool modified = TryInsert(key, value, InsertionBehavior.ThrowOnExisting);

        // If there was an existing Key and the Add failed, an exception will already have been thrown.
        Debug.Assert(modified);
    }

    void ICollection<Pair<TKey, TValue>>.Add(Pair<TKey, TValue> pair) => Add(pair.Key, pair.Value);

    public Result<Pair<TKey, TValue>, Exception> TryAdd(TKey key, TValue value)
    {
        if (!TryInsert(key, value, InsertionBehavior.None))
            return new ArgumentException($"Duplicate key '{key}' was found", nameof(key));
        return Pair(key, value);
    }


    public bool ContainsKey(TKey key) => !Notsafe.IsNullRef(ref FindValueRef(key));

    public bool ContainsValue(TValue value, IEqualityComparer<TValue>? valueEqualityComparer = null)
    {
        DictEntry[]? entries = _entries;

        var valueComparer = Equate.With(valueEqualityComparer);
        for (int i = 0; i < _count; i++)
        {
            if (entries![i].NextEntryIndex >= -1 && valueComparer.Equals(entries[i].Value, value))
            {
                return true;
            }
        }

        return false;
    }

    public bool Contains(TKey key, TValue value, IEqualityComparer<TValue>? valueEqualityComparer = null)
    {
        ref TValue exValue = ref FindValueRef(key);
        return Notsafe.IsNonNullRef(ref exValue) &&
            Equate.With(valueEqualityComparer).Equals(exValue, value);
    }

    public bool Contains(Pair<TKey, TValue> pair) => Contains(pair.Key, pair.Value);

    public bool Contains(Pair<TKey, TValue> pair, IEqualityComparer<TValue>? valueEqualityComparer) => Contains(pair.Key, pair.Value, valueEqualityComparer);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        ref TValue valRef = ref FindValueRef(key);
        if (!Notsafe.IsNullRef(ref valRef))
        {
            value = valRef;
            return true;
        }

        value = default;
        return false;
    }

    bool ICollection<Pair<TKey, TValue>>.Remove(Pair<TKey, TValue> pair) => TryRemove(pair);

    public Result<TValue, Exception> TryRemove(TKey key)
    {
        uint collisionCount = 0;
        TValue value;
        IEqualityComparer<TKey> comparer = _keyEqualityComparer;
        uint keyHash = GetKeyHash(key, comparer);
        ref int bucket = ref GetBucket(keyHash);
        DictEntry[] entries = _entries;
        int last = -1;
        int i = bucket - 1; // Value in buckets is 1-based
        while (i >= 0)
        {
            ref DictEntry entry = ref entries[i];

            if (entry.KeyHashCode == keyHash &&
                (typeof(TKey).IsValueType && comparer == null ? EqualityComparer<TKey>.Default.Equals(entry.Key, key) : comparer!.Equals(entry.Key, key)))
            {
                if (last < 0)
                {
                    bucket = entry.NextEntryIndex + 1; // Value in buckets is 1-based
                }
                else
                {
                    entries[last].NextEntryIndex = entry.NextEntryIndex;
                }

                value = entry.Value;

                Debug.Assert((START_OF_FREE_LIST - _freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                entry.NextEntryIndex = START_OF_FREE_LIST - _freeList;

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
                    {
#endif
                entry.Key = default!;
#if NETSTANDARD2_1 || NET6_0_OR_GREATER
                    }

                    if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
                    {
#endif
                entry.Value = default!;
#if NETSTANDARD2_1 || NET6_0_OR_GREATER
                    }
#endif

                _freeList = i;
                _freeCount++;
                return Ok(value);
            }

            last = i;
            i = entry.NextEntryIndex;

            collisionCount++;
            if (collisionCount > (uint)entries.Length)
            {
                // The chain of entries forms a loop; which means a concurrent update has happened.
                // Break out of the loop and throw, rather than looping forever.
                return new InvalidOperationException("Concurrent operations are not supported");
            }
        }

        return new KeyNotFoundException($"Key '{key}' was not found");
    }


    public Result<Pair<TKey, TValue>, Exception> TryRemove(Pair<TKey, TValue> pair)
    {
        ref TValue value = ref FindValueRef(pair.Key);
        if (!Notsafe.IsNullRef(ref value) && EqualityComparer<TValue>.Default.Equals(value, pair.Value))
        {
            return TryRemove(pair.Key).Select(v => pair);
        }

        return new KeyNotFoundException($"Key '{pair.Key}' was not found");
    }

    /// <summary>
    /// Sets the capacity of this dictionary to what it would be if it had been originally initialized with all its entries
    /// </summary>
    /// <remarks>
    /// This method can be used to minimize the memory overhead
    /// once it is known that no new elements will be added.
    ///
    /// To allocate minimum size storage array, execute the following statements:
    ///
    /// dictionary.Clear();
    /// dictionary.TrimExcess();
    /// </remarks>
    public void TrimExcess() => TrimExcess(Count);

    /// <summary>
    /// Sets the capacity of this dictionary to hold up 'capacity' entries without any further expansion of its backing storage
    /// </summary>
    /// <remarks>
    /// This method can be used to minimize the memory overhead
    /// once it is known that no new elements will be added.
    /// </remarks>
    public void TrimExcess(int capacity)
    {
        if (capacity < Count)
            capacity = Count;

        int newSize = Hasher.GetPrime(capacity);
        DictEntry[]? oldEntries = _entries;
        int currentCapacity = oldEntries == null ? 0 : oldEntries.Length;
        if (newSize >= currentCapacity)
        {
            return;
        }

        int oldCount = _count;
        _version++;
        Initialize(newSize);

        Debug.Assert(oldEntries is not null);

        CopyEntries(oldEntries!, oldCount);
    }

    public void Clear()
    {
        int count = _count;
        if (count > 0)
        {
            Array.Clear(_buckets, 0, _buckets.Length);

            _count = 0;
            _freeList = -1;
            _freeCount = 0;
            Array.Clear(_entries, 0, count);
        }
    }

    public void CopyTo(Pair<TKey, TValue>[] array, int arrayIndex = 0)
    {
        Validate.CopyTo(Count, array, arrayIndex).OkOrThrow();
        int count = _count;
        DictEntry[]? entries = _entries;
        for (int i = 0; i < count; i++)
        {
            if (entries![i].NextEntryIndex >= -1)
            {
                array[arrayIndex++] = new(entries[i].Key, entries[i].Value);
            }
        }
    }

    object ICloneable.Clone() => (object)Clone();

    public UberDict<TKey, TValue> Clone()
    {
        UberDict<TKey, TValue> clone = new();

        if (Count == 0)
        {
            // Nothing to copy, all done
            return clone;
        }

        clone.EnsureCapacity(Count);
        clone.CopyEntries(_entries!, _count);

        return clone;
    }

    /// <summary>
    /// Ensures that the dictionary can hold up to 'capacity' entries without any further expansion of its backing storage
    /// </summary>
    public int EnsureCapacity(int capacity)
    {
        int currentCapacity = _entries.Length;
        if (currentCapacity >= capacity)
        {
            return currentCapacity;
        }

        _version++;

        int newSize = Hasher.GetPrime(capacity);
        Resize(newSize);
        return newSize;
    }


    IEnumerator IEnumerable.GetEnumerator() => Count == 0 ? EmptyEnumerator<Pair<TKey, TValue>>.Instance : GetEnumerator();
    IEnumerator<Pair<TKey, TValue>> IEnumerable<Pair<TKey, TValue>>.GetEnumerator() => Count == 0 ? EmptyEnumerator<Pair<TKey, TValue>>.Instance : GetEnumerator();

    public UberDictEnumerator GetEnumerator() => new UberDictEnumerator(this);


    [MustDisposeResource(false)]
    public struct UberDictEnumerator : IEnumerator<Pair<TKey, TValue>>, IEnumerator
    {
        private readonly UberDict<TKey, TValue> _dictionary;
        private readonly int _version;

        private int _index;
        private Pair<TKey, TValue> _current;

        object? IEnumerator.Current => (object?)_current;
        public Pair<TKey, TValue> Current => _current;

        internal UberDictEnumerator(UberDict<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
            _version = dictionary._version;
            _index = 0;
            _current = default;
        }

        public bool MoveNext()
        {
            Throw.IfEnumerationChanged(_version != _dictionary._version);

            // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
            // dictionary.count+1 could be negative if dictionary.count is int.MaxValue
            while ((uint)_index < (uint)_dictionary._count)
            {
                ref DictEntry entry = ref _dictionary._entries![_index++];

                if (entry.NextEntryIndex >= -1)
                {
                    _current = new(entry.Key, entry.Value);
                    return true;
                }
            }

            _index = _dictionary._count + 1;
            _current = default;
            return false;
        }

        void IEnumerator.Reset()
        {
            Throw.IfEnumerationChanged(_version != _dictionary._version);
            _index = 0;
            _current = default;
        }

        void IDisposable.Dispose()
        {
        }
    }

    public sealed class UberDictKeys :
        ICollection<TKey>, // supported only as a condition for IDictionary support
        IReadOnlyCollection<TKey>
    // TODO: IReadOnlySet<TKey>
    {
        private readonly UberDict<TKey, TValue> _dictionary;

        bool ICollection<TKey>.IsReadOnly => true;

        public int Count => _dictionary.Count;

        public UberDictKeys(UberDict<TKey, TValue> dictionary)
        {
            Throw.IfNull(dictionary);
            _dictionary = dictionary;
        }

        void ICollection<TKey>.Add(TKey _) => throw new NotSupportedException();

        void ICollection<TKey>.Clear() => throw new NotSupportedException();

        bool ICollection<TKey>.Remove(TKey _) => throw new NotSupportedException();

        public bool Contains(TKey key) => _dictionary.ContainsKey(key);

        public void CopyTo(TKey[] array, int arrayIndex = 0)
        {
            int count = _dictionary._count;
            Validate.CopyTo(count, array, arrayIndex).OkOrThrow();

            DictEntry[]? entries = _dictionary._entries;
            for (int i = 0; i < count; i++)
            {
                if (entries![i].NextEntryIndex >= -1)
                    array[arrayIndex++] = entries[i].Key;
            }
        }

        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public KeysEnumerator GetEnumerator() => new KeysEnumerator(_dictionary);


        [MustDisposeResource(false)]
        public struct KeysEnumerator : IEnumerator<TKey>, IEnumerator
        {
            private readonly UberDict<TKey, TValue> _dictionary;
            private readonly int _version;

            private int _index;
            private TKey? _currentKey;


            public TKey Current => _currentKey!;
            object? IEnumerator.Current => (object?)_currentKey;

            internal KeysEnumerator(UberDict<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = 0;
                _currentKey = default;
            }

            public bool MoveNext()
            {
                Throw.IfEnumerationChanged(_version != _dictionary._version);

                while ((uint)_index < (uint)_dictionary._count)
                {
                    ref DictEntry entry = ref _dictionary._entries![_index++];

                    if (entry.NextEntryIndex >= -1)
                    {
                        _currentKey = entry.Key;
                        return true;
                    }
                }

                _index = _dictionary._count + 1;
                _currentKey = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                Throw.IfEnumerationChanged(_version != _dictionary._version);

                _index = 0;
                _currentKey = default;
            }

            void IDisposable.Dispose()
            {
            }
        }
    }


    public sealed class UberDictValues : IReadOnlyCollection<TValue>
    {
        private readonly UberDict<TKey, TValue> _dictionary;

        public int Count => _dictionary.Count;

        public UberDictValues(UberDict<TKey, TValue> dictionary)
        {
            Throw.IfNull(dictionary);
            _dictionary = dictionary;
        }

        public bool Contains(TValue value) => _dictionary.ContainsValue(value);

        public void CopyTo(TValue[] array, int arrayIndex = 0)
        {
            Validate.CopyTo(_dictionary.Count, array, arrayIndex).OkOrThrow();

            int count = _dictionary._count;
            DictEntry[]? entries = _dictionary._entries;
            for (int i = 0; i < count; i++)
            {
                if (entries![i].NextEntryIndex >= -1)
                    array[arrayIndex++] = entries[i].Value;
            }
        }

        [MustDisposeResource(false)]
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

        [MustDisposeResource(false)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MustDisposeResource(false)]
        public ValuesEnumerator GetEnumerator() => new ValuesEnumerator(_dictionary);

        [MustDisposeResource(false)]
        public struct ValuesEnumerator : IEnumerator<TValue>, IEnumerator
        {
            private readonly UberDict<TKey, TValue> _dictionary;
            private readonly int _version;

            private int _index;
            private TValue? _currentValue;

            public TValue Current => _currentValue!;
            object? IEnumerator.Current => (object?)_currentValue;

            internal ValuesEnumerator(UberDict<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = 0;
                _currentValue = default;
            }

            public bool MoveNext()
            {
                Throw.IfEnumerationChanged(_version != _dictionary._version);

                while ((uint)_index < (uint)_dictionary._count)
                {
                    ref DictEntry entry = ref _dictionary._entries![_index++];

                    if (entry.NextEntryIndex >= -1)
                    {
                        _currentValue = entry.Value;
                        return true;
                    }
                }

                _index = _dictionary._count + 1;
                _currentValue = default;
                return false;
            }


            void IEnumerator.Reset()
            {
                Throw.IfEnumerationChanged(_version != _dictionary._version);
                _index = 0;
                _currentValue = default;
            }

            void IDisposable.Dispose()
            {
            }
        }
    }
}