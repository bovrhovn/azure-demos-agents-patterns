# AP.Sequential — Sequential Agent Workflow

## What This Pattern Does

The **Sequential** pattern chains multiple agents in a fixed order. Each agent receives the output of the previous one as input. The workflow finishes when the last agent in the chain completes.

In this demo, three translation agents process a single user message one after another:

1. **Slovenian translator** — translates the input from English to Slovenian  
2. **Spanish translator** — translates the Slovenian output to Spanish  
3. **English translator** — translates back to English, completing the round-trip

## When to Use It

- You need deterministic, ordered processing across multiple agents.
- Each step's output is the next step's input (pipeline/chain pattern).
- Tasks such as: translate → summarize → format, or extract → validate → store.

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

cd src/AgentPatterns/AP.Sequential
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

`AgentHelper` is a factory that creates `ChatClientAgent` instances, each configured with language-specific instructions:

```csharp
public IEnumerable<ChatClientAgent> GetTranslationAgents() =>
    from lang in (string[])["Slovenian", "Spanish", "English"]
    select GetTranslationAgent(lang, client);
```

Each agent's system prompt instructs it to respond only in the target language and to name the source language.

### `Program.cs`

1. Reads `Endpoint` and `DeploymentName` from environment variables.
2. Creates an `AzureOpenAIClient` with `DefaultAzureCredential`.
3. Builds the sequential workflow with `AgentWorkflowBuilder.BuildSequential(agents)`.
4. Starts a streaming run and prints each agent's streamed response.

```csharp
var workflow = AgentWorkflowBuilder.BuildSequential(translationAgents);
await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, messages);
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `AgentHelper` | Creates and configures `ChatClientAgent` instances |
| `AgentWorkflowBuilder.BuildSequential` | Wires agents into a sequential pipeline |
| `InProcessExecution.RunStreamingAsync` | Executes the workflow and returns a streaming run |
| `StreamingRun.WatchStreamAsync` | Async stream of `WorkflowEvent` updates |
