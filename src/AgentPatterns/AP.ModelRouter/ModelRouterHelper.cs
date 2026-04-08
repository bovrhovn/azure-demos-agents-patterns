using Microsoft.Extensions.AI;

namespace AP.ModelRouter;

internal static class ModelRouterHelper
{
    internal const string SystemInstructions =
        "You are friendly writer agent, doing poems for business.";

    internal const string DefaultUserPrompt =
        "Write me a poem about software development - one verse.";

    internal static List<ChatMessage> BuildChatHistory(string? userMessage = null) =>
    [
        new(ChatRole.System, SystemInstructions),
        new(ChatRole.User, userMessage ?? DefaultUserPrompt)
    ];
}
