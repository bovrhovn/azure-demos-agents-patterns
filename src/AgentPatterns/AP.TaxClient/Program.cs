using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using Spectre.Console;

AnsiConsole.MarkupLine("[green]Using Tax client with Tax MCP server![/]");

#region Environment variables

var endpoint = Environment.GetEnvironmentVariable("Endpoint");
ArgumentException.ThrowIfNullOrEmpty(endpoint, "Endpoint environment variable is not set.");
var mcpEndpoint = Environment.GetEnvironmentVariable("McpEndpoint");
ArgumentException.ThrowIfNullOrEmpty(mcpEndpoint, "McpEndpoint environment variable is not set.");
var deploymentName = Environment.GetEnvironmentVariable("DeploymentName");
ArgumentException.ThrowIfNullOrEmpty(deploymentName, "DeploymentName environment variable is not set.");

#endregion

var credentials = new DefaultAzureCredential();
IChatClient client =
    new ChatClientBuilder(
            new AzureOpenAIClient(new Uri(endpoint), credentials)
                .GetChatClient(deploymentName)
                .AsIChatClient())
        .UseFunctionInvocation()
        .Build();

var transport = new ModelContextProtocol.Client.HttpClientTransport(
    new HttpClientTransportOptions
    {
        Name = "My Tax Server MCP",
        Endpoint = new Uri(mcpEndpoint)
    });
var mcpClient = await McpClient.CreateAsync(transport);
var tools = await mcpClient.ListToolsAsync();
AnsiConsole.MarkupLine("[blue]Tools available[/]");
foreach (var mcpClientTool in tools)
{
    AnsiConsole.WriteLine($"{mcpClientTool.Name} - {mcpClientTool.Description}");
}

var chatOptions = new ChatOptions
{
    Tools = [.. tools]
};

List<ChatMessage> chatHistory =
[
    new(ChatRole.System, """
                         You are friendly business tax consultant. 
                         You calculate tax for specific period of months.
                         """),
    new(ChatRole.User,
        "I am your customer Method. What's is my tax information for past 3 months?")
];

AnsiConsole.MarkupLine($"[gray]{chatHistory.Last().Role} >>> {chatHistory.Last()}[/]");

var response = await client.GetResponseAsync(chatHistory, chatOptions);
AnsiConsole.MarkupLine($"[red]Assistant[/] >>> {response.Text}");