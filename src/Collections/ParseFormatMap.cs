// using System.Collections;
// using System.Collections.Concurrent;
// using System.Diagnostics.CodeAnalysis;
// using System.Numerics;
// using ScrubJay.Exceptions;
//
// namespace ScrubJay.Decka;
//
// public sealed class AddingToParseFormatMap<T, F>
// {
//     private readonly ParseFormatMap<T, F> _map;
//
//     public T Value { get; }
//
//     public AddingToParseFormatMap(ParseFormatMap<T, F> map, T value)
//     {
//         _map = map;
//         Value = value;
//     }
//
//     public AddingToParseFormatMap<T, F> Add(F format, string text)
//     {
//         _map.Add(Value, format, text);
//         return this;
//     }
// }
//
// public class ParseFormatMap<T, F> : IEnumerable
// {
//     private readonly record struct Key(T? Value, F? Format);
//
//     private sealed class KeyComparer : IEqualityComparer<Key>
//     {
//         private IEqualityComparer<T> _valueComparer;
//
//         private IEqualityComparer<F> _formatComparer;
//
//         public KeyComparer(
//             IEqualityComparer<T>? valueComparer,
//             IEqualityComparer<F>? formatComparer)
//         {
//             _valueComparer = valueComparer ?? Equate.GetComparer<T>();
//             _formatComparer = formatComparer ?? Equate.GetComparer<F>();
//         }
//
//         public bool Equals(Key x, Key y)
//         {
//             return _valueComparer.Equals(x.Value, y.Value) &&
//                    _formatComparer.Equals(x.Format, y.Format);
//         }
//
//         public int GetHashCode(Key key)
//         {
//             var hasher = new Hasher();
//             hasher.Add<T>(key.Value, _valueComparer);
//             hasher.Add<F>(key.Format, _formatComparer);
//             return hasher.ToHashCode();
//         }
//     }
//
//     private readonly IEqualityComparer<T> _valueComparer;
//     private readonly IEqualityComparer<F> _formatComparer;
//     private readonly IEqualityComparer<string> _stringComparer;
//
//     private readonly Dictionary<Key, string> _formats;
//     private readonly Dictionary<string, T> _parses;
//
//     public ParseFormatMap(
//         IEqualityComparer<T>? valueComparer = null,
//         IEqualityComparer<F>? formatComparer = null,
//         IEqualityComparer<string>? stringComparer = null)
//     {
//         _valueComparer = valueComparer ?? Equate.GetComparer<T>();
//         _formatComparer = formatComparer ?? Equate.GetComparer<F>();
//         _stringComparer = stringComparer ?? StringComparer.OrdinalIgnoreCase;
//         _formats = new(new KeyComparer(_valueComparer, _formatComparer));
//         _parses = new(_stringComparer);
//     }
//
//     public void Add(T value, F format, string text)
//     {
//         var key = new Key(value, format);
//         if (!_formats.TryAdd(key, text))
//             throw Ex.Argument(key, "Duplicate Value/Formatting Key");
//     }
//
//     public AddingToParseFormatMap<T, F> For(T value)
//     {
//         return new AddingToParseFormatMap<T, F>(this, value);
//     }
//
//     public Result<T> TryParse(string? str)
//     {
//         if (str is null)
//             return Ex.Parse<T>(str, "String is null");
//
//         if (_parses.TryGetValue(str, out var value))
//         {
//             return Ok<T>(value);
//         }
//
//         return Ex.Parse<T>(str);
//     }
//
//     public string ToString(T? value, F format)
//     {
//         var key = new Key(value, format);
//
//         if (_formats.TryGetValue(key, out var text))
//             return text;
//
//         return value?.ToString() ?? string.Empty;
//     }
//
//     public IEnumerator GetEnumerator()
//     {
//         throw Ex.NotImplemented();
//     }
// }