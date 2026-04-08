using AP.TaxClient;
using Microsoft.Extensions.AI;

namespace AgentPatterns.Tests.TaxClient;

public class TaxClientHelperTests
{
    [Fact]
    public void BuildChatHistory_ReturnsTwoMessages()
    {
        var history = TaxClientHelper.BuildChatHistory();
        Assert.Equal(2, history.Count);
    }

    [Fact]
    public void BuildChatHistory_FirstMessageIsSystem()
    {
        var history = TaxClientHelper.BuildChatHistory();
        Assert.Equal(ChatRole.System, history[0].Role);
    }

    [Fact]
    public void BuildChatHistory_SecondMessageIsUser()
    {
        var history = TaxClientHelper.BuildChatHistory();
        Assert.Equal(ChatRole.User, history[1].Role);
    }

    [Fact]
    public void BuildChatHistory_SystemMessageMatchesInstructions()
    {
        var history = TaxClientHelper.BuildChatHistory();
        Assert.Equal(TaxClientHelper.SystemInstructions, history[0].Text);
    }

    [Fact]
    public void BuildChatHistory_UsesDefaultUserMessageWhenNoneProvided()
    {
        var history = TaxClientHelper.BuildChatHistory();
        Assert.Equal(TaxClientHelper.DefaultUserMessage, history[1].Text);
    }

    [Fact]
    public void BuildChatHistory_UsesCustomUserMessageWhenProvided()
    {
        const string customMessage = "I am customer Tax. What are my taxes for 6 months?";
        var history = TaxClientHelper.BuildChatHistory(customMessage);
        Assert.Equal(customMessage, history[1].Text);
    }

    [Fact]
    public void BuildChatHistory_NullUserMessage_FallsBackToDefault()
    {
        var history = TaxClientHelper.BuildChatHistory(null);
        Assert.Equal(TaxClientHelper.DefaultUserMessage, history[1].Text);
    }

    [Fact]
    public void SystemInstructions_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(TaxClientHelper.SystemInstructions));
    }

    [Fact]
    public void SystemInstructions_MentionsTax()
    {
        Assert.Contains("tax", TaxClientHelper.SystemInstructions, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DefaultUserMessage_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(TaxClientHelper.DefaultUserMessage));
    }

    [Fact]
    public void DefaultUserMessage_MentionsCustomer()
    {
        Assert.Contains("customer", TaxClientHelper.DefaultUserMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildChatHistory_EachCallReturnsNewList()
    {
        var h1 = TaxClientHelper.BuildChatHistory();
        var h2 = TaxClientHelper.BuildChatHistory();
        Assert.NotSame(h1, h2);
    }
}
