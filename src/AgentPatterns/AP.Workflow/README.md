# AP.Workflow — Critic–Writer Feedback Loop

## What This Pattern Does

The **Workflow** pattern implements an iterative **Writer → Critic → (revise or publish)** loop using the `Microsoft.Agents.AI.Workflows` graph engine. A **Writer** agent creates content, a **Critic** agent reviews it and returns structured JSON feedback, and the workflow either loops back for revision or forwards the approved content to a **Summary** agent.

Key concepts demonstrated:

- **Typed `Executor<TIn, TOut>`** — each node in the workflow is a strongly-typed executor.
- **Structured output** — the Critic uses `ChatResponseFormat.ForJsonSchema<CriticDecision>()` to produce a machine-readable decision object.
- **Shared workflow state** — `FlowState` is persisted via `IWorkflowContext.QueueStateUpdateAsync` so every executor can read iteration count and chat history.
- **Multiple message handlers** — `WriterExecutor` uses the `[MessageHandler]` attribute to handle both an initial `string` request and a `CriticDecision` revision request.
- **Automatic safety cap** — the Critic auto-approves after three iterations to prevent infinite loops.

## Workflow Graph

```
User prompt (string)
        │
        ▼
  ┌─────────────┐
  │   Writer    │  ← handles string (initial) or CriticDecision (revision)
  └──────┬──────┘
         │ ChatMessage
         ▼
  ┌─────────────┐
  │   Critic    │  ← structured JSON output (CriticDecision)
  └──────┬──────┘
         │
    ┌────┴────────────────┐
    │ approved?           │
    ▼ YES                 ▼ NO (loop back)
 ┌──────────┐      Writer (revision)
 │ Summary  │
 └──────────┘
```

## When to Use It

- Content-generation pipelines that require a review gate before publishing.
- Any iterative-refinement workflow where an AI judge drives branching.
- Demonstrations of structured output (JSON schema) from an LLM.
- Patterns where shared, cross-executor state is needed.

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

cd src/AgentPatterns/AP.Workflow
dotnet run
```

On Windows (PowerShell):

```powershell
$env:Endpoint = "https://<your-resource>.openai.azure.com/"
$env:DeploymentName = "gpt-4o"

dotnet run
```

## Code Walkthrough

### `FlowState.cs`

Shared state carried through the workflow:

```csharp
internal sealed class FlowState
{
    public int Iteration { get; set; } = 1;
    public List<ChatMessage> History { get; } = [];
}
```

### `FlowStateShared.cs`

Constants that identify the state slot in the workflow context:

```csharp
internal static class FlowStateShared
{
    public const string Scope = "FlowStateScope";
    public const string Key   = "singleton";
}
```

### `FlowStateHelpers.cs`

Thin helpers that wrap the `IWorkflowContext` state API:

```csharp
public static async Task<FlowState> ReadFlowStateAsync(IWorkflowContext context)
{
    var state = await context.ReadStateAsync<FlowState>(
        FlowStateShared.Key, scopeName: FlowStateShared.Scope);
    return state ?? new FlowState();
}

public static ValueTask SaveFlowStateAsync(IWorkflowContext context, FlowState state)
    => context.QueueStateUpdateAsync(
        FlowStateShared.Key, state, scopeName: FlowStateShared.Scope);
```

### `CriticDecision.cs`

Structured output schema for the Critic's review:

```csharp
internal sealed class CriticDecision
{
    [JsonPropertyName("approved")]
    public bool Approved { get; set; }

    [JsonPropertyName("feedback")]
    public string Feedback { get; set; } = "";

    [JsonIgnore] public string Content   { get; set; } = "";
    [JsonIgnore] public int    Iteration { get; set; }
}
```

`Content` and `Iteration` are populated by the workflow and excluded from JSON serialization.

### `WriterExecutor.cs`

Uses `[MessageHandler]` to dispatch different input types:

```csharp
[MessageHandler]
public async ValueTask<ChatMessage> HandleInitialRequestAsync(
    string message, IWorkflowContext context, CancellationToken ct) { ... }

[MessageHandler]
public async ValueTask<ChatMessage> HandleRevisionRequestAsync(
    CriticDecision decision, IWorkflowContext context, CancellationToken ct) { ... }
```

### `CriticExecutor.cs`

Returns a `CriticDecision` with structured JSON output from the model:

```csharp
ChatOptions = new()
{
    ResponseFormat = ChatResponseFormat.ForJsonSchema<CriticDecision>()
}
```

Auto-approves when `state.Iteration >= 3` to prevent unbounded loops.

### `SummaryExecutor.cs`

Receives the approved `CriticDecision` and presents the polished content:

```csharp
await context.YieldOutputAsync(result, cancellationToken);
```

## Key Classes

| Class | Purpose |
|-------|---------|
| `FlowState` | Shared state: iteration counter and chat history |
| `FlowStateShared` | Scope/key constants for the state slot |
| `FlowStateHelpers` | Read/write helpers for `IWorkflowContext` state |
| `CriticDecision` | JSON-serializable structured output for the Critic |
| `WriterExecutor` | Generates and revises content; multi-handler executor |
| `CriticExecutor` | Reviews content; produces structured approval decision |
| `SummaryExecutor` | Presents the final approved content to the user |

## Learn More

| Resource | Link |
|----------|------|
| Microsoft.Agents.AI.Workflows | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| Structured output (JSON schema) | <https://learn.microsoft.com/azure/ai-services/openai/how-to/structured-outputs> |
| Chat completions with Azure OpenAI | <https://learn.microsoft.com/azure/ai-services/openai/how-to/chatgpt> |
| Agentic AI workflows overview | <https://learn.microsoft.com/azure/ai-foundry/agents/overview> |
| AI agent design patterns | <https://learn.microsoft.com/azure/architecture/ai-ml/guide/> |
