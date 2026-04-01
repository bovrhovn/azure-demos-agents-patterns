using AP.Concurrent;
using Microsoft.Extensions.AI;
using NSubstitute;

namespace AgentPatterns.Tests.Concurrent;

public class AgentHelperTests
{
    private readonly IChatClient _mockClient = Substitute.For<IChatClient>();

    [Fact]
    public void GetTranslationAgents_ReturnsThreeAgents()
    {
        var helper = new AgentHelper(_mockClient);
        var agents = helper.GetTranslationAgents().ToList();
        Assert.Equal(3, agents.Count);
    }

    [Fact]
    public void GetTranslationAgents_AgentsAreNotNull()
    {
        var helper = new AgentHelper(_mockClient);
        var agents = helper.GetTranslationAgents().ToList();
        Assert.All(agents, Assert.NotNull);
    }

    [Fact]
    public void GetTranslationAgents_ContainsEnglishAgent()
    {
        var helper = new AgentHelper(_mockClient);
        var agents = helper.GetTranslationAgents().ToList();
        Assert.Contains(agents, a => a.Instructions?.Contains("English", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void GetTranslationAgents_ContainsSlovenianAgent()
    {
        var helper = new AgentHelper(_mockClient);
        var agents = helper.GetTranslationAgents().ToList();
        Assert.Contains(agents, a => a.Instructions?.Contains("Slovenian", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void GetTranslationAgents_ContainsSpanishAgent()
    {
        var helper = new AgentHelper(_mockClient);
        var agents = helper.GetTranslationAgents().ToList();
        Assert.Contains(agents, a => a.Instructions?.Contains("Spanish", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void GetTranslationAgent_CreatesAgentWithLanguageInInstructions()
    {
        var helper = new AgentHelper(_mockClient);
        var agent = helper.GetTranslationAgent("French", _mockClient);
        Assert.NotNull(agent);
        Assert.Contains("French", agent.Instructions, StringComparison.Ordinal);
    }

    [Fact]
    public void GetTranslationAgent_InstructionsDescribeTranslationBehavior()
    {
        var helper = new AgentHelper(_mockClient);
        var agent = helper.GetTranslationAgent("German", _mockClient);
        Assert.Contains("translation", agent.Instructions, StringComparison.OrdinalIgnoreCase);
    }
}
