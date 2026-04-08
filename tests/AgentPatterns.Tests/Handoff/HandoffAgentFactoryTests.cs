using AP.Handoff;
using Microsoft.Extensions.AI;
using NSubstitute;

namespace AgentPatterns.Tests.Handoff;

public class HandoffAgentFactoryTests
{
    private readonly IChatClient _mockClient = Substitute.For<IChatClient>();

    [Fact]
    public void CreateTriageAgent_ReturnsNonNullAgent()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agent = factory.CreateTriageAgent();
        Assert.NotNull(agent);
    }

    [Fact]
    public void CreateTriageAgent_HasCorrectName()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agent = factory.CreateTriageAgent();
        Assert.Equal(HandoffAgentFactory.TriageAgentName, agent.Name);
    }

    [Fact]
    public void CreateTriageAgent_InstructionsContainHandoff()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agent = factory.CreateTriageAgent();
        Assert.Contains("handoff", agent.Instructions, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateHistoryTutor_ReturnsNonNullAgent()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agent = factory.CreateHistoryTutor();
        Assert.NotNull(agent);
    }

    [Fact]
    public void CreateHistoryTutor_HasCorrectName()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agent = factory.CreateHistoryTutor();
        Assert.Equal(HandoffAgentFactory.HistoryTutorName, agent.Name);
    }

    [Fact]
    public void CreateHistoryTutor_InstructionsContainHistory()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agent = factory.CreateHistoryTutor();
        Assert.Contains("histor", agent.Instructions, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateMathTutor_ReturnsNonNullAgent()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agent = factory.CreateMathTutor();
        Assert.NotNull(agent);
    }

    [Fact]
    public void CreateMathTutor_HasCorrectName()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agent = factory.CreateMathTutor();
        Assert.Equal(HandoffAgentFactory.MathTutorName, agent.Name);
    }

    [Fact]
    public void CreateMathTutor_InstructionsContainMath()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agent = factory.CreateMathTutor();
        Assert.Contains("math", agent.Instructions, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateAllAgents_ReturnsThreeAgents()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agents = factory.CreateAllAgents();
        Assert.Equal(3, agents.Count);
    }

    [Fact]
    public void CreateAllAgents_AllNonNull()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agents = factory.CreateAllAgents();
        Assert.All(agents, a => Assert.NotNull(a));
    }

    [Fact]
    public void CreateAllAgents_ContainsTriageAgent()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agents = factory.CreateAllAgents();
        Assert.Contains(agents, a => a.Name == HandoffAgentFactory.TriageAgentName);
    }

    [Fact]
    public void CreateAllAgents_ContainsHistoryTutor()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agents = factory.CreateAllAgents();
        Assert.Contains(agents, a => a.Name == HandoffAgentFactory.HistoryTutorName);
    }

    [Fact]
    public void CreateAllAgents_ContainsMathTutor()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agents = factory.CreateAllAgents();
        Assert.Contains(agents, a => a.Name == HandoffAgentFactory.MathTutorName);
    }

    [Fact]
    public void CreateAllAgents_AgentsHaveDistinctNames()
    {
        var factory = new HandoffAgentFactory(_mockClient);
        var agents = factory.CreateAllAgents();
        var names = agents.Select(a => a.Name).ToList();
        Assert.Equal(names.Count, names.Distinct().Count());
    }

    [Fact]
    public void AgentNames_AreNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(HandoffAgentFactory.TriageAgentName));
        Assert.False(string.IsNullOrWhiteSpace(HandoffAgentFactory.HistoryTutorName));
        Assert.False(string.IsNullOrWhiteSpace(HandoffAgentFactory.MathTutorName));
    }

    [Fact]
    public void Instructions_AreNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(HandoffAgentFactory.TriageInstructions));
        Assert.False(string.IsNullOrWhiteSpace(HandoffAgentFactory.HistoryTutorInstructions));
        Assert.False(string.IsNullOrWhiteSpace(HandoffAgentFactory.MathTutorInstructions));
    }
}
