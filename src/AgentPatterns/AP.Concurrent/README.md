# AP.Concurrent — Concurrent Agent Workflow

## What This Pattern Does

The **Concurrent** pattern runs multiple agents simultaneously on the same input and then merges their results. Unlike the [Sequential](../AP.Sequential/README.md) pattern, agents do not depend on each other's output — they all start at the same time.

In this demo, three translation agents each independently translate the user's message:

- **Slovenian translator**
- **Spanish translator**
- **English translator**

All three run in parallel; a merge function combines their responses into a single result list.

## When to Use It

- You need the same task performed in multiple ways at once (fan-out pattern).
- You want to compare or aggregate results from independent agents.
- Latency matters — parallel execution is faster than sequential when agents are independent.
- Examples: multi-language responses, consensus voting, parallel summarisation.

## Concurrent vs. Sequential

| Aspect | Sequential | Concurrent |
|--------|-----------|------------|
| Execution | One agent at a time | All agents at once |
| Data flow | Output feeds next input | Shared input, independent outputs |
| Use case | Transformation pipeline | Fan-out / aggregation |
| Latency | Sum of all durations | Max of all durations |

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

cd src/AgentPatterns/AP.Concurrent
dotnet run
```

On Windows (PowerShell):

```powershell
$env:Endpoint = "https://<your-resource>.openai.azure.com/"
$env:DeploymentName = "gpt-4o"

dotnet run
```

## Code Walkthrough

### `AgentHelper.cs`

Identical structure to the Sequential helper — the agent configuration is the same; only the workflow builder differs:

```csharp
public IEnumerable<ChatClientAgent> GetTranslationAgents() =>
    from lang in (string[])["Slovenian", "Spanish", "English"]
    select GetTranslationAgent(lang, client);
```

### `Program.cs`

`AgentWorkflowBuilder.BuildConcurrent` takes a merge function that decides how the parallel results are combined:

```csharp
var workflow = AgentWorkflowBuilder.BuildConcurrent(
    translationAgents,
    results => results.SelectMany(r => r).ToList());
```

The streaming run then watches for `AgentResponseUpdateEvent` from each parallel agent.

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `AgentHelper` | Creates and configures `ChatClientAgent` instances |
| `AgentWorkflowBuilder.BuildConcurrent` | Wires agents into a concurrent (fan-out) workflow |
| `InProcessExecution.RunStreamingAsync` | Executes the workflow and returns a streaming run |
| `StreamingRun.WatchStreamAsync` | Async stream of `WorkflowEvent` updates from all agents |
