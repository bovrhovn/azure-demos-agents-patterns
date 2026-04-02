using Microsoft.Extensions.AI;

namespace AP.Workflow;

/// <summary>
/// Tracks the current iteration and conversation history across workflow executions.
/// </summary>
internal sealed class FlowState
{
    public int Iteration { get; set; } = 1;
    public List<ChatMessage> History { get; } = [];
}