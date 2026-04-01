# AP.Handoff — Handoff (Triage) Workflow

## What This Pattern Does

The **Handoff** pattern implements a triage-and-route architecture. A single entry-point agent (triage agent) analyses each incoming message and delegates it to the most appropriate specialist agent. After a specialist responds, control can return to the triage agent for the next turn.

This demo has three agents:

| Agent | Role |
|-------|------|
| `triage_agent` | Classifies the question and hands off to the right specialist |
| `history_tutor` | Answers historical questions only |
| `math_tutor` | Solves math problems only |

## When to Use It

- You want a single entry point that routes to domain specialists.
- Different agents have distinct competencies that should not overlap.
- Multi-turn conversations where the user asks questions across different domains.
- Customer support routing, helpdesk triage, multi-domain Q&A systems.

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

cd src/AgentPatterns/AP.Handoff
dotnet run
```

The demo runs an interactive loop — type a question and press Enter, or type `exit` to quit.

## Code Walkthrough

### Defining Agents

Each agent has a system prompt scoped to its domain:

```csharp
ChatClientAgent historyTutor = new(client,
    "You provide assistance with historical queries...",
    "history_tutor", "Specialist agent for historical questions");

ChatClientAgent triageAgent = new(client,
    "You determine which agent to use ... ALWAYS handoff to another agent.",
    "triage_agent", "Routes messages to the appropriate specialist agent");
```

### Building the Handoff Workflow

Handoff rules define which agents can transfer control to which others:

```csharp
var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triageAgent)
    .WithHandoffs(triageAgent, [mathTutor, historyTutor])  // triage routes to specialists
    .WithHandoffs([mathTutor, historyTutor], triageAgent)  // specialists return to triage
    .Build();
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `AgentWorkflowBuilder.CreateHandoffBuilderWith` | Creates a builder starting with the entry-point agent |
| `HandoffWorkflowBuilder.WithHandoffs` | Declares routing rules between agents |
| `InProcessExecution.RunStreamingAsync` | Runs the workflow interactively |
