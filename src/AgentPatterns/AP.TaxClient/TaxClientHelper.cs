using Microsoft.Extensions.AI;

namespace AP.TaxClient;

internal static class TaxClientHelper
{
    internal const string SystemInstructions =
        "You are friendly business tax consultant. \nYou calculate tax for specific period of months.";

    internal const string DefaultUserMessage =
        "I am your customer Method. What's is my tax information for past 3 months?";

    internal static List<ChatMessage> BuildChatHistory(string? userMessage = null) =>
    [
        new(ChatRole.System, SystemInstructions),
        new(ChatRole.User, userMessage ?? DefaultUserMessage)
    ];
}
