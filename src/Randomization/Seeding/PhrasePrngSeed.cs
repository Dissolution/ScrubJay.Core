using System.Text;

namespace ScrubJay.Randomization.Seeding;

[PublicAPI]
public sealed class PhrasePrngSeed : KnownPrngSeeds
{
    private static ulong[] GetSeeds(string phrase)
    {
        byte[] phraseBytes = Encoding.UTF32.GetBytes(phrase);

        int count = phraseBytes.Length / sizeof(ulong);
        int mod = phraseBytes.Length % sizeof(ulong);

        var seeds = new ulong[count];

        if (mod == 0)
        {
            Notsafe.Bytes.CopyTo(phraseBytes, MemoryMarshal.Cast<ulong, byte>(seeds), phraseBytes.Length);
        }
        else
        {
            throw new NotImplementedException();
        }

        return seeds;
    }

    public string Phrase { get; }

    public PhrasePrngSeed(string phrase) : base(GetSeeds(phrase))
    {
        this.Phrase = phrase;
    }

    public override string ToString() => $"Seed Phrase \"{Phrase}\"";
}