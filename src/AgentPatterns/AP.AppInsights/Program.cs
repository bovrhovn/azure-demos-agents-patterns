using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.AI;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Spectre.Console;

AnsiConsole.MarkupLine("[blue]Using Application Insights with Agents programmatically[/]");

const string SourceName = "AppInsightsWithMAFAgents";
const string ServiceName = "AgentOpenTelemetry";

#region Environment variables

var endpoint = Environment.GetEnvironmentVariable("Endpoint");
ArgumentException.ThrowIfNullOrEmpty(endpoint, "Endpoint environment variable is not set.");
var applicationInsightsConnectionString = Environment.GetEnvironmentVariable("APPLICATION_INSIGHTS_CONNECTION_STRING");
ArgumentException.ThrowIfNullOrEmpty(endpoint, "applicationInsightsConnectionString environment variable is not set.");
var deploymentName = Environment.GetEnvironmentVariable("DeploymentName");
ArgumentException.ThrowIfNullOrEmpty(deploymentName, "DeploymentName environment variable is not set.");
var enableSensitiveData = Environment.GetEnvironmentVariable("EnableSensitiveData") ?? "true";
ArgumentException.ThrowIfNullOrEmpty(enableSensitiveData, "EnableSensitiveData environment variable is not set.");

AnsiConsole.MarkupLine($"[green]Using Endpoint:[/] {endpoint}");
AnsiConsole.MarkupLine($"[green]Using DeploymentName:[/] {deploymentName}");
AnsiConsole.MarkupLine($"[green]Enable Sensitive data:[/] {enableSensitiveData}");

#endregion

#region Configure OpenTelemetry

var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService(ServiceName);

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource(SourceName)
    .AddAzureMonitorTraceExporter(options => options.ConnectionString = applicationInsightsConnectionString)
    .Build();

using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddAzureMonitorMetricExporter(options => options.ConnectionString = applicationInsightsConnectionString)
    .Build();

#endregion

var credentials = new DefaultAzureCredential();
IChatClient client =
    new ChatClientBuilder(
            new AzureOpenAIClient(new Uri(endpoint), credentials)
                .GetChatClient(deploymentName)
                .AsIChatClient())
        .UseOpenTelemetry(
            sourceName: "MyAgentTelemetry",
            configure: cfg =>
                cfg.EnableSensitiveData = bool.Parse(enableSensitiveData))
        .Build();

List<ChatMessage> chatHistory =
[

    new(ChatRole.System, """
                         You are friendly poem writer.
                         """),
    new(ChatRole.User,
        "Write poem about logging and agents.")
];

AnsiConsole.MarkupLine($"[gray]{chatHistory.Last().Role} >>> {chatHistory.Last()}[/]");

var response = await client.GetResponseAsync(chatHistory);
AnsiConsole.MarkupLine($"[red]Your poem writer:[/] >>> {response.Text}");