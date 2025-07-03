namespace ScrubJay.Benchmarks;

public class TextToStringBenchmarks
{
    public static IEnumerable<object[]> Args()
    {
        yield return [""];
        yield return [Environment.NewLine];
        yield return ["EE0A1999-5F25-411B-AA2C-ACB40D6778C1"];
        yield return [new string('x', 1000)];
    }


    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Args))]
    public string ReadOnlySpanToString(string str)
    {
        ReadOnlySpan<char> text = str.AsSpan();
        return text.ToString();
    }


    [Benchmark]
    [ArgumentsSource(nameof(Args))]
    public string NewStringFromReadOnlySpan(string str)
    {
        ReadOnlySpan<char> text = str.AsSpan();
#if NET481 || NETSTANDARD2_0
        return new string(text.ToArray());
#else
        return new string(text);
#endif
    }



    [Benchmark]
    [ArgumentsSource(nameof(Args))]
    public unsafe string NewStringFromPointer(string str)
    {
        ReadOnlySpan<char> text = str.AsSpan();
        fixed (char* ptr = text)
        {
            return new string(ptr, 0, text.Length);
        }
    }
}