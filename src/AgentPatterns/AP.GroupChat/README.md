# AP.GroupChat — Group Chat Workflow

## What This Pattern Does

The **Group Chat** pattern enables multi-agent, multi-turn conversations. Agents take turns contributing to a shared conversation history. A `GroupChatManager` controls who speaks next and when the conversation ends.

This demo uses a round-robin manager extended with an **approval-based termination** condition:

| Agent | Role |
|-------|------|
| `CopyWriter` | Generates marketing slogans and copy |
| `Reviewer` | Evaluates copy quality and either approves or requests revisions |

The conversation continues until the `Reviewer` sends a message containing the word **"approve"**, or the maximum iteration count is reached.

## When to Use It

- You need iterative, conversational refinement between agents (writer/reviewer, coder/tester).
- Autonomous quality loops where agents challenge each other until a condition is met.
- Brainstorming or collaborative decision-making patterns.

## Prerequisites

| Requirement | Details |
|-------------|---------|
| .NET 10 SDK | <https://dot.net> |
| Azure OpenAI resource | An endpoint and a deployed model |
| `Endpoint` env var | Your Azure OpenAI endpoint URL |
| `DeploymentName` env var | Name of the deployed model (e.g. `gpt-4o`) |

## Running the Demo

```bash
export Endpoint="https://<your-resource>.openai.azure.com/"
export DeploymentName="gpt-4o"

cd src/AgentPatterns/AP.GroupChat
dotnet run
```

## Code Walkthrough

### `ApprovalBasedManager.cs`

`ApprovalBasedManager` extends `RoundRobinGroupChatManager` and overrides `ShouldTerminateAsync` to stop when the designated approver's last message contains "approve":

```csharp
protected override ValueTask<bool> ShouldTerminateAsync(
    IReadOnlyList<ChatMessage> history, CancellationToken ct = default)
{
    var last = history.LastOrDefault();
    bool shouldTerminate =
        last?.AuthorName == approverName &&
        last.Text?.Contains("approve", StringComparison.OrdinalIgnoreCase) == true;
    return ValueTask.FromResult(shouldTerminate);
}
```

### Building the Workflow

The manager factory receives the participant list at build time:

```csharp
var workflow = AgentWorkflowBuilder
    .CreateGroupChatBuilderWith(agents =>
        new RoundRobinGroupChatManager(agents) { MaximumIterationCount = 5 })
    .AddParticipants(writer, reviewer)
    .Build();
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `ApprovalBasedManager` | Extends round-robin with approval-based termination |
| `RoundRobinGroupChatManager` | Built-in manager that cycles through agents in order |
| `GroupChatManager.ShouldTerminateAsync` | Override to define custom stopping conditions |
| `AgentWorkflowBuilder.CreateGroupChatBuilderWith` | Entry point for group chat workflow construction |
