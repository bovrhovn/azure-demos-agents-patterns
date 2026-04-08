namespace AP.WorkIQ;

internal static class WorkIQHelper
{
    internal const string ServerName = "WorkIQ";
    internal const string AskWorkIQToolName = "ask_work_iq";
    internal const string DefaultQuestion = "What were last 2 emails from my manager?";
    internal const string McpCommand = "npx";
    internal static readonly string[] McpArguments = ["-y", "@microsoft/workiq@latest", "mcp"];
}
