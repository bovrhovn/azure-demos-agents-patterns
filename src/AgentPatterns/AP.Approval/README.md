# AP.Approval — Human Approval Workflow

## What This Pattern Does

The **Approval** pattern adds a human-in-the-loop gate around sensitive tool calls. When an agent attempts to invoke a tool that is marked with `ApprovalRequiredAIFunction`, the workflow pauses and emits a `RequestInfoEvent`. A human operator (or automated policy) reviews the pending tool call and sends an approval or rejection response before execution continues.

This demo models a software deployment pipeline:

| Agent | Role |
|-------|------|
| `QAEngineer` | Runs test suites before deployment |
| `DevOpsEngineer` | Checks staging, creates rollback plan, deploys to production |

The `DeployToProduction` tool requires explicit human approval before it executes.

## When to Use It

- Critical or irreversible operations (deployments, data deletion, payments).
- Compliance workflows that require an audit trail of human approvals.
- Any action where the cost of a mistake is high.

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

cd src/AgentPatterns/AP.Approval
dotnet run
```

The demo automatically approves the tool call to demonstrate the approval flow. In production you would replace this with a real user prompt or policy evaluation.

## Code Walkthrough

### Marking a Tool for Approval

Wrap any `AIFunction` with `ApprovalRequiredAIFunction`:

```csharp
new ApprovalRequiredAIFunction(AIFunctionFactory.Create(DeployToProduction))
```

### Handling the Approval Request

When the workflow pauses, a `RequestInfoEvent` is emitted. Extract the pending tool call and respond:

```csharp
case RequestInfoEvent e when e.Request.TryGetDataAs(out ToolApprovalRequestContent? content):
    // Inspect content.ToolCall, then approve or reject
    await run.SendResponseAsync(
        e.Request.CreateResponse(content.CreateResponse(approved: true)));
```

### Custom Speaker Selection — `DeploymentGroupChatManager`

`DeploymentGroupChatManager` overrides `SelectNextAgentAsync` to ensure the QA Engineer always speaks first, followed by the DevOps Engineer:

```csharp
if (this.IterationCount == 0)
    return new ValueTask<AIAgent>(agents.First(a => a.Name == "QAEngineer"));

return new ValueTask<AIAgent>(agents.First(a => a.Name == "DevOpsEngineer"));
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `ApprovalRequiredAIFunction` | Wraps a tool so it requires human approval before execution |
| `DeploymentGroupChatManager` | Custom speaker-selection logic for the group chat |
| `GroupChatManager` | Base class for custom manager implementations |
| `RequestInfoEvent` | Workflow event that carries the pending approval request |
