using System.Runtime.Remoting;

namespace ScrubJay.Core.Tests;

public class OptionNoneTests
{
    public static TheoryData<None, None> TestData => TheoryDataHelper.AllCombinations<None>(
            new None(),
            default(None),
            Activator.CreateInstance<None>(),
            Option<string>.None,
            
    
    [Theory]
    [MemberData(nameof(TestData))]
    public void NoneIsNone(None none)
    {
        
        
    }
}

public class OptionTests
{
    

}