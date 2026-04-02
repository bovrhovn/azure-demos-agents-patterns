using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Spectre.Console;

AnsiConsole.MarkupLine("[blue]Model Router with Agents programmatically[/]");

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
List<ChatMessage> chatHistory =
[

    new(ChatRole.System, """
                         You are friendly writer agent, doing poems for business.
                         """),
    new(ChatRole.User,
        "Write me a poem about software development - one verse.")
];

AnsiConsole.MarkupLine($"[gray]{chatHistory.Last().Role} >>> {chatHistory.Last()}[/]");
var response = await client.GetResponseAsync(chatHistory);
AnsiConsole.MarkupLine($"[red]Assistant with model[/] [blue]{response.ModelId}[/] >>> {response.Text}");