// using ScrubJay.Text;
//
// namespace ScrubJay.Tests.TextTests.TextBuilderTests;
//
// public class BlockTests
// {
//     [Fact]
//     public void BlockFromEmptyWorks()
//     {
//         using var builder = new TextBuilder();
//         builder.Block(TextBuilder.BlockSpec.Allman, static tb => tb.Append('x'));
//         string str = builder.ToString();
//
//         const string EXPECTED = """
//             {
//                 x
//             }
//
//             """;
//
//         Assert.Equal(EXPECTED, str);
//     }
//
//     [Fact]
//     public void BlockFromNewlineWorks()
//     {
//         string str = TextBuilder
//             .New
//             .Append("TRJ")
//             .NewLine()
//             .Block(TextBuilder.BlockSpec.Allman, static tb => tb.Append('x'))
//             .ToStringAndDispose();
//
//         const string EXPECTED = """
//             TRJ
//             {
//                 x
//             }
//
//             """;
//
//         Assert.Equal(EXPECTED, str);
//     }
//
//     [Fact]
//     public void BlockFromOffsetWorks()
//     {
//         string str = TextBuilder
//             .New
//             .Append("TRJ")
//             .Block(TextBuilder.BlockSpec.Allman, static tb => tb.Append('x'))
//             .ToStringAndDispose();
//
//         const string EXPECTED = """
//             TRJ
//             {
//                 x
//             }
//
//             """;
//
//         Assert.Equal(EXPECTED, str);
//     }
//
//     [Fact]
//     public void BlockWithNewlineWorks()
//     {
//         string str = TextBuilder
//             .New
//             .Block(TextBuilder.BlockSpec.Allman, static tb => tb.AppendLine('x'))
//             .ToStringAndDispose();
//
//         const string EXPECTED = """
//             {
//                 x
//             }
//
//             """;
//
//         Assert.Equal(EXPECTED, str);
//     }
// }