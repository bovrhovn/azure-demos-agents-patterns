using AP.AppInsights;

namespace AgentPatterns.Tests.AppInsights;

public class AppInsightsConfigTests
{
    [Fact]
    public void SourceName_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(AppInsightsConfig.SourceName));
    }

    [Fact]
    public void SourceName_HasExpectedValue()
    {
        Assert.Equal("AppInsightsWithMAFAgents", AppInsightsConfig.SourceName);
    }

    [Fact]
    public void ServiceName_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(AppInsightsConfig.ServiceName));
    }

    [Fact]
    public void ServiceName_HasExpectedValue()
    {
        Assert.Equal("AgentOpenTelemetry", AppInsightsConfig.ServiceName);
    }

    [Fact]
    public void TelemetrySourceName_IsNonEmpty()
    {
        Assert.False(string.IsNullOrWhiteSpace(AppInsightsConfig.TelemetrySourceName));
    }

    [Fact]
    public void TelemetrySourceName_HasExpectedValue()
    {
        Assert.Equal("MyAgentTelemetry", AppInsightsConfig.TelemetrySourceName);
    }

    [Fact]
    public void EndpointEnvVar_HasExpectedValue()
    {
        Assert.Equal("Endpoint", AppInsightsConfig.EndpointEnvVar);
    }

    [Fact]
    public void AppInsightsConnectionStringEnvVar_HasExpectedValue()
    {
        Assert.Equal("APPLICATION_INSIGHTS_CONNECTION_STRING", AppInsightsConfig.AppInsightsConnectionStringEnvVar);
    }

    [Fact]
    public void DeploymentNameEnvVar_HasExpectedValue()
    {
        Assert.Equal("DeploymentName", AppInsightsConfig.DeploymentNameEnvVar);
    }

    [Fact]
    public void EnableSensitiveDataEnvVar_HasExpectedValue()
    {
        Assert.Equal("EnableSensitiveData", AppInsightsConfig.EnableSensitiveDataEnvVar);
    }

    [Fact]
    public void DefaultEnableSensitiveData_IsTrue()
    {
        Assert.Equal("true", AppInsightsConfig.DefaultEnableSensitiveData);
    }

    [Fact]
    public void SourceName_And_ServiceName_AreDifferent()
    {
        Assert.NotEqual(AppInsightsConfig.SourceName, AppInsightsConfig.ServiceName);
    }

    [Fact]
    public void SourceName_And_TelemetrySourceName_AreDifferent()
    {
        Assert.NotEqual(AppInsightsConfig.SourceName, AppInsightsConfig.TelemetrySourceName);
    }
}
