﻿using System.Collections.Concurrent;

namespace ScrubJay.Extensions;

public static class DictionaryExtensions
{
    public static TValue AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
        where TKey : notnull
    {
        return dictionary.AddOrUpdate(key, value, (_, _) => value);
    }
    
    public static TValue AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
        where TKey : notnull
    {
        dictionary[key] = value;
        return value;
    }
    
    public static TValue AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue addValue,
        Func<TKey, TValue, TValue> updateValueFactory)
        where TKey : notnull
    {
        TValue newValue;
        if (dictionary.TryGetValue(key, out TValue? existingValue))
        {
            newValue = updateValueFactory(key, existingValue);
        }
        else
        {
            newValue = addValue;
        }
        dictionary[key] = newValue;
        return newValue;
    }

    public static TValue AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> valueFactory,
        Func<TKey, TValue, TValue> updateValueFactory)
        where TKey : notnull
    {
        TValue newValue;
        if (dictionary.TryGetValue(key, out TValue? existingValue))
        {
            newValue = updateValueFactory(key, existingValue);
        }
        else
        {
            newValue = valueFactory();
        }
        dictionary[key] = newValue;
        return newValue;
    }

    public static TValue AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> addValueFactory,
        Func<TKey, TValue, TValue> updateValueFactory)
        where TKey : notnull
    {
        TValue newValue;
        if (dictionary.TryGetValue(key, out TValue? existingValue))
        {
            newValue = updateValueFactory(key, existingValue);
        }
        else
        {
            newValue = addValueFactory(key);
        }
        dictionary[key] = newValue;
        return newValue;
    }

    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out TValue? existingValue))
        {
            return existingValue;
        }
        dictionary[key] = value;
        return value;
    }

    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> valueFactory)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out TValue? existingValue))
        {
            return existingValue;
        }
        TValue newValue = valueFactory(key);
        dictionary[key] = newValue;
        return newValue;
    }
}