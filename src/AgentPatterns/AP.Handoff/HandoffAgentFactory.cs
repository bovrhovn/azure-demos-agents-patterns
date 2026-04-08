using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AP.Handoff;

internal sealed class HandoffAgentFactory(IChatClient client)
{
    internal const string TriageAgentName = "triage_agent";
    internal const string HistoryTutorName = "history_tutor";
    internal const string MathTutorName = "math_tutor";

    internal const string TriageInstructions =
        "You determine which agent to use based on the user's homework question. ALWAYS handoff to another agent.";

    internal const string HistoryTutorInstructions =
        "You provide assistance with historical queries. Explain important events and context clearly. Only respond about history.";

    internal const string MathTutorInstructions =
        "You provide help with math problems. Explain your reasoning at each step and include examples. Only respond about math.";

    internal ChatClientAgent CreateTriageAgent() =>
        new(client, TriageInstructions, TriageAgentName,
            "Routes messages to the appropriate specialist agent");

    internal ChatClientAgent CreateHistoryTutor() =>
        new(client, HistoryTutorInstructions, HistoryTutorName,
            "Specialist agent for historical questions");

    internal ChatClientAgent CreateMathTutor() =>
        new(client, MathTutorInstructions, MathTutorName,
            "Specialist agent for math questions");

    internal IReadOnlyList<ChatClientAgent> CreateAllAgents() =>
        [CreateTriageAgent(), CreateHistoryTutor(), CreateMathTutor()];
}
