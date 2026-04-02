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

### Function Calling & Tool Use

| Resource | Link |
|----------|------|
| Azure OpenAI function calling | <https://learn.microsoft.com/azure/ai-services/openai/how-to/function-calling> |
| AIFunctionFactory API reference | <https://learn.microsoft.com/dotnet/api/microsoft.extensions.ai.aifunctionfactory> |
| Model Context Protocol (MCP) | <https://modelcontextprotocol.io/> |
| MCP .NET SDK (csharp-sdk) | <https://github.com/modelcontextprotocol/csharp-sdk> |

### Vector Search & RAG

| Resource | Link |
|----------|------|
| Azure AI Search overview | <https://learn.microsoft.com/azure/search/search-what-is-azure-search> |
| Vector search in Azure AI Search | <https://learn.microsoft.com/azure/search/vector-search-overview> |
| Create a vector index | <https://learn.microsoft.com/azure/search/vector-search-how-to-create-index> |
| Azure OpenAI embeddings | <https://learn.microsoft.com/azure/ai-services/openai/concepts/understand-embeddings> |
| Retrieval-Augmented Generation | <https://learn.microsoft.com/azure/search/retrieval-augmented-generation-overview> |

### Chat History & Memory

| Resource | Link |
|----------|------|
| Chat history in Microsoft.Extensions.AI | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| Azure OpenAI chat completions | <https://learn.microsoft.com/azure/ai-services/openai/how-to/chatgpt> |

### Azure OpenAI Models

| Resource | Link |
|----------|------|
| Available models | <https://learn.microsoft.com/azure/ai-services/openai/concepts/models> |
| GPT-4o overview | <https://learn.microsoft.com/azure/ai-services/openai/concepts/models#gpt-4o> |
| Deployment management | <https://learn.microsoft.com/azure/ai-services/openai/how-to/create-resource> |

---

## 📄 License

This project is licensed under the terms of the [LICENSE](./LICENSE) file.

