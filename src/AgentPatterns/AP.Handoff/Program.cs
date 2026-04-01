using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Spectre.Console;

AnsiConsole.MarkupLine("[blue]Handoff workflow with Agents programmatically[/]");

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
// 2) Create specialized agents
ChatClientAgent historyTutor = new(client,
    "You provide assistance with historical queries. Explain important events and context clearly. Only respond about history.",
    "history_tutor",
    "Specialist agent for historical questions");

ChatClientAgent mathTutor = new(client,
    "You provide help with math problems. Explain your reasoning at each step and include examples. Only respond about math.",
    "math_tutor",
    "Specialist agent for math questions");

ChatClientAgent triageAgent = new(client,
    "You determine which agent to use based on the user's homework question. ALWAYS handoff to another agent.",
    "triage_agent",
    "Routes messages to the appropriate specialist agent");

// 3) Build handoff workflow with routing rules
var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triageAgent)
    .WithHandoffs(triageAgent, [mathTutor, historyTutor]) // Triage can route to either specialist
    .WithHandoffs([mathTutor, historyTutor], triageAgent) // Both specialists can return to triage
    .Build();

// 4) Process multi-turn conversations
List<ChatMessage> messages = new();

while (true)
{
    var userInput = AnsiConsole.Ask<string>("Your question (or type 'exit' to quit):");
    if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
    messages.Add(new(ChatRole.User, userInput));

    // Execute workflow and process events
    await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, messages);
    await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

    string? lastExecutorId = null;
    List<ChatMessage> newMessages = new();
    await foreach (WorkflowEvent evt in run.WatchStreamAsync())
    {
        if (evt is AgentResponseUpdateEvent e)
        {
            if (e.ExecutorId != lastExecutorId)
            {
                lastExecutorId = e.ExecutorId;
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine(e.ExecutorId);
            }

            AnsiConsole.Write(e.Update.Text);
        }
        else if (evt is WorkflowOutputEvent outputEvt)
        {
            newMessages = outputEvt.As<List<ChatMessage>>()!;
            break;
        }
    }

    // Add new messages to conversation history
    messages.AddRange(newMessages.Skip(messages.Count));
}
AnsiConsole.MarkupLine("[red]Concurrent workflow with Agents programmatically has ended.[/]");