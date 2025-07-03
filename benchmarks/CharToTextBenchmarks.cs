namespace ScrubJay.Benchmarks;

public class CharToTextBenchmarks
{
    public static IEnumerable<object[]> Characters()
    {
        yield return ['\0'];
        yield return [','];
        yield return [char.MaxValue];
    }


    [ArgumentsSource(nameof(Characters))]
    public ReadOnlySpan<char> CharToStringAsSpan(char ch)
    {
        return ch.ToString().AsSpan();
    }

    [Benchmark]
    [ArgumentsSource(nameof(Characters))]
    public ReadOnlySpan<char> NewTextFromCharArray(char ch)
    {
        return new ReadOnlySpan<char>([ch]);
    }


    [Benchmark]
    [ArgumentsSource(nameof(Characters))]
    public unsafe ReadOnlySpan<char> NewTextFromPointer(char ch)
    {
        char* ptr = &ch;
        return new ReadOnlySpan<char>(ptr, 1);
    }
}
