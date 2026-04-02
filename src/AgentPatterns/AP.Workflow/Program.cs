/// This sample demonstrates an iterative refinement workflow between Writer and Critic agents.
///
/// The workflow implements a content creation and review loop that:
/// 1. Writer creates initial content based on the user's request
/// 2. Critic reviews the content and provides feedback using structured output
/// 3. If approved: Summary executor presents the final content
/// 4. If rejected: Writer revises based on feedback (loops back)
/// 5. Continues until approval or max iterations (3) is reached
///
/// This pattern is useful when you need:
/// - Iterative content improvement through feedback loops
/// - Quality gates with reviewer approval
/// - Maximum iteration limits to prevent infinite loops
/// - Conditional workflow routing based on agent decisions
/// - Structured output for reliable decision-making
///
/// Key Learning: Workflows can implement loops with conditional edges, shared state,
/// and structured output for robust agent decision-making.
using AP.Workflow;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Spectre.Console;

AnsiConsole.MarkupLine("[blue]Workflow example with Agents programmatically[/]");

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

WriterExecutor writer = new(client);
CriticExecutor critic = new(client);
SummaryExecutor summary = new(client);

const int MaxIterations = 3;

// Build the workflow with conditional routing based on critic's decision
WorkflowBuilder workflowBuilder = new WorkflowBuilder(writer)
    .AddEdge(writer, critic)
    .AddSwitch(critic, sw => sw
        .AddCase<CriticDecision>(cd => cd?.Approved == true, summary)
        .AddCase<CriticDecision>(cd => cd?.Approved == false, writer))
    .WithOutputFrom(summary);

// Execute the workflow with a sample task
// The workflow loops back to Writer if content is rejected,
// or proceeds to Summary if approved. State tracking ensures we don't loop forever.
AnsiConsole.WriteLine(new string('=', 80));
AnsiConsole.WriteLine("TASK: Write a short blog post about AI ethics (200 words)");
AnsiConsole.WriteLine(new string('=', 80) + "\n");

const string InitialTask = "Write a 200-word blog post about AI ethics. Make it thoughtful and engaging.";

Workflow workflow = workflowBuilder.Build();
await ExecuteWorkflowAsync(workflow, InitialTask);

AnsiConsole.WriteLine("\n✅ Sample Complete: Writer-Critic iteration demonstrates conditional workflow loops\n");
AnsiConsole.WriteLine("Key Concepts Demonstrated:");
AnsiConsole.WriteLine("  ✓ Iterative refinement loop with conditional routing");
AnsiConsole.WriteLine("  ✓ Shared workflow state for iteration tracking");
AnsiConsole.WriteLine($"  ✓ Max iteration cap ({MaxIterations}) for safety");
AnsiConsole.WriteLine("  ✓ Multiple message handlers in a single executor");
AnsiConsole.WriteLine("  ✓ Streaming support with structured output\n");
static async Task ExecuteWorkflowAsync(Workflow workflow, string input)
{
    // Execute in streaming mode to see real-time progress
    await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, input);

    // Watch the workflow events
    await foreach (WorkflowEvent evt in run.WatchStreamAsync())
    {
        switch (evt)
        {
            case AgentResponseUpdateEvent agentUpdate:
                // Stream agent output in real-time
                if (!string.IsNullOrEmpty(agentUpdate.Update.Text)) 
                    AnsiConsole.Write(agentUpdate.Update.Text);
                break;

            case WorkflowOutputEvent output:
                AnsiConsole.WriteLine("\n\n" + new string('=', 80));
                AnsiConsole.WriteLine("✅ FINAL APPROVED CONTENT");
                AnsiConsole.WriteLine(new string('=', 80));
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine((string)output.Data);
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine(new string('=', 80));
                break;
        }
    }
}