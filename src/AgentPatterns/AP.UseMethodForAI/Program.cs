using AP.UseMethodForAI;
using Azure.AI.OpenAI;
using Azure.Identity;
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
var chatOptions = new ChatOptions
{
    Tools =
    [
        AIFunctionFactory.Create(DotnetMethodHelper.CalculateTax,
            "get_tax_for_customer",
            "Gets tax for customer for specific period of months")
    ]
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