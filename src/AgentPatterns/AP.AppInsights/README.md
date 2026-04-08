# AP.AppInsights — Application Insights with OpenTelemetry

## What This Pattern Does

The **AppInsights** pattern demonstrates how to wire [Azure Application Insights](https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview) into an AI agent built with `Microsoft.Extensions.AI`. It uses the OpenTelemetry .NET SDK with the Azure Monitor exporter to send traces and metrics directly to an Application Insights resource.

This demo:
1. Builds a tracer provider and meter provider backed by Azure Monitor.
2. Wraps an `IChatClient` with the `UseOpenTelemetry()` middleware so every agent call emits spans.
3. Sends a creative writing prompt and prints the response.

## When to Use It

- You need end-to-end observability (traces, metrics, logs) for your AI workloads in Azure Monitor.
- You want to correlate agent responses with other application telemetry in Application Insights.
- You need to audit which prompts and completions occurred and how long they took.
- A/B testing or canary deployments where you compare response quality across runs.

## Prerequisites

| Requirement | Details |
|-------------|---------|
| .NET 10 SDK | <https://dot.net> |
| Azure OpenAI resource | An endpoint and a deployed model |
| Azure Application Insights resource | Create one at <https://portal.azure.com> |
| `Endpoint` env var | Your Azure OpenAI endpoint URL |
| `DeploymentName` env var | Name of the deployed model (e.g. `gpt-4o`) |
| `APPLICATION_INSIGHTS_CONNECTION_STRING` env var | Connection string from your Application Insights resource |
| `EnableSensitiveData` env var | (Optional) `true` or `false` — defaults to `true` |

## Running the Demo

```bash
export Endpoint="https://<your-resource>.openai.azure.com/"
export DeploymentName="gpt-4o"
export APPLICATION_INSIGHTS_CONNECTION_STRING="InstrumentationKey=..."

cd src/AgentPatterns/AP.AppInsights
dotnet run
```

On Windows (PowerShell):

```powershell
$env:Endpoint = "https://<your-resource>.openai.azure.com/"
$env:DeploymentName = "gpt-4o"
$env:APPLICATION_INSIGHTS_CONNECTION_STRING = "InstrumentationKey=..."

dotnet run
```

## Code Walkthrough

### Configuring OpenTelemetry

A `TracerProvider` and `MeterProvider` are built via the SDK builder APIs, both targeting the same Azure Monitor connection string:

```csharp
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("AgentOpenTelemetry"))
    .AddSource("AppInsightsWithMAFAgents")
    .AddAzureMonitorTraceExporter(options => options.ConnectionString = connectionString)
    .Build();
```

### Instrumenting the Chat Client

The `UseOpenTelemetry()` middleware attaches to the `IChatClient` pipeline. Setting `EnableSensitiveData = true` records prompt and completion content in the trace:

```csharp
IChatClient client = new ChatClientBuilder(innerClient)
    .UseOpenTelemetry(
        sourceName: "MyAgentTelemetry",
        configure: cfg => cfg.EnableSensitiveData = true)
    .Build();
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `AppInsightsConfig` | Constants for source name, service name, and environment variable names |
| `Sdk.CreateTracerProviderBuilder` | Configures and builds the OpenTelemetry tracer pipeline |
| `AddAzureMonitorTraceExporter` | Routes traces to Azure Application Insights |
| `ChatClientBuilder.UseOpenTelemetry` | Middleware that emits spans for every AI call |

## Learn More

| Resource | Link |
|----------|------|
| Azure Monitor OpenTelemetry exporter | <https://learn.microsoft.com/azure/azure-monitor/app/opentelemetry-enable> |
| OpenTelemetry .NET SDK | <https://opentelemetry.io/docs/languages/net/> |
| Microsoft.Extensions.AI telemetry | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| Application Insights overview | <https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview> |
