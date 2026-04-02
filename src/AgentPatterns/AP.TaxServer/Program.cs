var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(settings => settings.AddServerHeader = false);
builder.Logging.AddConsole(consoleLogOptions => 
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace);
builder.Services.AddHealthChecks();
builder.Services
    .AddMcpServer()
    .WithHttpTransport(o => o.Stateless = true)
    .WithToolsFromAssembly();

var app = builder.Build();
app.MapHealthChecks("/health");
app.MapMcp("/mcp");

app.Run();

public partial class Program { }