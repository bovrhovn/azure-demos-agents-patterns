using ModelContextProtocol.Client;
using Spectre.Console;

AnsiConsole.MarkupLine("[green]Using client with WorkIQ MCP server![/]");
await using var mcpClient = await McpClient.CreateAsync(
    new StdioClientTransport(new StdioClientTransportOptions
    {
        Name = "WorkIQ",
        Command = "npx",
        Arguments = ["-y", "@microsoft/workiq@latest", "mcp"],
        StandardErrorLines = line => AnsiConsole.WriteLine($"[workiq] {line}")
    }));

var tools = await mcpClient.ListToolsAsync().ConfigureAwait(false);
AnsiConsole.WriteLine($"Connected to WorkIQ. Tools number: {tools.Count}");
foreach (var currentMcpTool in tools)
{
    AnsiConsole.WriteLine($"- {currentMcpTool.Name}: {currentMcpTool.Description}");		
}
var askTool = tools.FirstOrDefault(t => string.Equals(t.Name, "ask_work_iq", 
    StringComparison.OrdinalIgnoreCase));
if (askTool is null)
{
    AnsiConsole.MarkupLine("Tool [red]'ask_work_iq'[/] not found.");
    return;
}
//askTool.JsonSchema
AnsiConsole.WriteLine("Getting last 2 emails from my manager:");
// Build args that match the schema
var arguments = new Dictionary<string, object>
{
    { "question", "What were last 2 emails from my manager?" }
};
// Invoke by tool name
var result = await mcpClient.CallToolAsync(askTool.Name, arguments!);
var text = string.Join(
    Environment.NewLine,
    result.Content
        .OfType<ModelContextProtocol.Protocol.TextContentBlock>()
        .Select(c => c.Text)
);
AnsiConsole.WriteLine(text);
