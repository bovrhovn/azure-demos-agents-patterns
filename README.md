# Azure Demos — Agents & Patterns

A collection of demos showcasing different **agent patterns** built with the [Microsoft Agent Framework](https://microsoft.github.io/autogen/), [Semantic Kernel](https://learn.microsoft.com/semantic-kernel/), and [Azure AI Foundry](https://ai.azure.com/).

---

## 📁 Repository Structure

| Folder | Purpose |
|--------|---------|
| [`src/`](./src/README.md) | Source code — one sub-folder per agent pattern or demo |
| [`tests/`](./tests/README.md) | Automated tests mirroring the `src/` structure |
| [`docs/`](./docs/README.md) | Architecture docs, ADRs, guides, and runbooks |

---

## 🗺️ Patterns Overview

| Pattern | Project | Description |
|---------|---------|-------------|
| **Sequential** | [AP.Sequential](src/AgentPatterns/AP.Sequential/README.md) | Chain agents in a fixed order — each agent's output feeds the next |
| **Concurrent** | [AP.Concurrent](src/AgentPatterns/AP.Concurrent/README.md) | Run agents in parallel on the same input and merge results |
| **Handoff** | [AP.Handoff](src/AgentPatterns/AP.Handoff/README.md) | Triage agent routes requests to the appropriate specialist |
| **Approval** | [AP.Approval](src/AgentPatterns/AP.Approval/README.md) | Human approval gate around sensitive tool calls |
| **Group Chat** | [AP.GroupChat](src/AgentPatterns/AP.GroupChat/README.md) | Multi-agent conversation with custom termination logic |
| **Human-in-the-Loop** | [AP.HumanInTheLoop](src/AgentPatterns/AP.HumanInTheLoop/README.md) | Typed external input injected directly into the workflow graph |
| **Memory** | [AP.Memory](src/AgentPatterns/AP.Memory/README.md) | Agent session with in-memory chat history storage and retrieval |
| **Model Router** | [AP.ModelRouter](src/AgentPatterns/AP.ModelRouter/README.md) | Inspect `ModelId` on responses; foundation for routing across models |
| **Vector Store** | [AP.AzureSearchAsVectorStore](src/AgentPatterns/AP.AzureSearchAsVectorStore/README.md) | Use Azure AI Search as a vector store for semantic / kNN search |
| **MCP Server** | [AP.TaxServer](src/AgentPatterns/AP.TaxServer/README.md) | ASP.NET Core server exposing a tool over Model Context Protocol |
| **MCP Client** | [AP.TaxClient](src/AgentPatterns/AP.TaxClient/README.md) | AI client that discovers and invokes remote MCP tools |
| **Method as Tool** | [AP.UseMethodForAI](src/AgentPatterns/AP.UseMethodForAI/README.md) | Wrap a plain .NET method with `AIFunctionFactory` for in-process tool use |
| **Critic–Writer Loop** | [AP.Workflow](src/AgentPatterns/AP.Workflow/README.md) | Iterative Writer→Critic feedback loop with structured JSON output and shared workflow state |
| **Application Insights** | [AP.AppInsights](src/AgentPatterns/AP.AppInsights/README.md) | Wire Azure Application Insights + OpenTelemetry into an `IChatClient` pipeline for traces and metrics |
| **Foundry Basic** | [AP.FoundryBasic](src/AgentPatterns/AP.FoundryBasic/README.md) | Call a pre-deployed agent on Azure AI Foundry using the `Azure.AI.Projects` SDK |
| **WorkIQ MCP Client** | [AP.WorkIQ](src/AgentPatterns/AP.WorkIQ/README.md) | Discover and invoke Microsoft 365 tools via the WorkIQ MCP server and the stdio transport |

---

## ✅ Prerequisites

| Requirement | Details |
|-------------|---------|
| .NET 10 SDK | <https://dot.net> |
| Azure subscription | Required for Azure OpenAI |
| Azure OpenAI resource | Create one at <https://portal.azure.com> |
| Azure CLI / `az login` | Used by `DefaultAzureCredential` for local authentication |

---

## 🚀 Getting Started

1. Browse the [`src/`](./src/README.md) folder to find an agent pattern you are interested in.
2. Read the `README.md` inside that pattern's sub-folder for setup and run instructions.
3. Check [`docs/`](./docs/README.md) for architecture overviews and cross-cutting guidelines.

---

## ⚙️ Running the Demos

Most demos require two environment variables:

```bash
# Bash / macOS / Linux
export Endpoint="https://<your-resource>.openai.azure.com/"
export DeploymentName="gpt-4o"
```

```powershell
# PowerShell / Windows
$env:Endpoint = "https://<your-resource>.openai.azure.com/"
$env:DeploymentName = "gpt-4o"
```

Then run any demo from its project directory:

```bash
cd src/AgentPatterns/AP.Sequential
dotnet run
```

> **Authentication:** The demos use `DefaultAzureCredential`, so run `az login` before starting (or use a managed identity in Azure).

---

## 🤝 Contributing

Contributions are welcome! Please:

- Add new patterns as sub-folders under `src/`.
- Include tests in the matching sub-folder under `tests/`.
- Document your pattern with a `README.md` and update [`docs/`](./docs/README.md) as needed.

---

## 📚 Learn More

### Core Frameworks

| Resource | Link |
|----------|------|
| Microsoft.Agents.AI | <https://learn.microsoft.com/dotnet/ai/> |
| Azure AI Foundry | <https://learn.microsoft.com/azure/ai-foundry/> |
| Azure OpenAI Service | <https://learn.microsoft.com/azure/ai-services/openai/> |
| Semantic Kernel | <https://learn.microsoft.com/semantic-kernel/> |
| Microsoft.Extensions.AI | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| Azure Identity / DefaultAzureCredential | <https://learn.microsoft.com/dotnet/azure/sdk/authentication/> |

### Agent Patterns & Architecture

| Resource | Link |
|----------|------|
| Multi-agent systems on Azure AI Foundry | <https://learn.microsoft.com/azure/ai-foundry/concepts/agents> |
| AI agent design patterns | <https://learn.microsoft.com/azure/architecture/ai-ml/guide/> |
| Agentic AI workflows overview | <https://learn.microsoft.com/azure/ai-foundry/agents/overview> |
| Responsible AI for agents | <https://learn.microsoft.com/azure/ai-services/responsible-use-of-ai-overview> |

### Sequential & Concurrent Patterns

| Resource | Link |
|----------|------|
| Chaining chat completions | <https://learn.microsoft.com/azure/ai-services/openai/how-to/chatgpt> |
| Parallel task execution in .NET | <https://learn.microsoft.com/dotnet/standard/parallel-programming/task-parallel-library-tpl> |
| Task.WhenAll for concurrent work | <https://learn.microsoft.com/dotnet/api/system.threading.tasks.task.whenall> |
| Streaming chat completions | <https://learn.microsoft.com/azure/ai-services/openai/how-to/streaming> |

### Handoff & Routing Patterns

| Resource | Link |
|----------|------|
| Multi-agent orchestration | <https://learn.microsoft.com/azure/ai-foundry/concepts/agents#multi-agent-scenarios> |
| AI agent triage and routing | <https://learn.microsoft.com/azure/architecture/ai-ml/guide/> |
| Group chat with custom managers | <https://learn.microsoft.com/azure/ai-foundry/agents/overview> |

### Human-in-the-Loop & Approval Patterns

| Resource | Link |
|----------|------|
| Human oversight of AI agents | <https://learn.microsoft.com/azure/ai-services/responsible-use-of-ai-overview> |
| Responsible AI principles | <https://learn.microsoft.com/azure/ai-services/responsible-use-of-ai-overview> |
| Workflow graphs with RequestPort | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |

### Structured Output (Critic–Writer Workflow)

| Resource | Link |
|----------|------|
| Structured outputs with Azure OpenAI | <https://learn.microsoft.com/azure/ai-services/openai/how-to/structured-outputs> |
| JSON schema mode | <https://learn.microsoft.com/azure/ai-services/openai/how-to/structured-outputs#json-schema> |
| ChatResponseFormat API | <https://learn.microsoft.com/dotnet/api/microsoft.extensions.ai.chatresponseformat> |
| Iterative agent refinement | <https://learn.microsoft.com/azure/ai-foundry/agents/overview> |

### Function Calling & Tool Use

| Resource | Link |
|----------|------|
| Azure OpenAI function calling | <https://learn.microsoft.com/azure/ai-services/openai/how-to/function-calling> |
| AIFunctionFactory API reference | <https://learn.microsoft.com/dotnet/api/microsoft.extensions.ai.aifunctionfactory> |
| Model Context Protocol (MCP) | <https://modelcontextprotocol.io/> |
| MCP .NET SDK (csharp-sdk) | <https://github.com/modelcontextprotocol/csharp-sdk> |
| MCP server with ASP.NET Core | <https://learn.microsoft.com/dotnet/ai/model-context-protocol> |
| Exposing .NET methods as AI tools | <https://learn.microsoft.com/dotnet/ai/quickstarts/use-function-calling> |

### Vector Search & RAG

| Resource | Link |
|----------|------|
| Azure AI Search overview | <https://learn.microsoft.com/azure/search/search-what-is-azure-search> |
| Vector search in Azure AI Search | <https://learn.microsoft.com/azure/search/vector-search-overview> |
| Create a vector index | <https://learn.microsoft.com/azure/search/vector-search-how-to-create-index> |
| Azure OpenAI embeddings | <https://learn.microsoft.com/azure/ai-services/openai/concepts/understand-embeddings> |
| Retrieval-Augmented Generation | <https://learn.microsoft.com/azure/search/retrieval-augmented-generation-overview> |
| Semantic ranking | <https://learn.microsoft.com/azure/search/semantic-search-overview> |
| kNN and ANN search algorithms | <https://learn.microsoft.com/azure/search/vector-search-ranking> |

### Chat History & Memory

| Resource | Link |
|----------|------|
| Chat history in Microsoft.Extensions.AI | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| Azure OpenAI chat completions | <https://learn.microsoft.com/azure/ai-services/openai/how-to/chatgpt> |
| Managing conversation context | <https://learn.microsoft.com/azure/ai-services/openai/how-to/chatgpt#managing-conversations> |
| In-memory vs. persistent storage | <https://learn.microsoft.com/dotnet/standard/data/> |

### Model Routing

| Resource | Link |
|----------|------|
| Azure OpenAI model overview | <https://learn.microsoft.com/azure/ai-services/openai/concepts/models> |
| Selecting models for different tasks | <https://learn.microsoft.com/azure/ai-services/openai/concepts/models#model-summary-table-and-region-availability> |
| Deployment management | <https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource> |
| Cost and latency trade-offs | <https://learn.microsoft.com/azure/ai-services/openai/concepts/models#gpt-4o> |

### Azure OpenAI Models

| Resource | Link |
|----------|------|
| Available models | <https://learn.microsoft.com/azure/ai-services/openai/concepts/models> |
| GPT-4o overview | <https://learn.microsoft.com/azure/ai-services/openai/concepts/models#gpt-4o> |
| Deployment management | <https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource> |
| Quotas and limits | <https://learn.microsoft.com/azure/ai-services/openai/quotas-limits> |

---

## 📄 License

This project is licensed under the terms of the [LICENSE](./LICENSE) file.

