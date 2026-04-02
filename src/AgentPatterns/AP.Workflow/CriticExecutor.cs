using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AP.Workflow;

/// <summary>
/// Executor that reviews content and decides whether to approve or request revisions.
/// Uses structured output with streaming for reliable decision-making.
/// </summary>
internal sealed class CriticExecutor(IChatClient chatClient) : Executor<ChatMessage, CriticDecision>("Critic")
{
    private readonly AIAgent _agent = new ChatClientAgent(chatClient, new ChatClientAgentOptions
    {
        Name = "Critic",
        ChatOptions = new()
        {
            Instructions = """
                           You are a constructive critic. Review the content and provide specific feedback.
                           Always try to provide actionable suggestions for improvement and strive to identify improvement points.
                           Only approve if the content is high quality, clear, and meets the original requirements and you see no improvement points.

                           Provide your decision as structured output with:
                           - approved: true if content is good, false if revisions needed
                           - feedback: specific improvements needed (empty if approved)

                           Be concise but specific in your feedback.
                           """,
            ResponseFormat = ChatResponseFormat.ForJsonSchema<CriticDecision>()
        }
    });

    public override async ValueTask<CriticDecision> HandleAsync(
        ChatMessage message,
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        int maxIterations = 3;
        FlowState state = await FlowStateHelpers.ReadFlowStateAsync(context);

        Console.WriteLine($"=== Critic (Iteration {state.Iteration}) ===\n");

        // Use RunStreamingAsync to get streaming updates, then deserialize at the end
        IAsyncEnumerable<AgentResponseUpdate> updates = this._agent.RunStreamingAsync(message, cancellationToken: cancellationToken);

        // Stream the output in real-time (for any rationale/explanation)
        await foreach (AgentResponseUpdate update in updates)
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                Console.Write(update.Text);
            }
        }
        Console.WriteLine("\n");

        // Convert the stream to a response and deserialize the structured output
        AgentResponse response = await updates.ToAgentResponseAsync(cancellationToken);
        CriticDecision decision = JsonSerializer.Deserialize<CriticDecision>(response.Text, JsonSerializerOptions.Web)
                                  ?? throw new JsonException("Failed to deserialize CriticDecision from response text.");

        Console.WriteLine($"Decision: {(decision.Approved ? "✅ APPROVED" : "❌ NEEDS REVISION")}");
        if (!string.IsNullOrEmpty(decision.Feedback))
        {
            Console.WriteLine($"Feedback: {decision.Feedback}");
        }
        Console.WriteLine();

        // Safety: approve if max iterations reached
        if (!decision.Approved && state.Iteration >= maxIterations)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️ Max iterations ({maxIterations}) reached - auto-approving");
            Console.ResetColor();
            decision.Approved = true;
            decision.Feedback = "";
        }

        // Increment iteration ONLY if rejecting (will loop back to Writer)
        if (!decision.Approved)
        {
            state.Iteration++;
        }

        // Store the decision in history
        state.History.Add(new ChatMessage(ChatRole.Assistant,
            $"[Decision: {(decision.Approved ? "Approved" : "Needs Revision")}] {decision.Feedback}"));
        await FlowStateHelpers.SaveFlowStateAsync(context, state);

        // Populate workflow-specific fields
        decision.Content = message.Text ?? "";
        decision.Iteration = state.Iteration;

        return decision;
    }
}