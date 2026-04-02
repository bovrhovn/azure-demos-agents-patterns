using System.Text;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AP.Workflow;

/// <summary>
/// Executor that creates or revises content based on user requests or critic feedback.
/// This executor demonstrates multiple message handlers for different input types.
/// </summary>
internal sealed partial class WriterExecutor(IChatClient chatClient) : Executor("Writer")
{
    private readonly AIAgent _agent = new ChatClientAgent(
        chatClient,
        name: "Writer",
        instructions: """
                      You are a skilled writer. Create clear, engaging content.
                      If you receive feedback, carefully revise the content to address all concerns.
                      Maintain the same topic and length requirements.
                      """
    );

    /// <summary>
    /// Handles the initial writing request from the user.
    /// </summary>
    [MessageHandler]
    public async ValueTask<ChatMessage> HandleInitialRequestAsync(
        string message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        return await this.HandleAsyncCoreAsync(new ChatMessage(ChatRole.User, message), context, cancellationToken);
    }

    /// <summary>
    /// Handles revision requests from the critic with feedback.
    /// </summary>
    [MessageHandler]
    public async ValueTask<ChatMessage> HandleRevisionRequestAsync(
        CriticDecision decision,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        string prompt = "Revise the following content based on this feedback:\n\n" +
                        $"Feedback: {decision.Feedback}\n\n" +
                        $"Original Content:\n{decision.Content}";

        return await this.HandleAsyncCoreAsync(new ChatMessage(ChatRole.User, prompt), context, cancellationToken);
    }

    /// <summary>
    /// Core implementation for generating content (initial or revised).
    /// </summary>
    private async Task<ChatMessage> HandleAsyncCoreAsync(
        ChatMessage message,
        IWorkflowContext context,
        CancellationToken cancellationToken)
    {
        FlowState state = await FlowStateHelpers.ReadFlowStateAsync(context);

        Console.WriteLine($"\n=== Writer (Iteration {state.Iteration}) ===\n");

        StringBuilder sb = new();
        await foreach (AgentResponseUpdate update in this._agent.RunStreamingAsync(message, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                sb.Append(update.Text);
                Console.Write(update.Text);
            }
        }
        Console.WriteLine("\n");

        string text = sb.ToString();
        state.History.Add(new ChatMessage(ChatRole.Assistant, text));
        await FlowStateHelpers.SaveFlowStateAsync(context, state);

        return new ChatMessage(ChatRole.User, text);
    }

    protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
    {
        throw new NotImplementedException();
    }
}