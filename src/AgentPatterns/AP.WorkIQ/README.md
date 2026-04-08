# AP.WorkIQ — WorkIQ MCP Client

## What This Pattern Does

The **WorkIQ** pattern demonstrates how to connect to the [Microsoft WorkIQ MCP server](https://github.com/microsoft/work-iq-mcp) using the [Model Context Protocol](https://modelcontextprotocol.io/) .NET SDK. The demo discovers available tools, locates the `ask_work_iq` tool, and invokes it to query Microsoft 365 data.

This demo:
1. Launches the WorkIQ MCP server via `npx` using the stdio transport.
2. Lists all tools the server exposes.
3. Finds the `ask_work_iq` tool by name.
4. Calls the tool with a natural-language question about recent emails.
5. Prints the text response.

## When to Use It

- You want to integrate Microsoft 365 data (emails, meetings, files) into an AI workflow via MCP.
- You need a standards-compliant way to call Microsoft 365 Copilot tools from a .NET application.
- You want to chain WorkIQ results with other AI agents or tools.

## Prerequisites

| Requirement | Details |
|-------------|---------|
| .NET 10 SDK | <https://dot.net> |
| Node.js & npm | Required to run `npx @microsoft/workiq` |
| Microsoft 365 account | The WorkIQ MCP server uses your M365 identity |
| WorkIQ EULA acceptance | Run `npx @microsoft/workiq@latest eula accept` once |

## Running the Demo

```bash
cd src/AgentPatterns/AP.WorkIQ
dotnet run
```

On Windows (PowerShell):

```powershell
cd src/AgentPatterns/AP.WorkIQ
dotnet run
```

> **Authentication:** The WorkIQ server authenticates using your Microsoft 365 credentials. Ensure you have a valid M365 session before running.

## Code Walkthrough

### Launching the MCP Server

A `StdioClientTransport` starts the WorkIQ server as a child process via `npx`:

```csharp
await using var mcpClient = await McpClient.CreateAsync(
    new StdioClientTransport(new StdioClientTransportOptions
    {
        Name    = "WorkIQ",
        Command = "npx",
        Arguments = ["-y", "@microsoft/workiq@latest", "mcp"]
    }));
```

### Discovering and Calling a Tool

List all tools, find `ask_work_iq` by name, and call it with a question:

```csharp
var tools  = await mcpClient.ListToolsAsync();
var askTool = tools.FirstOrDefault(t =>
    string.Equals(t.Name, "ask_work_iq", StringComparison.OrdinalIgnoreCase));

var result = await mcpClient.CallToolAsync(askTool.Name,
    new Dictionary<string, object> { { "question", "What were last 2 emails from my manager?" } });
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `WorkIQHelper` | Constants for server name, tool name, MCP command, and default question |
| `McpClient.CreateAsync` | Establishes a connection to the MCP server |
| `StdioClientTransport` | Launches the server as a local process via stdio |
| `McpClient.ListToolsAsync` | Discovers all tools the server exposes |
| `McpClient.CallToolAsync` | Invokes a tool by name with typed arguments |

## Learn More

| Resource | Link |
|----------|------|
| Model Context Protocol specification | <https://modelcontextprotocol.io/> |
| ModelContextProtocol .NET SDK | <https://github.com/modelcontextprotocol/csharp-sdk> |
| Microsoft WorkIQ MCP server | <https://github.com/microsoft/work-iq-mcp> |
| Microsoft 365 Copilot | <https://learn.microsoft.com/microsoft-365-copilot/> |
