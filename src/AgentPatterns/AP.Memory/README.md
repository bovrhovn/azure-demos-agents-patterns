# AP.Memory — Agent Session Memory

## What This Pattern Does

The **Memory** pattern demonstrates how an agent maintains conversation history across turns using an in-memory storage provider. The demo creates an agent with a persistent `AgentSession`, sends a message, and then reads back the full conversation history stored inside the session.

This is useful when you want to:
- Inspect messages the agent sent and received in a session.
- Build tooling that observes or replays conversation history.
- Understand the difference between stateless single-shot calls and session-based multi-turn conversations.

## When to Use It

- Multi-turn chatbots that must recall previous exchanges.
- Audit-trail scenarios where every message must be retrievable after the conversation ends.
- Debugging agent behaviour by replaying or inspecting full message history.

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

cd src/AgentPatterns/AP.Memory
dotnet run
```

On Windows (PowerShell):

```powershell
$env:Endpoint = "https://<your-resource>.openai.azure.com/"
$env:DeploymentName = "gpt-4o"

dotnet run
```

## Code Walkthrough

### Creating the Agent and Session

The agent is created directly from an `IChatClient` using the `AsAIAgent` extension method. A session scopes the conversation history:

```csharp
var agent = client.AsAIAgent(instructions: "You are a helpful assistant.", name: "Assistant");
AgentSession session = await agent.CreateSessionAsync();
var response = await agent.RunAsync("Tell me a joke about a pirate.", session);
```

### Reading History from the Session

After the conversation, the in-memory provider exposes the stored messages:

```csharp
var provider = agent.GetService<InMemoryChatHistoryProvider>();
List<ChatMessage>? messages = provider?.GetMessages(session);
```

Each message includes the `Role` (System, User, Assistant) and the text content, making it straightforward to replay or log the entire exchange.

## Key Classes

| Class / Method | Purpose |
|----------------|---------|
| `IChatClient.AsAIAgent` | Wraps a chat client as an `AIAgent` with optional instructions |
| `AIAgent.CreateSessionAsync` | Creates a new conversation session with isolated history |
| `AIAgent.RunAsync` | Sends a message and returns a response within the session |
| `InMemoryChatHistoryProvider` | Default in-process storage for session messages |
| `InMemoryChatHistoryProvider.GetMessages` | Retrieves all messages stored for a session |

## Learn More

| Resource | Link |
|----------|------|
| Microsoft.Agents.AI | <https://learn.microsoft.com/dotnet/ai/> |
| Chat history in Microsoft.Extensions.AI | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| Azure OpenAI Service | <https://learn.microsoft.com/azure/ai-services/openai/> |
