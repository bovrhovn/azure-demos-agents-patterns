using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Spectre.Console;

AnsiConsole.MarkupLine("[green]Using Foundry SDK![/]");

#region Environment variables

var endpoint = Environment.GetEnvironmentVariable("Endpoint");
ArgumentException.ThrowIfNullOrEmpty(endpoint, "Endpoint environment variable is not set.");
var agentName = Environment.GetEnvironmentVariable("AgentName");
ArgumentException.ThrowIfNullOrEmpty(agentName, "AgentName environment variable is not set.");

#endregion

AnsiConsole.MarkupLine($"[green]Endpoint:[/] {endpoint}");
AnsiConsole.MarkupLine($"[green]Agent Name:[/] {agentName}");

var credentials = new DefaultAzureCredential();
AIProjectClient projectClient = new(endpoint: new Uri(endpoint), 
    tokenProvider: credentials);
var agentReference = new AgentReference(name: agentName);
var responseClient = projectClient
    .ProjectOpenAIClient.GetProjectResponsesClientForAgent(agentReference);
// Use the agent to generate a response
var response = responseClient.CreateResponse(
    "give me MSFT stock info"
);
AnsiConsole.WriteLine(response.Value.GetOutputText());