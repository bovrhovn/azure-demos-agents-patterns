using AP.TaxServer.Tools;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AgentPatterns.Tests.TaxServer;

public class McpToolsTests
{
    private static McpTools CreateTools() =>
        new(Substitute.For<ILogger<McpTools>>());

    [Fact]
    public void CalculateTax_MethodCustomer_DoubleMonths()
    {
        var tools = CreateTools();
        var result = tools.CalculateTax("Method", 3);
        Assert.Contains("6", result);
    }

    [Fact]
    public void CalculateTax_TaxCustomer_HalfMonths()
    {
        var tools = CreateTools();
        var result = tools.CalculateTax("Tax", 4);
        Assert.Contains("2", result);
    }

    [Fact]
    public void CalculateTax_UnknownCustomer_MonthsUnchanged()
    {
        var tools = CreateTools();
        var result = tools.CalculateTax("Alice", 7);
        Assert.Contains("7", result);
    }

    [Fact]
    public void CalculateTax_ReturnsStringContainingCustomerName()
    {
        var tools = CreateTools();
        var result = tools.CalculateTax("Method", 3);
        Assert.Contains("Method", result);
    }

    [Fact]
    public void CalculateTax_ReturnsStringContainingMonthCount()
    {
        var tools = CreateTools();
        var result = tools.CalculateTax("Tax", 6);
        Assert.Contains("6", result);
    }

    [Fact]
    public void CalculateTax_LogsInformation()
    {
        var logger = Substitute.For<ILogger<McpTools>>();
        var tools = new McpTools(logger);

        tools.CalculateTax("Method", 2);

        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void CalculateTax_ResultIsNonEmptyString()
    {
        var tools = CreateTools();
        var result = tools.CalculateTax("Method", 1);
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
        var tools = CreateTools();
        var result = tools.CalculateTax(customer, months);
        Assert.Contains(expectedValue, result);
    }

    [Fact]
    public void CalculateTax_MethodCustomer_ZeroMonths_ReturnsZero()
    {
        var tools = CreateTools();
        var result = tools.CalculateTax("Method", 0);
        Assert.Contains("0", result);
    }

    [Fact]
    public void CalculateTax_TaxCustomer_OddMonths_ReturnsHalf()
    {
        // 3 / 2.0 = 1.5, but decimal separator is culture-dependent
        var tools = CreateTools();
        var result = tools.CalculateTax("Tax", 3);
        var expected = ((double)3 / 2).ToString();
        Assert.Contains(expected, result);
    }
}
