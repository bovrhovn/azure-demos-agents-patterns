# AP.UseMethodForAI — Using a .NET Method as an AI Tool

## What This Pattern Does

The **UseMethodForAI** pattern shows how to expose an ordinary .NET method as an AI function tool that a language model can invoke during a chat session. The demo wraps `DotnetMethodHelper.CalculateTax` with `AIFunctionFactory.Create`, making it available to the model as a callable tool named `get_tax_for_customer`.

When the user asks a tax-related question, the LLM recognises the intent and calls the registered method directly in-process, without any additional server infrastructure.

## When to Use It

- You have existing C# business logic you want an AI assistant to use.
- You want tool invocation in-process (no MCP server needed).
- Rapid prototyping: expose a method as a tool with a single line of code.
- Compare with [AP.TaxServer](../AP.TaxServer/README.md) + [AP.TaxClient](../AP.TaxClient/README.md) for an out-of-process equivalent.

## In-Process vs. MCP (Out-of-Process)

| Aspect | UseMethodForAI (in-process) | TaxServer/TaxClient (MCP) |
|--------|-----------------------------|---------------------------|
| Deployment | Single process | Separate server + client |
| Discovery | Registered directly in code | HTTP endpoint, tool list |
| Overhead | None | HTTP round-trip |
| Reusability | Client-specific | Any MCP-compatible client |

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

cd src/AgentPatterns/AP.UseMethodForAI
dotnet run
```

On Windows (PowerShell):

```powershell
$env:Endpoint       = "https://<your-resource>.openai.azure.com/"
$env:DeploymentName = "gpt-4o"

dotnet run
```

## Code Walkthrough

### `DotnetMethodHelper.cs`

The helper contains a straightforward business calculation:

```csharp
public static string CalculateTax(string customer, int forMonths)
{
    var multiplier = 2;
    var result = customer switch
    {
        "Method" => forMonths * multiplier,
        "Tax"    => (double)forMonths / multiplier,
        _        => forMonths
    };
    return $"Tax for customer {customer} for past {forMonths} months is {result}";
}
```

#### Tax calculation rules

| Customer name | Calculation |
|---------------|-------------|
| `"Method"` | `months × 2` |
| `"Tax"` | `months ÷ 2` |
| Any other | `months × 1` (unchanged) |

### Registering the Method as a Tool

`AIFunctionFactory.Create` wraps the method with metadata the LLM uses to decide when to call it:

```csharp
var chatOptions = new ChatOptions
{
    Tools =
    [
        AIFunctionFactory.Create(
            DotnetMethodHelper.CalculateTax,
            "get_tax_for_customer",
            "Gets tax for customer for specific period of months")
    ]
};
```

### Calling the Model

The tool is passed alongside the chat history. The model invokes it automatically when needed:

```csharp
var response = await client.GetResponseAsync(chatHistory, chatOptions);
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `DotnetMethodHelper.CalculateTax` | Business logic exposed as an AI tool |
| `AIFunctionFactory.Create` | Wraps any delegate or `MethodInfo` as an `AIFunction` |
| `ChatOptions.Tools` | Registers the tool list for the LLM to discover |
| `IChatClient.GetResponseAsync` | Sends the chat history (and available tools) to the model |

## Learn More

| Resource | Link |
|----------|------|
| Function calling with Microsoft.Extensions.AI | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| AIFunctionFactory documentation | <https://learn.microsoft.com/dotnet/api/microsoft.extensions.ai.aifunctionfactory> |
| Azure OpenAI tool use / function calling | <https://learn.microsoft.com/azure/ai-services/openai/how-to/function-calling> |
| Model Context Protocol (out-of-process alternative) | <https://modelcontextprotocol.io/> |
