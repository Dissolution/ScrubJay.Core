// using ScrubJay.Comparison;
// using ScrubJay.Extensions;
// using ScrubJay.Memory;
// using ScrubJay.Validation;
// using ScrubJay.Validation.Demanding;
//
// namespace ScrubJay.Tests.MemoryTests;
//
// public class TextSplitTests
// {
//     public const string ALL_CHARS = "Sphinx of black quartz, judge my vow.";
//
//     [Fact]
//     public void EmptySplitItemYieldsOnce()
//     {
//         ReadOnlySpan<char> span = default;
//         var splitter = span.Split(' ');
//         //
//         // bool moved = splitter.TryMoveNext(out var segment);
//
//
//         Demand.That(splitter.TryMoveNext(out var segment)).IsTrue();
//         Demand.That(segment.Span).IsEmpty();
//         Demand.That(splitter.TryMoveNext(out segment)).IsFalse();
//     }
//
//     [Fact]
//     public void EmptySpanSeparatorsYieldsOnce()
//     {
//         ReadOnlySpan<char> span = default;
//         var splitter = span.Splitter("  ".AsSpan());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal(string.Empty, splitter.Current.ToString());
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void EmptySeparatorsYieldsOnce()
//     {
//         ReadOnlySpan<char> span = ALL_CHARS.AsSpan();
//         var splitter = span.Splitter("".AsSpan());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal(ALL_CHARS, splitter.Current.ToString());
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void UnmatchedSeparatorYieldsOnce()
//     {
//         ReadOnlySpan<char> span = ALL_CHARS.AsSpan();
//         var splitter = span.Splitter('_');
//         Assert.True(splitter.MoveNext());
//         Assert.Equal(ALL_CHARS, splitter.Current.ToString());
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void UnmatchedSeparatorsYieldsOnce()
//     {
//         ReadOnlySpan<char> span = ALL_CHARS.AsSpan();
//         var splitter = span.Splitter("kc".AsSpan());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal(ALL_CHARS, splitter.Current.ToString());
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void LargerSeparatorsYieldsOnce()
//     {
//         ReadOnlySpan<char> span = ALL_CHARS.AsSpan();
//         var splitter = span.Splitter($"{Guid.NewGuid()}{Guid.NewGuid()}{Guid.NewGuid()}".AsSpan());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal(ALL_CHARS, splitter.Current.ToString());
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void OneSeparatorMatchWorks()
//     {
//         ReadOnlySpan<char> span = ALL_CHARS.AsSpan();
//         var splitter = span.Splitter('q');
//         Assert.True(splitter.MoveNext());
//         Assert.Equal("Sphinx of black ", splitter.Current.ToString());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal("uartz, judge my vow.", splitter.Current.ToString());
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void ManySeparatorMatchesWorks()
//     {
//         ReadOnlySpan<char> span = ALL_CHARS.AsSpan();
//         var splitter = span.Splitter('u');
//         Assert.True(splitter.MoveNext());
//         Assert.Equal("Sphinx of black q", splitter.Current.ToString());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal("artz, j", splitter.Current.ToString());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal("dge my vow.", splitter.Current.ToString());
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void ExactSeparatorMatchYieldsEmptyTwice()
//     {
//         ReadOnlySpan<char> span = "X".AsSpan();
//         var splitter = span.Splitter('X');
//         Assert.True(splitter.MoveNext());
//         Assert.Equal(string.Empty, splitter.Current.ToString());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal(string.Empty, splitter.Current.ToString());
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void ExactSeparatorMatchWithIgnoreEmptyYieldsNothing()
//     {
//         ReadOnlySpan<char> span = "X".AsSpan();
//         var splitter = span.Splitter('X', SplitOptions.IgnoreEmpty);
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void ExactSeparatorsMatchYieldsEmptyTwice()
//     {
//         ReadOnlySpan<char> span = "XYZ".AsSpan();
//         var splitter = span.Splitter("XYZ".AsSpan());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal(string.Empty, splitter.Current.ToString());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal(string.Empty, splitter.Current.ToString());
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void ExactSeparatorsMatchWithIgnoreEmptyYieldsNothing()
//     {
//         ReadOnlySpan<char> span = "XYZ".AsSpan();
//         var splitter = span.Splitter("XYZ".AsSpan(), SplitOptions.IgnoreEmpty);
//         Assert.False(splitter.MoveNext());
//     }
//
//     [Fact]
//     public void OptionIgnoreEmptyWithSeparatorInARowWorks()
//     {
//         ReadOnlySpan<char> span = "AAAABBBCCD".AsSpan();
//         var splitter = span.Splitter('B', SplitOptions.IgnoreEmpty);
//         Assert.True(splitter.MoveNext());
//         Assert.Equal("AAAA", splitter.Current.ToString());
//         Assert.True(splitter.MoveNext());
//         Assert.Equal("CCD", splitter.Current.ToString());
//         Assert.False(splitter.MoveNext());
//     }
// }