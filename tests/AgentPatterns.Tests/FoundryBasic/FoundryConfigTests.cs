using AP.FoundryBasic;

namespace AgentPatterns.Tests.FoundryBasic;

public class FoundryConfigTests
{
    [Fact]
    public void EndpointEnvVar_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(FoundryConfig.EndpointEnvVar));
    }

    [Fact]
    public void EndpointEnvVar_HasExpectedValue()
    {
        Assert.Equal("Endpoint", FoundryConfig.EndpointEnvVar);
    }

    [Fact]
    public void AgentNameEnvVar_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(FoundryConfig.AgentNameEnvVar));
    }

    [Fact]
    public void AgentNameEnvVar_HasExpectedValue()
    {
        Assert.Equal("AgentName", FoundryConfig.AgentNameEnvVar);
    }

    [Fact]
    public void DefaultPrompt_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(FoundryConfig.DefaultPrompt));
    }

    [Fact]
    public void DefaultPrompt_MentionsStock()
    {
        Assert.Contains("stock", FoundryConfig.DefaultPrompt, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EndpointEnvVar_And_AgentNameEnvVar_AreDifferent()
    {
        Assert.NotEqual(FoundryConfig.EndpointEnvVar, FoundryConfig.AgentNameEnvVar);
    }
}
