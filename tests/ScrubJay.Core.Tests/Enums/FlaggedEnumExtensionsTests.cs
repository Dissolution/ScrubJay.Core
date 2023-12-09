using System.Reflection;
using ScrubJay.Enums;

using static ScrubJay.Tests.Enums.EnumTestData;



namespace ScrubJay.Tests.Enums;

public class FlaggedEnumExtensionsTests
{
    
    [Fact]
    public void IsDefaultWorks()
    {
        Assert.True(EnumExtensions.IsDefault(default(Flagged)));
        Assert.True(EnumExtensions.IsDefault(Flagged.Default));
        Assert.True(EnumExtensions.IsDefault((Flagged)0));
        
        Assert.False(EnumExtensions.IsDefault(Flagged.Ichi));
        Assert.False(EnumExtensions.IsDefault(Flagged.Ni | Flagged.San));
        Assert.False(EnumExtensions.IsDefault((Flagged)3));
    }
    
    [Fact]
    public void EqualWorks()
    {
        Assert.True(EnumExtensions.Equal(default(Flagged), default(Flagged)));
        Assert.True(EnumExtensions.Equal(default(Flagged), (Flagged)0));
        Assert.True(EnumExtensions.Equal(Flagged.Ni, Flagged.Ni));
        Assert.True(EnumExtensions.Equal(Flagged.San | Flagged.Ichi, Flagged.Ichi | Flagged.San));
        Assert.True(EnumExtensions.Equal(Flagged.Ichi | Flagged.Ni, (Flagged)3));
        
        Assert.False(EnumExtensions.Equal(default(Flagged), Flagged.San));
        Assert.False(EnumExtensions.Equal(Flagged.Yon, Flagged.Ichi));
        Assert.False(EnumExtensions.Equal(Flagged.Ni, Flagged.Ni | Flagged.San));
        Assert.False(EnumExtensions.Equal((Flagged)3, (Flagged)2));
    }
}