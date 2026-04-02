# AP.TaxClient — MCP Tax Client

## What This Pattern Does

The **TaxClient** project demonstrates how an AI assistant uses the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) to discover and invoke remote tools at runtime. The client:

1. Connects to a running [AP.TaxServer](../AP.TaxServer/README.md) instance over HTTP.
2. Lists the tools the server exposes.
3. Passes those tools to an `IChatClient` backed by Azure OpenAI.
4. Sends a natural-language user message; the LLM decides when and how to call `get_tax_for_customer`.

## When to Use It

- You want to decouple business logic (the server) from the AI assistant (the client).
- Multiple clients (or multiple LLMs) should share the same set of back-end tools.
- You need a standards-compliant, transport-agnostic way to expose tools to AI models.

## Prerequisites

| Requirement | Details |
|-------------|---------|
| .NET 10 SDK | <https://dot.net> |
| Azure OpenAI resource | An endpoint and a deployed model |
| Running AP.TaxServer | Start the server before running this client |
| `Endpoint` env var | Your Azure OpenAI endpoint URL |
| `McpEndpoint` env var | The MCP server endpoint (e.g. `http://localhost:5000/mcp`) |
| `DeploymentName` env var | Name of the deployed model (e.g. `gpt-4o`) |

## Running the Demo

Start the server first:

```bash
cd src/AgentPatterns/AP.TaxServer
dotnet run &
```

Then run the client:

```bash
export Endpoint="https://<your-resource>.openai.azure.com/"
export McpEndpoint="http://localhost:5000/mcp"
export DeploymentName="gpt-4o"

cd src/AgentPatterns/AP.TaxClient
dotnet run
```

On Windows (PowerShell):

```powershell
# Terminal 1 — start the server
cd src/AgentPatterns/AP.TaxServer
dotnet run

# Terminal 2 — run the client
$env:Endpoint     = "https://<your-resource>.openai.azure.com/"
$env:McpEndpoint  = "http://localhost:5000/mcp"
$env:DeploymentName = "gpt-4o"

cd src/AgentPatterns/AP.TaxClient
dotnet run
```

## Code Walkthrough

### Connecting to the MCP Server

An `HttpClientTransport` targets the server's `/mcp` endpoint:

```csharp
var transport = new HttpClientTransport(new HttpClientTransportOptions
{
    Name     = "My Tax Server MCP",
    Endpoint = new Uri(mcpEndpoint)
});
var mcpClient = await McpClient.CreateAsync(transport);
var tools     = await mcpClient.ListToolsAsync();
```

### Passing Tools to the LLM

The discovered `AITool` list is passed directly to `ChatOptions.Tools`. The `UseFunctionInvocation()` middleware handles the round-trip call automatically:

```csharp
IChatClient client = new ChatClientBuilder(...)
    .UseFunctionInvocation()
    .Build();

var chatOptions = new ChatOptions { Tools = [.. tools] };
var response = await client.GetResponseAsync(chatHistory, chatOptions);
```

When the LLM determines it should call `get_tax_for_customer`, the middleware invokes the MCP server and injects the result back into the conversation.

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `HttpClientTransport` | HTTP transport for the MCP client |
| `McpClient.CreateAsync` | Establishes a connection to the MCP server |
| `McpClient.ListToolsAsync` | Discovers all tools the server exposes |
| `UseFunctionInvocation()` | Middleware that automatically calls tools and relays results |
| `ChatOptions.Tools` | Tells the LLM which tools are available |

## Learn More

| Resource | Link |
|----------|------|
| Model Context Protocol specification | <https://modelcontextprotocol.io/> |
| ModelContextProtocol .NET SDK | <https://github.com/modelcontextprotocol/csharp-sdk> |
| Function calling with Microsoft.Extensions.AI | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| Azure OpenAI tool use | <https://learn.microsoft.com/azure/ai-services/openai/how-to/function-calling> |
