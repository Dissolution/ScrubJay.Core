using ScrubJay.Text;

namespace ScrubJay.Tests.TextTests;

public class FluentIndentTextBuilderTests
{
    [Fact]
    public void StartAndEndIndentWorks()
    {
        const string OUTPUT = """
        L1
        L2L3
        ----L4
        ----L5L6
        L7
        L8
        """;

        string? text = IndentTextBuilder.New
            .Append("L1")
            .NewLine()
            .Append("L2")
            .AddIndent("----")
            .Append("L3")
            .NewLine()
            .Append("L4")
            .NewLine()
            .Append("L5")
            .RemoveIndent(out string? indent)
            .Append("L6")
            .NewLine()
            .Append("L7")
            .NewLine()
            .Append("L8")
            .ToStringAndDispose();

        Assert.Equal("----", indent);
        Assert.Equal(OUTPUT, text);
    }


    [Fact]
    public void IndentsWork()
    {
        using var builder = new IndentTextBuilder();
        builder.Append("Start")
            .Indented(
                "   ", b =>
                {
                    b.NewLine()
                        .AppendLine("level 1, A")
                        .Append("level 1, B");
                })
            .NewLine()
            .Append("End");

        const string OUTPUT = """
            Start
               level 1, A
               level 1, B
            End
            """;

        Assert.Equal(OUTPUT, builder.ToString());
    }


    [Fact]
    public void NewLinesAreParsedInText()
    {
        const string SAMPLE = """
            Come with me,
            into the trees.
            We lay on the grass,
            and let hours pass.
            """;

        const string SAMPLE_INDENTED = """
            Come with me,
                into the trees.
                We lay on the grass,
                and let hours pass.
            """;

        using var builder = new IndentTextBuilder();

        builder.Indented(
            "    ", b => b
                .Append(SAMPLE));

        string? output = builder.ToString();

        Assert.Equal(SAMPLE_INDENTED, output);
    }

    [Fact]
    public void PlaceholdersAreParsed()
    {
        string className = "Test";

        Action<IndentTextBuilder> ba = tb => tb.Append("public void DoThing() => { };");

        using var builder = new IndentTextBuilder();
        builder.Append(
            $$"""
            public void {{className}}()
            {
                {{ba}}
            }
            """);

        string output = """
            public void Test()
            {
                public void DoThing() => { };
            }
            """;
        Assert.Equal(output, builder.ToString());
    }

    private static void WriteBody(IndentTextBuilder builder)
    {
        builder.Delimit(static b => b.NewLine(), Enumerable.Range(0, 10), static (b, i) => b.Append(i));
    }

    [Fact]
    public void ComplexInterpolationWorks()
    {
        using var builder = new IndentTextBuilder();
        builder.Append(
            $$"""
            public class TestClass()
            {
                public void DoThing()
                {
                    {{WriteBody}}
                }
            }
            """);

        const string OUTPUT = $$"""
            public class TestClass()
            {
                public void DoThing()
                {
                    0
                    1
                    2
                    3
                    4
                    5
                    6
                    7
                    8
                    9
                }
            }
            """;

        string builderString = builder.ToString();
        Assert.Equal(OUTPUT, builderString);
    }
}
