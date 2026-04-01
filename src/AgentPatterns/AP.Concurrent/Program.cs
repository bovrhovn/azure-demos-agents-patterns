using AP.Concurrent;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Spectre.Console;

AnsiConsole.MarkupLine("[blue]Concurrent workflow with Agents programmatically[/]");

#region Environment variables

var endpoint = Environment.GetEnvironmentVariable("Endpoint");
ArgumentException.ThrowIfNullOrEmpty(endpoint, "Endpoint environment variable is not set.");
var deploymentName = Environment.GetEnvironmentVariable("DeploymentName");
ArgumentException.ThrowIfNullOrEmpty(deploymentName, "DeploymentName environment variable is not set.");

AnsiConsole.MarkupLine($"[green]Using Endpoint:[/] {endpoint}");
AnsiConsole.MarkupLine($"[green]Using DeploymentName:[/] {deploymentName}");

#endregion

var credentials = new DefaultAzureCredential();
IChatClient client =
    new ChatClientBuilder(
            new AzureOpenAIClient(new Uri(endpoint), credentials)
                .GetChatClient(deploymentName)
                .AsIChatClient())
        .Build();

var agentHelper = new AgentHelper(client);
var translationAgents = agentHelper.GetTranslationAgents();
AnsiConsole.WriteLine($"{translationAgents.Count()} translation agents created");
var workflow = AgentWorkflowBuilder.BuildConcurrent(translationAgents);
var messages = new List<ChatMessage> { new(ChatRole.User, "Hello, team! How are you?") };

await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, messages);
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

string? lastExecutorId = null;
List<ChatMessage> result = [];
await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    if (evt is AgentResponseUpdateEvent e)
    {
        if (e.ExecutorId != lastExecutorId)
        {
            lastExecutorId = e.ExecutorId;
            AnsiConsole.WriteLine();
            AnsiConsole.Write($"{e.ExecutorId}: ");
        }

        AnsiConsole.Write(e.Update.Text);
    }
    else if (evt is WorkflowOutputEvent outputEvt)
    {
        result = outputEvt.As<List<ChatMessage>>()!;
        break;
    }
}

// Display final result
AnsiConsole.WriteLine();
foreach (var message in result)
{
    AnsiConsole.MarkupLine($"[blue]{message.Role}[/]: [red]{message.Text}[/]");
}
