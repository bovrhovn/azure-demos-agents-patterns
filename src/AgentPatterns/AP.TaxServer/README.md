# AP.TaxServer — MCP Tax Server

## What This Pattern Does

The **TaxServer** project is an [ASP.NET Core](https://learn.microsoft.com/aspnet/core/) web application that exposes a tax-calculation tool over the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/). MCP is an open standard that lets AI models discover and invoke server-side tools through a well-defined transport layer.

The server exposes a single tool:

| Tool | Description |
|------|-------------|
| `get_tax_for_customer` | Calculates the tax owed by a named customer for a given number of months |

AI clients (such as [AP.TaxClient](../AP.TaxClient/README.md)) connect to this server, list available tools, and invoke them automatically when responding to user queries.

## When to Use It

- Exposing business logic as reusable AI tools without embedding the logic inside every client.
- Centralising computation behind an HTTP endpoint that multiple AI clients can call.
- Building MCP-compliant backends that work with any MCP-aware AI framework.

## Prerequisites

| Requirement | Details |
|-------------|---------|
| .NET 10 SDK | <https://dot.net> |

> **Note:** This server does not require an Azure subscription or an Azure OpenAI resource — it is pure business logic exposed over HTTP.

## Running the Server

```bash
cd src/AgentPatterns/AP.TaxServer
dotnet run
```

The server starts on the default Kestrel port. The MCP endpoint is available at `/mcp` and the health check at `/health`.

## Code Walkthrough

### `Program.cs`

The server is configured with a minimal ASP.NET Core host. MCP is registered via the `AddMcpServer()` extension and uses HTTP transport in stateless mode:

```csharp
builder.Services
    .AddMcpServer()
    .WithHttpTransport(o => o.Stateless = true)
    .WithToolsFromAssembly();

app.MapHealthChecks("/health");
app.MapMcp("/mcp");
```

`WithToolsFromAssembly()` scans the assembly for classes annotated with `[McpServerToolType]` and registers their methods as callable tools.

### `Tools/McpTools.cs`

`McpTools` is the tool host class. It is injected via the DI container (constructor receives `ILogger<McpTools>`):

```csharp
[McpServerToolType]
public class McpTools(ILogger<McpTools> logger)
{
    [McpServerTool(Name = "get_tax_for_customer")]
    public string CalculateTax(string customer, int forMonths)
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
}
```

#### Tax calculation rules

| Customer name | Calculation |
|---------------|-------------|
| `"Method"` | `months × 2` |
| `"Tax"` | `months ÷ 2` |
| Any other | `months × 1` (unchanged) |

## Key Classes

| Class / Member | Purpose |
|----------------|---------|
| `McpTools` | Tool host; annotated with `[McpServerToolType]` |
| `McpTools.CalculateTax` | Business logic exposed as the `get_tax_for_customer` MCP tool |
| `AddMcpServer()` | ASP.NET Core service registration for the MCP host |
| `WithToolsFromAssembly()` | Auto-discovers `[McpServerToolType]` classes in the assembly |
| `MapMcp("/mcp")` | Maps the MCP HTTP transport endpoint |

## Learn More

| Resource | Link |
|----------|------|
| Model Context Protocol specification | <https://modelcontextprotocol.io/> |
| ModelContextProtocol .NET SDK | <https://github.com/modelcontextprotocol/csharp-sdk> |
| ASP.NET Core minimal APIs | <https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis> |
| Microsoft.Extensions.AI function invocation | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
