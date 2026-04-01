using System.Reflection;
using AP.Approval;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using NSubstitute;

namespace AgentPatterns.Tests.Approval;

public class DeploymentGroupChatManagerTests
{
    private static readonly IChatClient MockClient = Substitute.For<IChatClient>();

    private static DeploymentGroupChatManager CreateManager()
    {
        var agents = new List<AIAgent>
        {
            new ChatClientAgent(MockClient, "QA instructions", "QAEngineer"),
            new ChatClientAgent(MockClient, "DevOps instructions", "DevOpsEngineer"),
        };
        return new DeploymentGroupChatManager(agents);
    }

    private static async Task<AIAgent> InvokeSelectNextAgentAsync(
        DeploymentGroupChatManager manager,
        IReadOnlyList<ChatMessage> history)
    {
        var method = typeof(DeploymentGroupChatManager).GetMethod(
            "SelectNextAgentAsync",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        try
        {
            var task = (ValueTask<AIAgent>)method.Invoke(manager, [history, CancellationToken.None])!;
            return await task;
        }
        catch (TargetInvocationException tie) when (tie.InnerException is not null)
        {
            // Unwrap reflection wrapper so callers see the real exception
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
            throw; // unreachable
        }
    }

    [Fact]
    public async Task SelectNextAgent_EmptyHistory_ThrowsInvalidOperationException()
    {
        var manager = CreateManager();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => InvokeSelectNextAgentAsync(manager, []));
    }

    [Fact]
    public async Task SelectNextAgent_FirstIteration_ReturnsQAEngineer()
    {
        var manager = CreateManager();
        var history = new List<ChatMessage>
        {
            new(ChatRole.User, "Deploy version 2.4.0 to production.")
        };

        AIAgent result = await InvokeSelectNextAgentAsync(manager, history);

        Assert.Equal("QAEngineer", result.Name);
    }

    [Fact]
    public async Task SelectNextAgent_SubsequentIteration_ReturnsDevOpsEngineer()
    {
        var manager = CreateManager();
        var history = new List<ChatMessage>
        {
            new(ChatRole.User, "Deploy version 2.4.0 to production.")
        };

        // Increment IterationCount via reflection to simulate a subsequent turn
        SetIterationCount(manager, 1);

        AIAgent result = await InvokeSelectNextAgentAsync(manager, history);

        Assert.Equal("DevOpsEngineer", result.Name);
    }

    [Fact]
    public async Task SelectNextAgent_MultipleSubsequentIterations_AlwaysReturnsDevOpsEngineer()
    {
        var manager = CreateManager();
        var history = new List<ChatMessage>
        {
            new(ChatRole.User, "Deploy request")
        };

        for (int iteration = 1; iteration <= 3; iteration++)
        {
            SetIterationCount(manager, iteration);
            AIAgent result = await InvokeSelectNextAgentAsync(manager, history);
            Assert.Equal("DevOpsEngineer", result.Name);
        }
    }

    /// <summary>Sets IterationCount on GroupChatManager via reflection (auto-property backing field).</summary>
    private static void SetIterationCount(GroupChatManager manager, int count)
    {
        var prop = typeof(GroupChatManager).GetProperty(
            "IterationCount",
            BindingFlags.Public | BindingFlags.Instance);

        if (prop?.CanWrite == true)
        {
            prop.SetValue(manager, count);
            return;
        }

        // Fall back to the compiler-generated backing field for auto-properties
        var backingField = typeof(GroupChatManager).GetField(
            "<IterationCount>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Instance);

        backingField?.SetValue(manager, count);
    }
}
