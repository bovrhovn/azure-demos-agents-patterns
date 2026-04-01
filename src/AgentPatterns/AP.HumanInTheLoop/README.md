# AP.HumanInTheLoop — Human-in-the-Loop Workflow

## What This Pattern Does

The **Human-in-the-Loop** pattern integrates external input directly into a workflow graph. Instead of relying on agents making free-form decisions, the workflow explicitly pauses at a `RequestPort` and waits for a typed signal from a human operator (or another system).

This demo implements a binary-search number-guessing game:

1. The workflow asks the human to enter a number.
2. `JudgeExecutor` compares the guess to a hidden target (42).
3. It signals `Above`, `Below`, or yields the success message.
4. The workflow loops back to the `RequestPort` until the correct number is guessed.

## When to Use It

- Workflows that require validated external input at specific decision points.
- Scenarios where human judgment must be injected mid-workflow.
- Iterative refinement loops where a human provides feedback each iteration.
- Any case where the workflow graph must explicitly model the human as a node.

## Difference from the Approval Pattern

| Aspect | Approval (AP.Approval) | Human-in-the-Loop (AP.HumanInTheLoop) |
|--------|------------------------|--------------------------------------|
| Integration point | Tool call gate | First-class workflow node (`RequestPort`) |
| Trigger | Agent tries to call a guarded tool | Workflow reaches the port in the graph |
| Data type | Tool approval request | Strongly-typed generic signal |
| Use case | Approve/reject agent actions | Supply external data to the computation |

## Prerequisites

| Requirement | Details |
|-------------|---------|
| .NET 10 SDK | <https://dot.net> |

> **Note:** This demo does not require an Azure OpenAI endpoint — `JudgeExecutor` is pure logic.

## Running the Demo

```bash
cd src/AgentPatterns/AP.HumanInTheLoop
dotnet run
```

Follow the prompts in the console to guess the hidden number.

## Code Walkthrough

### `WorkflowFactory.cs`

The workflow graph is built explicitly using `WorkflowBuilder`:

```csharp
RequestPort numberRequestPort = RequestPort.Create<NumberSignal, int>("GuessNumber");
JudgeExecutor judgeExecutor = new(42);

return new WorkflowBuilder(numberRequestPort)
    .AddEdge(numberRequestPort, judgeExecutor)   // port → judge
    .AddEdge(judgeExecutor, numberRequestPort)   // judge → port (loop back)
    .WithOutputFrom(judgeExecutor)
    .Build();
```

### `JudgeExecutor`

`JudgeExecutor` is an `Executor<int>` that handles incoming integer guesses:

```csharp
public override async ValueTask HandleAsync(int message, IWorkflowContext context, ...)
{
    _tries++;
    if (message == _targetNumber)
        await context.YieldOutputAsync($"{_targetNumber} found in {_tries} tries!");
    else if (message < _targetNumber)
        await context.SendMessageAsync(NumberSignal.Below, ...);
    else
        await context.SendMessageAsync(NumberSignal.Above, ...);
}
```

### `NumberSignal` Enum

The signal enum drives branching without needing string comparisons:

```csharp
internal enum NumberSignal { Init, Above, Below }
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `WorkflowFactory.BuildWorkflow` | Constructs and returns the workflow graph |
| `JudgeExecutor` | Core logic: compares guess to target, emits signals or output |
| `RequestPort.Create<TSignal, TMessage>` | Creates a typed input port in the workflow graph |
| `IWorkflowContext.YieldOutputAsync` | Emits the final output and ends the workflow |
| `IWorkflowContext.SendMessageAsync` | Sends a signal to the next node in the graph |
