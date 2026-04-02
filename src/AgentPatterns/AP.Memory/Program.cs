using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Spectre.Console;

AnsiConsole.MarkupLine("[blue]Memory service with Agents programmatically[/]");

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
var agent = client.AsAIAgent(instructions: "You are a helpful assistant.", name: "Assistant");
AgentSession session = await agent.CreateSessionAsync();
var agentResponse = await agent.RunAsync("Tell me a joke about a pirate.", session);
AnsiConsole.WriteLine(agentResponse.Text);

// When in-memory chat history storage is used, it's possible to access the chat history
// that is stored in the session via the provider attached to the agent.
var provider = agent.GetService<InMemoryChatHistoryProvider>();
List<ChatMessage>? messages = provider?.GetMessages(session);
if (messages == null)
{
    AnsiConsole.MarkupLine("[red]No messages found.[/]");
    return;
}
AnsiConsole.MarkupLine("[gray]History of messages from AI:[/]");
foreach (var chatMessage in messages)
{
    AnsiConsole.MarkupLine($"[yellow]{chatMessage.Role}:[/]  {chatMessage.Text}");  
}