using ScrubJay.Validation;

namespace ScrubJay.Tests.ValidationTests;

public class ValidationsTests
{
    [Fact]
    public void EmptyValidationsDoNotThrow()
    {
        new Validations().ThrowIfErrors();
    }

    [Fact]
    public void SingleExceptionThrowsThatException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new Validations()
            {
                Validate.IsNotNull(147),
                Validate.Is<int>(Guid.NewGuid()),
                Validate.IsGreaterThan(4, 3),
            }.ThrowIfErrors();
        });
    }
}
