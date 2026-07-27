using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry;
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
var agentRecord = await projectClient.AgentAdministrationClient.GetAgentAsync(agentName);
FoundryAgent agent = projectClient.AsAIAgent(agentRecord);
AgentResponse response = await agent.RunAsync("Give me MSFT stock info.");
AnsiConsole.WriteLine(response.Text);