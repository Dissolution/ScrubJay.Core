using ScrubJay.Maths.Ternary;
using static ScrubJay.Maths.Ternary.Trit;

namespace ScrubJay.Tests.MathsTests;

public class TritTests
{
    public static IEnumerable<(Trit, Trit)> NotData()
    {
        yield return (True, False);
        yield return (Unknown, Unknown);
        yield return (False, True);
    }

    [Theory]
    [MemberData(nameof(NotData))]
    public void NotOperatorWorks(Trit trit, Trit expected)
    {
        var result = !trit;
        Demand.That(result).IsEqualTo(expected);
    }

    public static IEnumerable<(Trit, Trit, Trit)> AndData()
    {
        yield return (False, False, False);
        yield return (Unknown, False, False);
        yield return (True, False, False);
        yield return (False, Unknown, False);
        yield return (Unknown, Unknown, Unknown);
        yield return (True, Unknown, Unknown);
        yield return (False, True, False);
        yield return (Unknown, True, Unknown);
        yield return (True, True, True);
    }

    [Theory]
    [MemberData(nameof(AndData))]
    public void AndOperatorWorks(Trit left, Trit right, Trit expected)
    {
        var result = left & right;
        Demand.That(result).IsEqualTo(expected);
    }

    public static IEnumerable<(Trit, Trit, Trit)> OrData()
    {
        yield return (False, False, False);
        yield return (Unknown, False, Unknown);
        yield return (True, False, True);
        yield return (False, Unknown, Unknown);
        yield return (Unknown, Unknown, Unknown);
        yield return (True, Unknown, True);
        yield return (False, True, True);
        yield return (Unknown, True, True);
        yield return (True, True, True);
    }

    [Theory]
    [MemberData(nameof(OrData))]
    public void OrOperatorWorks(Trit left, Trit right, Trit expected)
    {
        var result = left | right;
        Demand.That(result).IsEqualTo(expected);
    }

    [Fact]
    public void TrueOperatorWorks()
    {
        if (True)
        {
            // okay!
        }
        else
        {
            Demand.Fail("Trit.True did not evaluate as `true`");
        }

        if (Unknown)
        {
            Demand.Fail("Trit.Unknown evaluated as `true`");
        }

        if (False)
        {
            Demand.Fail("Trit.False evaluated as `true`");
        }
    }

}