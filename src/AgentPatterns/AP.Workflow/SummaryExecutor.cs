using System.Text;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AP.Workflow;

/// <summary>
/// Executor that presents the final approved content to the user.
/// </summary>
internal sealed class SummaryExecutor(IChatClient chatClient) : Executor<CriticDecision, ChatMessage>("Summary")
{
    private readonly AIAgent _agent = new ChatClientAgent(
        chatClient,
        name: "Summary",
        instructions: """
                      You present the final approved content to the user.
                      Simply output the polished content - no additional commentary needed.
                      """
    );

    public override async ValueTask<ChatMessage> HandleAsync(
        CriticDecision message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Summary ===\n");

        string prompt = $"Present this approved content:\n\n{message.Content}";

        StringBuilder sb = new();
        await foreach (AgentResponseUpdate update in this._agent.RunStreamingAsync(new ChatMessage(ChatRole.User, prompt), cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                sb.Append(update.Text);
            }
        }

        ChatMessage result = new(ChatRole.Assistant, sb.ToString());
        await context.YieldOutputAsync(result, cancellationToken);
        return result;
    }
}