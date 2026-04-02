# AP.ModelRouter — Model Router

## What This Pattern Does

The **Model Router** pattern shows how to inspect the `ModelId` property of an AI response to determine which model actually served the request. In more advanced setups you can extend this to route different tasks to different models based on cost, capability, or latency requirements.

This demo:
1. Sends a simple creative writing request to an Azure OpenAI deployment.
2. Prints both the response text and the `ModelId` the service reports.

## When to Use It

- You deploy multiple models (e.g. `gpt-4o` for complex tasks, `gpt-4o-mini` for cost-sensitive ones) and want to route requests programmatically.
- You need to log or audit which model served each request.
- A/B testing different models on the same prompt.

## Prerequisites

| Requirement | Details |
|-------------|---------|
| .NET 10 SDK | <https://dot.net> |
| Azure OpenAI resource | An endpoint and a deployed model |
| `Endpoint` env var | Your Azure OpenAI endpoint URL |
| `DeploymentName` env var | Name of the deployed model (e.g. `gpt-4o`) |

## Running the Demo

```bash
export Endpoint="https://<your-resource>.openai.azure.com/"
export DeploymentName="gpt-4o"

cd src/AgentPatterns/AP.ModelRouter
dotnet run
```

On Windows (PowerShell):

```powershell
$env:Endpoint = "https://<your-resource>.openai.azure.com/"
$env:DeploymentName = "gpt-4o"

dotnet run
```

## Code Walkthrough

The client is built via the fluent `ChatClientBuilder`:

```csharp
IChatClient client =
    new ChatClientBuilder(
        new AzureOpenAIClient(new Uri(endpoint), credentials)
            .GetChatClient(deploymentName)
            .AsIChatClient())
    .Build();
```

After calling `GetResponseAsync`, the response exposes the model identifier that handled the call:

```csharp
var response = await client.GetResponseAsync(chatHistory);
Console.WriteLine($"Assistant with model {response.ModelId} >>> {response.Text}");
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `IChatClient.GetResponseAsync` | Sends the chat history and returns a `ChatResponse` |
| `ChatResponse.ModelId` | Identifies the model that produced the response |
| `ChatClientBuilder` | Fluent builder for configuring pipeline middleware (logging, retry, etc.) |

## Extending the Pattern

To implement actual routing, you can build a custom `IChatClient` middleware that selects a backend based on message complexity or token count:

```csharp
// Pseudocode — run a cheap model for short prompts, expensive for long
if (messages.Sum(m => m.Text?.Length ?? 0) < 200)
    return cheapClient.GetResponseAsync(messages, options, ct);

return powerfulClient.GetResponseAsync(messages, options, ct);
```

## Learn More

| Resource | Link |
|----------|------|
| Microsoft.Extensions.AI overview | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| Azure OpenAI Service models | <https://learn.microsoft.com/azure/ai-services/openai/concepts/models> |
| ChatClientBuilder middleware | <https://learn.microsoft.com/dotnet/api/microsoft.extensions.ai.chatclientbuilder> |
