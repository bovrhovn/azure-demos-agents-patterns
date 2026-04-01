using AP.GroupChat;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using NSubstitute;

namespace AgentPatterns.Tests.GroupChat;

/// <summary>
/// Test subclass that exposes the protected ShouldTerminateAsync method for unit testing.
/// </summary>
internal sealed class TestableApprovalBasedManager(
    IReadOnlyList<AIAgent> agents,
    string approverName) : ApprovalBasedManager(agents, approverName)
{
    public ValueTask<bool> TestShouldTerminateAsync(
        IReadOnlyList<ChatMessage> history,
        CancellationToken cancellationToken = default)
        => base.ShouldTerminateAsync(history, cancellationToken);
}

public class ApprovalBasedManagerTests
{
    private static readonly IChatClient MockClient = Substitute.For<IChatClient>();

    private static TestableApprovalBasedManager CreateManager(string approverName = "Reviewer")
    {
        var agents = new List<AIAgent>
        {
            new ChatClientAgent(MockClient, "Writer instructions", "CopyWriter"),
            new ChatClientAgent(MockClient, "Reviewer instructions", "Reviewer"),
        };
        return new TestableApprovalBasedManager(agents, approverName);
    }

    [Fact]
    public async Task ShouldTerminate_WhenApproverApproves_ReturnsTrue()
    {
        var manager = CreateManager("Reviewer");
        var history = new List<ChatMessage>
        {
            new(ChatRole.Assistant, "I approve this slogan.") { AuthorName = "Reviewer" }
        };

        bool result = await manager.TestShouldTerminateAsync(history);

        Assert.True(result);
    }

    [Fact]
    public async Task ShouldTerminate_WhenApproverDoesNotApprove_ReturnsFalse()
    {
        var manager = CreateManager("Reviewer");
        var history = new List<ChatMessage>
        {
            new(ChatRole.Assistant, "This needs more work.") { AuthorName = "Reviewer" }
        };

        bool result = await manager.TestShouldTerminateAsync(history);

        Assert.False(result);
    }

    [Fact]
    public async Task ShouldTerminate_WhenDifferentAuthorApproves_ReturnsFalse()
    {
        var manager = CreateManager("Reviewer");
        var history = new List<ChatMessage>
        {
            new(ChatRole.Assistant, "I approve this slogan.") { AuthorName = "CopyWriter" }
        };

        bool result = await manager.TestShouldTerminateAsync(history);

        Assert.False(result);
    }

    [Fact]
    public async Task ShouldTerminate_WithEmptyHistory_ReturnsFalse()
    {
        var manager = CreateManager("Reviewer");

        bool result = await manager.TestShouldTerminateAsync([]);

        Assert.False(result);
    }

    [Fact]
    public async Task ShouldTerminate_CaseInsensitive_Approve()
    {
        var manager = CreateManager("Reviewer");
        var history = new List<ChatMessage>
        {
            new(ChatRole.Assistant, "APPROVED — great work!") { AuthorName = "Reviewer" }
        };

        bool result = await manager.TestShouldTerminateAsync(history);

        Assert.True(result);
    }

    [Fact]
    public async Task ShouldTerminate_CaseInsensitive_LowerCaseApprove()
    {
        var manager = CreateManager("Reviewer");
        var history = new List<ChatMessage>
        {
            new(ChatRole.Assistant, "i approve the final draft") { AuthorName = "Reviewer" }
        };

        bool result = await manager.TestShouldTerminateAsync(history);

        Assert.True(result);
    }

    [Fact]
    public async Task ShouldTerminate_OnlyLastMessageConsidered_WhenLastIsNotApproval()
    {
        var manager = CreateManager("Reviewer");
        var history = new List<ChatMessage>
        {
            new(ChatRole.Assistant, "I approve the draft.") { AuthorName = "Reviewer" },
            new(ChatRole.Assistant, "Actually, revise it.") { AuthorName = "Reviewer" },
        };

        bool result = await manager.TestShouldTerminateAsync(history);

        Assert.False(result);
    }
}
