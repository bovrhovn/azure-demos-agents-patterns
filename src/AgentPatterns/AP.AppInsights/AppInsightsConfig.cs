namespace AP.AppInsights;

internal static class AppInsightsConfig
{
    internal const string SourceName = "AppInsightsWithMAFAgents";
    internal const string ServiceName = "AgentOpenTelemetry";
    internal const string TelemetrySourceName = "MyAgentTelemetry";
    internal const string EndpointEnvVar = "Endpoint";
    internal const string AppInsightsConnectionStringEnvVar = "APPLICATION_INSIGHTS_CONNECTION_STRING";
    internal const string DeploymentNameEnvVar = "DeploymentName";
    internal const string EnableSensitiveDataEnvVar = "EnableSensitiveData";
    internal const string DefaultEnableSensitiveData = "true";
}
