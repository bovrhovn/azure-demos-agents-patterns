# AP.FoundryBasic — Azure AI Foundry SDK Basic

## What This Pattern Does

The **FoundryBasic** pattern demonstrates how to call a pre-deployed agent on [Azure AI Foundry](https://ai.azure.com/) using the `Microsoft.Agents.AI.Foundry` provider and `Azure.AI.Projects` SDK. Rather than building an agent programmatically, this demo references an existing named Foundry agent and invokes it through the common `AIAgent` API.

This demo:
1. Creates an `AIProjectClient` pointing at your Foundry endpoint.
2. Resolves a named agent by `AgentName`.
3. Wraps the Foundry agent as a `FoundryAgent`.
4. Calls `RunAsync` with a natural-language prompt.
5. Prints the agent's text output.

## When to Use It

- You have an agent defined and managed in the Azure AI Foundry portal.
- You want a minimal integration with a Foundry-managed agent through Microsoft Agent Framework.
- You need to call a Foundry agent from a .NET application without the overhead of local orchestration.

## Prerequisites

| Requirement | Details |
|-------------|---------|
| .NET 10 SDK | <https://dot.net> |
| Azure AI Foundry project | Create one at <https://ai.azure.com/> |
| Deployed agent | An agent deployed inside your Foundry project |
| `Endpoint` env var | Your Azure AI Foundry project endpoint URL |
| `AgentName` env var | The name of the deployed agent to call |

## Running the Demo

```bash
export Endpoint="https://<your-project>.services.ai.azure.com/api/projects/<project-name>"
export AgentName="<your-agent-name>"

cd src/AgentPatterns/AP.FoundryBasic
dotnet run
```

On Windows (PowerShell):

```powershell
$env:Endpoint  = "https://<your-project>.services.ai.azure.com/api/projects/<project-name>"
$env:AgentName = "<your-agent-name>"

dotnet run
```

## Code Walkthrough

### Connecting to the Foundry Project

`AIProjectClient` authenticates via `DefaultAzureCredential` — run `az login` beforehand for local development:

```csharp
var credentials = new DefaultAzureCredential();
AIProjectClient projectClient = new(endpoint: new Uri(endpoint), tokenProvider: credentials);
```

### Calling the Agent

Resolve the agent by name, wrap it as an `AIAgent`, and call `RunAsync` with your prompt:

```csharp
var agentRecord = await projectClient.AgentAdministrationClient.GetAgentAsync(agentName);
FoundryAgent agent = projectClient.AsAIAgent(agentRecord);
AgentResponse response = await agent.RunAsync("Give me MSFT stock info.");
Console.WriteLine(response.Text);
```

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `FoundryConfig` | Constants for environment variable names and the default prompt |
| `AIProjectClient` | Entry point for the Azure AI Foundry project SDK |
| `AgentAdministrationClient.GetAgentAsync` | Retrieves the current version of an existing Foundry agent by name |
| `AsAIAgent` | Wraps the named Foundry agent as an Agent Framework `FoundryAgent` |
| `RunAsync` | Sends a prompt and returns the agent's response |

## Learn More

| Resource | Link |
|----------|------|
| Azure AI Foundry overview | <https://learn.microsoft.com/azure/ai-foundry/> |
| Azure AI Projects SDK | <https://learn.microsoft.com/azure/ai-foundry/how-to/develop/sdk-overview> |
| DefaultAzureCredential | <https://learn.microsoft.com/dotnet/azure/sdk/authentication/> |
| Azure AI Foundry agents | <https://learn.microsoft.com/azure/ai-foundry/concepts/agents> |
