using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AP.Approval;

internal sealed class DeploymentGroupChatManager(IReadOnlyList<AIAgent> agents) : GroupChatManager
{
    protected override ValueTask<AIAgent> SelectNextAgentAsync(
        IReadOnlyList<ChatMessage> history,
        CancellationToken cancellationToken = default)
    {
        if (history.Count == 0)
        {
            throw new InvalidOperationException("Conversation is empty; cannot select next speaker.");
        }

        // First speaker after initial user message
        if (this.IterationCount == 0)
        {
            AIAgent qaAgent = agents.First(a => a.Name == "QAEngineer");
            return new ValueTask<AIAgent>(qaAgent);
        }

        // Subsequent speakers are DevOps Engineer
        AIAgent devopsAgent = agents.First(a => a.Name == "DevOpsEngineer");
        return new ValueTask<AIAgent>(devopsAgent);
    }
}