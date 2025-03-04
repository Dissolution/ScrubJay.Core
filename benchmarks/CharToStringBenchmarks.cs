namespace ScrubJay.Benchmarks;

public class CharToStringBenchmarks
{
    public static IEnumerable<char> Characters()
    {
        yield return '\0';
        yield return ',';
        yield return (char)127;
        yield return char.MaxValue;
    }


    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Characters))]
    public string CharToString(char ch)
    {
        return ch.ToString();
    }


    [Benchmark]
    [ArgumentsSource(nameof(Characters))]
    public string NewStringFromChar(char ch)
    {
        return new string(ch, 1);
    }


    [Benchmark]
    [ArgumentsSource(nameof(Characters))]
    public unsafe string NewStringFromPointer(char ch)
    {
        char* ptr = &ch;
        return new string(ptr, 0, 1);
    }
}
