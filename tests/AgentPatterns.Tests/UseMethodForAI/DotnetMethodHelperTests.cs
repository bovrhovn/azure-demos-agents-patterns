using AP.UseMethodForAI;

namespace AgentPatterns.Tests.UseMethodForAI;

public class DotnetMethodHelperTests
{
    [Fact]
    public void CalculateTax_MethodCustomer_DoubleMonths()
    {
        var result = DotnetMethodHelper.CalculateTax("Method", 3);
        Assert.Contains("6", result);
    }

    [Fact]
    public void CalculateTax_TaxCustomer_HalfMonths()
    {
        var result = DotnetMethodHelper.CalculateTax("Tax", 4);
        Assert.Contains("2", result);
    }

    [Fact]
    public void CalculateTax_UnknownCustomer_ReturnsMonthsUnchanged()
    {
        var result = DotnetMethodHelper.CalculateTax("Alice", 5);
        Assert.Contains("5", result);
    }

    [Fact]
    public void CalculateTax_ReturnsStringContainingCustomerName()
    {
        var result = DotnetMethodHelper.CalculateTax("Method", 3);
        Assert.Contains("Method", result);
    }

    [Fact]
    public void CalculateTax_ReturnsStringContainingMonthCount()
    {
        var result = DotnetMethodHelper.CalculateTax("Tax", 6);
        Assert.Contains("6", result);
    }

    [Fact]
    public void CalculateTax_MethodCustomer_ZeroMonths_ReturnsZero()
    {
        var result = DotnetMethodHelper.CalculateTax("Method", 0);
        Assert.Contains("0", result);
    }

    [Fact]
    public void CalculateTax_TaxCustomer_OddMonths_ReturnsHalf()
    {
        // 3 / 2.0 = 1.5, but decimal separator is culture-dependent
        var result = DotnetMethodHelper.CalculateTax("Tax", 3);
        // The result value contains the halved number (may be "1.5" or "1,5" depending on culture)
        var expected = ((double)3 / 2).ToString();
        Assert.Contains(expected, result);
    }

    [Fact]
    public void CalculateTax_ResultIsNonEmptyString()
    {
        var result = DotnetMethodHelper.CalculateTax("Method", 1);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Theory]
    [InlineData("Method", 2, "4")]
    [InlineData("Method", 5, "10")]
    [InlineData("Tax", 2, "1")]
    [InlineData("Tax", 10, "5")]
    [InlineData("Other", 7, "7")]
    public void CalculateTax_VariousInputs_CorrectValueInResult(string customer, int months, string expectedValue)
    {
        var result = DotnetMethodHelper.CalculateTax(customer, months);
        Assert.Contains(expectedValue, result);
    }
}
