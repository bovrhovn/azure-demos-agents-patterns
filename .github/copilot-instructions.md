# Copilot Instructions

## Toolchain and commands

- Use the SDK pinned in `global.json`: .NET `10.0.400-preview.0.26322.102` (with latest-patch roll-forward). All projects target `net10.0`, enable nullable reference types, and use implicit usings.
- Build the complete solution from the repository root:

  ```powershell
  dotnet build src/AgentPatterns/AgentPatterns.slnx
  ```

- Run all tests:

  ```powershell
  dotnet test tests/AgentPatterns.Tests/AgentPatterns.Tests.csproj
  ```

- Run one test or one test class with xUnit's `FullyQualifiedName` filter:

  ```powershell
  dotnet test tests/AgentPatterns.Tests/AgentPatterns.Tests.csproj --filter "FullyQualifiedName=AgentPatterns.Tests.HumanInTheLoop.JudgeExecutorTests.HandleAsync_ExactMatch_YieldsOutput"
  dotnet test tests/AgentPatterns.Tests/AgentPatterns.Tests.csproj --filter "FullyQualifiedName~AgentPatterns.Tests.HumanInTheLoop.JudgeExecutorTests"
  ```

- There is no repository-specific lint command or analyzer configuration. Run a demo by targeting its project, for example:

  ```powershell
  dotnet run --project src/AgentPatterns/AP.Sequential/AP.Sequential.csproj
  ```

## Architecture

- This is a collection of independent, executable .NET demos rather than a shared application. `src/AgentPatterns/AgentPatterns.slnx` organizes them into agent patterns, approvals, MCP, Foundry, methods, and vector-store categories; each `AP.*` project has its own `Program.cs` and README.
- Most Azure OpenAI samples construct an `AzureOpenAIClient` with `DefaultAzureCredential`, adapt it to `IChatClient`, and optionally wrap it in `ChatClientBuilder` middleware. They require `Endpoint` and `DeploymentName`; authenticate locally with `az login` rather than adding credentials to source or configuration.
- The agent-pattern projects demonstrate `Microsoft.Agents.AI` workflows: sequential/concurrent composition, handoff routing, group chat, tool approval, external workflow requests, and the writer/critic loop. Follow the event-driven execution model already used by the demos: start an `InProcessExecution` streaming run, handle `AgentResponseUpdateEvent` and `WorkflowOutputEvent`, and respond to `RequestInfoEvent` when a workflow needs external input or approval.
- `AP.Workflow` is the most stateful sample. Its writer and critic executors exchange typed messages, persist `FlowState` through `IWorkflowContext`, route via a `CriticDecision`, and cap revisions at three iterations.
- MCP samples use two distinct transports: `AP.TaxServer` is an ASP.NET Core MCP server exposing assembly-discovered tools at `/mcp` (and `/health`); `AP.TaxClient` discovers those tools through HTTP and supplies them to `ChatOptions.Tools` with `UseFunctionInvocation()`. Start the server before the client and set the client's `McpEndpoint`. `AP.WorkIQ` instead starts the WorkIQ MCP server with `npx` over stdio; it needs Node.js/npm and prior WorkIQ EULA acceptance.
- Resource-specific projects have additional inputs: `AP.AzureSearchAsVectorStore` requires `SearchEndpoint` and an embeddings deployment; `AP.FoundryBasic` uses an AI Foundry `Endpoint` plus `AgentName`; `AP.AppInsights` requires `APPLICATION_INSIGHTS_CONNECTION_STRING`. Keep `EnableSensitiveData` false unless recording prompts and completions is intentional.

## Repository conventions

- Add a new demo as a self-contained `src/AgentPatterns/AP.<Pattern>` project with a pattern-specific README, add it to `AgentPatterns.slnx`, and add matching tests under `tests/AgentPatterns.Tests/<Pattern>`. Register the source project in `AgentPatterns.Tests.csproj`.
- Keep top-level `Program.cs` focused on composing and displaying a runnable demo. Put logic that can be unit tested without Azure credentials into small named helpers, factories, configuration classes, executors, or MCP tool classes in the demo project.
- Tests mirror the source pattern name, use xUnit and NSubstitute, and cover helpers without live Azure calls. Source projects expose internal test seams through the `InternalsVisibleTo` assembly attribute in their `.csproj`; retain or add that attribute when testing internal types.
- Centralize sample prompts, agent names, tool names, environment-variable names, and default values in the pattern's helper/configuration type when those values are asserted by tests. Preserve the agent and tool names that workflow routing and MCP discovery depend on.
- Keep pattern-specific operational details in that project's README, and reserve `docs/` for cross-cutting material.
