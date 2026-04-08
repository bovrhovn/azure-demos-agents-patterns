using AP.Memory;

namespace AgentPatterns.Tests.Memory;

public class MemoryConfigTests
{
    [Fact]
    public void DefaultInstructions_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(MemoryConfig.DefaultInstructions));
    }

    [Fact]
    public void DefaultInstructions_DescribesHelpfulAssistant()
    {
        Assert.Contains("helpful", MemoryConfig.DefaultInstructions, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DefaultAgentName_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(MemoryConfig.DefaultAgentName));
    }

    [Fact]
    public void DefaultAgentName_IsAssistant()
    {
        Assert.Equal("Assistant", MemoryConfig.DefaultAgentName);
    }

    [Fact]
    public void DefaultPrompt_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(MemoryConfig.DefaultPrompt));
    }

    [Fact]
    public void DefaultPrompt_ContainsPirate()
    {
        Assert.Contains("pirate", MemoryConfig.DefaultPrompt, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NoMessagesFoundMessage_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(MemoryConfig.NoMessagesFoundMessage));
    }

    [Fact]
    public void AllConstants_AreDifferent()
    {
        var values = new[]
        {
            MemoryConfig.DefaultInstructions,
            MemoryConfig.DefaultAgentName,
            MemoryConfig.DefaultPrompt,
            MemoryConfig.NoMessagesFoundMessage
        };
        Assert.Equal(values.Length, values.Distinct().Count());
    }
}
