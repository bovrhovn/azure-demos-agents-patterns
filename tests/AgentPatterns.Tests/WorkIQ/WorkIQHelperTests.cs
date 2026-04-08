using AP.WorkIQ;

namespace AgentPatterns.Tests.WorkIQ;

public class WorkIQHelperTests
{
    [Fact]
    public void ServerName_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(WorkIQHelper.ServerName));
    }

    [Fact]
    public void ServerName_HasExpectedValue()
    {
        Assert.Equal("WorkIQ", WorkIQHelper.ServerName);
    }

    [Fact]
    public void AskWorkIQToolName_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(WorkIQHelper.AskWorkIQToolName));
    }

    [Fact]
    public void AskWorkIQToolName_HasExpectedValue()
    {
        Assert.Equal("ask_work_iq", WorkIQHelper.AskWorkIQToolName);
    }

    [Fact]
    public void DefaultQuestion_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(WorkIQHelper.DefaultQuestion));
    }

    [Fact]
    public void DefaultQuestion_MentionsEmails()
    {
        Assert.Contains("email", WorkIQHelper.DefaultQuestion, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void McpCommand_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(WorkIQHelper.McpCommand));
    }

    [Fact]
    public void McpCommand_HasExpectedValue()
    {
        Assert.Equal("npx", WorkIQHelper.McpCommand);
    }

    [Fact]
    public void McpArguments_IsNotEmpty()
    {
        Assert.NotEmpty(WorkIQHelper.McpArguments);
    }

    [Fact]
    public void McpArguments_ContainsWorkIQPackage()
    {
        Assert.Contains(WorkIQHelper.McpArguments,
            a => a.Contains("workiq", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void McpArguments_AllNonEmpty()
    {
        Assert.All(WorkIQHelper.McpArguments, a => Assert.False(string.IsNullOrWhiteSpace(a)));
    }

    [Fact]
    public void ServerName_And_AskWorkIQToolName_AreDifferent()
    {
        Assert.NotEqual(WorkIQHelper.ServerName, WorkIQHelper.AskWorkIQToolName);
    }
}
