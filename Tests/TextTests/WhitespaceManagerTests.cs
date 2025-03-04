// using ScrubJay.Functional;
// using ScrubJay.Text.Builders;
//
// namespace ScrubJay.Text.Tests;
//
// public class WhitespaceManagerTests
// {
//
//
//     [Fact]
//     public void StartEndSlicesWork()
//     {
//         using var whitespace = new WhitespaceManager();
//
//         string[] indents = ["\t", Environment.NewLine, "    "];
//
//         foreach (var indent in indents)
//         {
//             var started = whitespace.TryStartIndent(indent.AsSpan());
//             Assert.True(started.IsOk);
//         }
//
//         Result<string, Exception> ended;
//
//         for (var i = 0; i < indents.Length; i++)
//         {
//             ended = whitespace.TryEndIndent();
//             Assert.True(ended.HasOk(out var indent));
//             Assert.Equal(indents[^(i + 1)], indent);
//         }
//
//         ended = whitespace.TryEndIndent();
//         Assert.True(ended.IsError);
//     }
//
//     private static string WideName(int width) => $"W{width}";
//
//     private static string DeepName(int depth) => $"D{depth}";
//
//     [Theory, CombinatorialData]
//     public void NestedIndentWorks([CombinatorialRange(0, 4)] int width)
//     {
//         using var text = new TextBuilder();
//         using var whitespace = new WhitespaceManager();
//
//         Assert.False(whitespace.TryEndIndent());
//
//         for (var w = 0; w < width; w++)
//         {
//             var name = WideName(w);
//             whitespace.StartIndent(name);
//             Assert.Equal(name, whitespace.CurrentIndentString[(^name.Length)..]);
//             text.Append(name);
//             Assert.Equal(text.ToString(), whitespace.CurrentIndentString);
//         }
//
//         Assert.Equal(text.ToString(), whitespace.CurrentIndentString);
//
//         for (var w = width - 1; w >= 0; w--)
//         {
//             var name = WideName(w);
//             Assert.True(whitespace.TryEndIndent().HasOk(out var indent));
//             Assert.Equal(name, indent);
//             Assert.NotEqual(text.ToString(), whitespace.CurrentIndentString);
//             text.RemoveLast(name.Length);
//             Assert.Equal(text.ToString(), whitespace.CurrentIndentString);
//         }
//
//         Assert.False(whitespace.TryEndIndent());
//     }
//
//     [Theory, CombinatorialData]
//     public void IndentAndBlockWorks([CombinatorialRange(0, 3)] int depth, [CombinatorialRange(0,3)] int width)
//     {
//         using var whitespace = new WhitespaceManager();
//         using var text = new TextBuilder();
//
//         // Go Wide
//         for (var w = 0; w < width; w++)
//         {
//             string indent = $"w{w}";
//             whitespace.StartIndent(indent);
//             text.Append(indent);
//         }
//
//         string wide = text.ToString();
//
//         Assert.Equal(wide, whitespace.CurrentIndentString);
//
//         // Go deep
//         for (var d = 0; d < depth; d++)
//         {
//             whitespace.StartBlock();
//             Assert.Empty(whitespace.CurrentIndentString);
//             text.Clear();
//
//             // go wide
//             for (var w = 0; w < width; w++)
//             {
//                 string indent = $"w{w}";
//                 whitespace.StartIndent(indent);
//                 text.Append(indent);
//             }
//
//             wide = text.ToString();
//
//             Assert.Equal(wide, whitespace.CurrentIndentString);
//
//             // go narrow
//
//             for (var w = width - 1; w >= 0; w--)
//             {
//                 Assert.True(whitespace.TryEndIndent().HasOk(out var indent));
//
//                 // Slice off the end of text
//                 var textEnd = text.AsText().Slice(text.Length - indent.Length);
//                 Assert.Equal(textEnd.ToString(), indent);
//                 text.RemoveLast(indent.Length);
//             }
//
//             Assert.False(whitespace.TryEndIndent());
//             Assert.Equal(string.Empty, whitespace.CurrentIndentString);
//         }
//
//         // go shallow
//         for (var d = depth - 1; d >= 0; d--)
//         {
//             Assert.Equal(string.Empty, whitespace.CurrentIndentString);
//             Assert.True(whitespace.TryEndBlock(false));
//             Assert.Equal(wide, whitespace.CurrentIndentString);
//
//             // go wide
//             for (var w = 0; w < width; w++)
//             {
//                 string indent = $"w{w}";
//                 whitespace.StartIndent(indent);
//                 text.Append(indent);
//             }
//
//             wide = text.ToString();
//
//             Assert.Equal(wide, whitespace.CurrentIndentString);
//
//             // go narrow
//
//             for (var w = width - 1; w >= 0; w--)
//             {
//                 Assert.True(whitespace.TryEndIndent().HasOk(out var indent));
//
//                 // Slice off the end of text
//                 var textEnd = text.AsText().Slice(text.Length - indent.Length);
//                 Assert.Equal(textEnd.ToString(), indent);
//                 text.RemoveLast(indent.Length);
//             }
//
//             Assert.False(whitespace.TryEndIndent());
//             Assert.Equal(string.Empty, whitespace.CurrentIndentString);
//         }
//
//
//         Assert.Equal(wide, whitespace.CurrentIndentString);
//         Assert.False(whitespace.TryEndBlock());
//     }
// }