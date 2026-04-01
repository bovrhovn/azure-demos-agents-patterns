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

| Resource | Link |
|----------|------|
| Microsoft Agent Framework (Microsoft.Agents.AI) | <https://learn.microsoft.com/dotnet/ai/> |
| Azure AI Foundry | <https://learn.microsoft.com/azure/ai-foundry/> |
| Azure OpenAI Service | <https://learn.microsoft.com/azure/ai-services/openai/> |
| Semantic Kernel | <https://learn.microsoft.com/semantic-kernel/> |
| Microsoft.Extensions.AI | <https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai> |
| Azure Identity / DefaultAzureCredential | <https://learn.microsoft.com/dotnet/azure/sdk/authentication/> |
| Multi-agent systems on Azure AI Foundry | <https://learn.microsoft.com/azure/ai-foundry/concepts/agents> |
| AI agent design patterns | <https://learn.microsoft.com/azure/architecture/ai-ml/guide/> |

---

## 📄 License

This project is licensed under the terms of the [LICENSE](./LICENSE) file.

