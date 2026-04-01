using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AP.GroupChat;

public class ApprovalBasedManager(IReadOnlyList<AIAgent> agents, string approverName)
    : RoundRobinGroupChatManager(agents)
{
    // Override to add custom termination logic
    protected override ValueTask<bool> ShouldTerminateAsync(
        IReadOnlyList<ChatMessage> history,
        CancellationToken cancellationToken = default)
    {
        var last = history.LastOrDefault();
        bool shouldTerminate = last?.AuthorName == approverName &&
                               last.Text?.Contains("approve", StringComparison.OrdinalIgnoreCase) == true;

        return ValueTask.FromResult(shouldTerminate);
    }
}
