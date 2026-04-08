using AP.ModelRouter;
using Microsoft.Extensions.AI;

namespace AgentPatterns.Tests.ModelRouter;

public class ModelRouterHelperTests
{
    [Fact]
    public void BuildChatHistory_ReturnsTwoMessages()
    {
        var history = ModelRouterHelper.BuildChatHistory();
        Assert.Equal(2, history.Count);
    }

    [Fact]
    public void BuildChatHistory_FirstMessageIsSystem()
    {
        var history = ModelRouterHelper.BuildChatHistory();
        Assert.Equal(ChatRole.System, history[0].Role);
    }

    [Fact]
    public void BuildChatHistory_SecondMessageIsUser()
    {
        var history = ModelRouterHelper.BuildChatHistory();
        Assert.Equal(ChatRole.User, history[1].Role);
    }

    [Fact]
    public void BuildChatHistory_SystemMessageContainsInstructions()
    {
        var history = ModelRouterHelper.BuildChatHistory();
        Assert.Equal(ModelRouterHelper.SystemInstructions, history[0].Text);
    }

    [Fact]
    public void BuildChatHistory_UsesDefaultPromptWhenNoMessageProvided()
    {
        var history = ModelRouterHelper.BuildChatHistory();
        Assert.Equal(ModelRouterHelper.DefaultUserPrompt, history[1].Text);
    }

    [Fact]
    public void BuildChatHistory_UsesCustomUserMessageWhenProvided()
    {
        const string customMessage = "Write a haiku about testing.";
        var history = ModelRouterHelper.BuildChatHistory(customMessage);
        Assert.Equal(customMessage, history[1].Text);
    }

    [Fact]
    public void BuildChatHistory_NullUserMessage_FallsBackToDefault()
    {
        var history = ModelRouterHelper.BuildChatHistory(null);
        Assert.Equal(ModelRouterHelper.DefaultUserPrompt, history[1].Text);
    }

    [Fact]
    public void SystemInstructions_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(ModelRouterHelper.SystemInstructions));
    }

    [Fact]
    public void DefaultUserPrompt_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(ModelRouterHelper.DefaultUserPrompt));
    }

    [Fact]
    public void BuildChatHistory_SystemMessageContainsPoemOrWriter()
    {
        var history = ModelRouterHelper.BuildChatHistory();
        var systemText = history[0].Text ?? string.Empty;
        Assert.True(
            systemText.Contains("poem", StringComparison.OrdinalIgnoreCase) ||
            systemText.Contains("writer", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void BuildChatHistory_EachCallReturnsNewList()
    {
        var h1 = ModelRouterHelper.BuildChatHistory();
        var h2 = ModelRouterHelper.BuildChatHistory();
        Assert.NotSame(h1, h2);
    }
}
