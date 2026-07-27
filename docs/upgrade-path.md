# Microsoft Agent Framework upgrade path

> Assessment date: 2026-07-27  
> Scope: `src/AgentPatterns` and `tests/AgentPatterns.Tests`

## Implemented upgrade

The upgrade path was applied on 2026-07-27:

| Area | Implemented version or change |
|---|---|
| SDK selection | Added `global.json` pinning SDK `10.0.400-preview.0.26322.102`. |
| Agent Framework | Upgraded `Microsoft.Agents.AI`, `.Abstractions`, and `.Workflows` to 1.15.0. |
| Foundry provider | Removed all `Microsoft.Agents.AI.AzureAI` references. `AP.FoundryBasic` now uses `Microsoft.Agents.AI.Foundry` 1.15.0-preview.260722.1 and wraps a deployed Foundry agent with `AsAIAgent`. |
| Foundry SDK | Aligned `Azure.AI.Projects` to 2.1.0-beta.4, required by the Foundry provider. |
| Microsoft.Extensions.AI | Upgraded `Microsoft.Extensions.AI` and `.OpenAI` to 10.8.1. |
| MCP | Upgraded `ModelContextProtocol`, `.AspNetCore`, and `.Core` to 1.4.1. |
| Supporting packages | Upgraded the Azure Monitor OpenTelemetry exporter to 1.8.3, Spectre.Console to 0.57.2, Microsoft.NET.Test.Sdk to 18.8.1, NSubstitute to 6.0.0, and logging abstractions to 10.0.10. |
| Telemetry safety | Changed `AP.AppInsights` to opt out of sensitive telemetry by default and corrected the Application Insights connection-string guard. |

## Executive summary

The repository compiles today, but it uses Microsoft Agent Framework core/workflow packages at **1.9.0** and mixes them with pre-release `Microsoft.Agents.AI.AzureAI` versions (`1.0.0-rc4` and `1.0.0-rc5`). Current official Agent Framework documentation instead presents the Foundry provider as `Microsoft.Agents.AI.Foundry --prerelease`.

Upgrade the Agent Framework packages as one compatibility unit; do not update individual framework packages independently. Adopt the current Foundry provider only in a separate, evaluated migration because it is pre-release and changes the provider boundary.

[Microsoft Agent Framework overview](https://learn.microsoft.com/en-us/agent-framework/) describes the current platform as agents, harness, and graph-based workflows, with sessions, context providers, middleware, telemetry, and MCP integration.

## Current state

| Area | Current repository state | Assessment |
|---|---|---|
| Target framework | `net10.0` in every project | Retain unless a platform-support decision requires otherwise. The installed SDK is a preview build and there is no `global.json`, so developer and CI SDK selection is not pinned. |
| Agent Framework core | `Microsoft.Agents.AI` / `Abstractions` / `Workflows` 1.9.0 | Upgrade together to 1.15.0. |
| Azure provider | `Microsoft.Agents.AI.AzureAI` rc4 in eight samples; rc5 in `AP.Memory` | Replace after a dedicated migration spike; current Learn examples use `Microsoft.Agents.AI.Foundry --prerelease`. |
| `Microsoft.Extensions.AI` | 10.6.0 across the samples | Upgrade `Microsoft.Extensions.AI` and `.OpenAI` together to 10.8.1. |
| Azure OpenAI SDK | `Azure.AI.OpenAI` 2.1.0 | Keep on the current stable line initially. A newer 2.9.0-beta.1 exists, but should not be adopted merely to upgrade packages. |
| MCP SDK | 1.4.0 | Update `ModelContextProtocol`, `.AspNetCore`, and `.Core` to 1.4.1 together. |
| Other stable updates | App Insights exporter 1.8.1; Spectre.Console 0.55.2; test SDK 18.6.0; NSubstitute 5.3.0 | Update independently after the framework migration has a green build. |

The current build succeeds with three existing nullable warnings: one in `AP.GroupChat` and two in `AP.Workflow`. Resolve these before using warning-free builds as the migration gate.

## Recommended upgrade sequence

1. Pin the .NET SDK with `global.json`, then create a branch and preserve a green baseline build and test run.
2. Update every direct `Microsoft.Agents.AI`, `.Abstractions`, and `.Workflows` reference from 1.9.0 to 1.15.0 in one change. Do not let NuGet resolve a mixture of 1.9.x and 1.15.x.
3. Upgrade `Microsoft.Extensions.AI` and `Microsoft.Extensions.AI.OpenAI` together from 10.6.0 to 10.8.1.
4. Build and exercise each agent/workflow pattern, especially streaming, group chat selection, handoff, human input, workflow loops, and tool calls.
5. Run a separate provider migration spike: replace `Microsoft.Agents.AI.AzureAI` with the current Foundry provider package and adapt the affected samples to the documented `AIProjectClient(...).AsAIAgent(...)` pattern. Do not ship the pre-release provider without locking its version and passing integration tests.
6. Upgrade the MCP packages to 1.4.1 and run an authenticated client-to-server integration test against `AP.TaxServer`.
7. Upgrade the telemetry, console, and test packages; then evaluate the Azure OpenAI beta separately.
8. Add a package-update policy: central package management or a shared props file, locked restore, and Dependabot/Renovate grouping for Agent Framework and Microsoft.Extensions.AI packages.

## Expected upgrade challenges

| Change | Likely challenge | Required mitigation |
|---|---|---|
| Agent Framework 1.9.0 to 1.15.0 | Source and behavioral compatibility changes in workflows, events, agent sessions, streaming, and tool invocation. | Compile all projects together; add focused tests for each pattern's final output and event stream. |
| `AzureAI` RC provider to Foundry provider | The package name, transitive Azure AI Projects dependencies, configuration conventions, and provider APIs can change. The target remains pre-release. | Treat as a migration rather than a patch update; pin exact versions, update one sample first, then migrate the rest. |
| Package version skew | `Microsoft.Agents.AI.AzureAI` currently brings core/transitive agent dependencies while projects directly reference workflow packages. Partial updates can cause restore conflicts or runtime type-load failures. | Use one version matrix for all Agent Framework packages and inspect `dotnet list package --include-transitive` after each update. |
| Model migration | A deployment name is not a model identifier guarantee. The selected model must exist in the chosen region and deployment type, and some model/API combinations do not support all parameters. | Make model deployment names configuration, validate availability/quota first, and run contract tests for structured output and function calling. |
| Responses API adoption | Responses enables additional hosted tools, but tool availability differs from Chat Completions. | Choose the client/API per required tool. Validate every required tool against the Agent Framework provider support matrix. |
| Embedding-model migration | Changing embeddings changes dimensions and vector values. Azure AI Search data indexed with the old model is not compatible with a new embedding model. | Create a new index/profile and re-embed all content; test retrieval quality and cost before cutover. |
| SDK selection | The repository currently builds with a preview .NET SDK selected implicitly. | Add `global.json`, require a supported SDK in CI, and run restore/build/test on the exact pinned SDK. |

## What the upgrade enables

- Current Agent Framework agent, workflow, session, middleware, telemetry, context/memory, and MCP capabilities.
- A documented Foundry provider path for persistent server-side agents and managed chat history.
- A provider choice based on required features: Azure OpenAI, OpenAI, and Foundry support function tools, structured outputs, code interpreter, file search, MCP tools, and background responses; capabilities differ for other providers.
- Tool approval as a first-class agent flow: a run returns approval requests, and the caller must explicitly approve or reject each request before continuing.
- An agent can be exposed as an MCP tool through `AsAIFunction()`, or use MCP tools provided by an approved server.

## Model recommendations

Select models only after evaluating representative prompts, tool calls, latency, token use, regional availability, and quota. Do not infer capability or pricing from a deployment name.

| Workload in this repository | Recommended evaluation candidates | Rationale and caveats |
|---|---|---|
| High-value planning, critic/writer, complex handoff, and safety-sensitive decisions | `gpt-5.6-terra`, `gpt-5.6-sol`, `gpt-5.6-luna`; use `gpt-5.4` where 5.6 is unavailable | The current catalog lists reasoning, Responses and Chat Completions, structured outputs, text/image processing, function tools, and parallel tool calling for these models. Select a candidate through evaluation rather than assuming a tier relationship. |
| Default general-purpose agents, sequential/concurrent translations, standard tool calling | `gpt-5.4-mini` | The framework's current C# quickstart uses it, and the catalog lists reasoning, structured output, function tools, and parallel tool calling. It is the recommended default migration target for these demos. |
| Simple routing/classification and high-volume low-risk work | `gpt-5.4-nano`, with quality gates | It supports the same core API/tool features, but requires quality evaluation before use in routing decisions. |
| Existing deployment compatibility or multimodal baseline | `gpt-4.1-mini` or `gpt-4o-mini` | Both are valid migration baselines; GPT-4.1 supports Chat Completions, Responses, streaming, function calling, and structured output. |
| Vector search in `AP.AzureSearchAsVectorStore` | `text-embedding-3-large`; evaluate `text-embedding-3-small` for cost-sensitive retrieval | `text-embedding-3-large` is documented as the latest and most capable embedding model. Changing models requires a full re-embedding and index migration. |

Avoid preview model aliases for production defaults. Treat any newly deployed model as an application behavior change, not a configuration-only change.

## Security actions required before production use

1. **Use a production credential explicitly.** All current samples use `DefaultAzureCredential`. It is appropriate for local development, but Microsoft cautions that production should use a specific credential such as `ManagedIdentityCredential` to avoid fallback probing, unintended credential selection, and added latency. Assign only the required Azure RBAC role, such as `Cognitive Services OpenAI User`.
2. **Protect telemetry and prompts.** `AP.AppInsights` defaults `EnableSensitiveData` to `true`. Change production configuration to opt in only after approved redaction, data classification, retention, and access controls are in place. Do not emit prompts, tool arguments, retrieved documents, tokens, or approval decisions by default.
3. **Require approval for privileged tools.** `AP.Approval` already protects deployment. Apply the same `ApprovalRequiredAIFunction` pattern to state-changing, destructive, payment, data-export, identity, and administrative tools. Display the exact tool name and validated arguments; persist an audited allow/deny decision.
4. **Constrain every function tool.** Use explicit tool allowlists per agent, narrow schemas, server-side authorization, input validation, rate/transaction limits, and idempotency where relevant. Instructions alone are not an authorization boundary.
5. **Treat MCP servers as third parties.** `AP.TaxClient` accepts an endpoint and `AP.WorkIQ` starts an external stdio transport. Microsoft notes that prompts and returned data can cross the MCP-server boundary. Allowlist trusted servers, use TLS for remote connections, avoid proxy servers, scope credentials per server/run, and audit server/tool registration and invocations.
6. **Keep secrets out of source, process arguments, and shared client configuration.** Use managed identity or a secret store; inject short-lived credentials at runtime. Rotate immediately after suspected exposure.
7. **Implement responsible-AI controls.** Microsoft places the responsibility for metaprompts, content filters, safety systems, and testing on the application owner. Add prompt-injection tests, content filtering, output validation, grounded retrieval, and human escalation for high-impact actions.
8. **Use least privilege and isolation.** Separate identities for inference, Azure AI Search, Application Insights, and MCP tools. Put outbound network controls around agents that can access data or tools.

## Official sources

- [Microsoft Agent Framework overview](https://learn.microsoft.com/en-us/agent-framework/)
- [Agent Framework providers overview](https://learn.microsoft.com/en-us/agent-framework/agents/providers/)
- [Agent Framework tools overview and provider support matrix](https://learn.microsoft.com/en-us/agent-framework/agents/tools/)
- [Function-tool approval](https://learn.microsoft.com/en-us/agent-framework/agents/tools/tool-approval)
- [MCP tools with agents and third-party considerations](https://learn.microsoft.com/en-us/agent-framework/agents/tools/local-mcp-tools)
- [Semantic Kernel to Agent Framework migration guide](https://learn.microsoft.com/en-us/agent-framework/migration-guide/from-semantic-kernel/)
- [Microsoft Foundry models sold by Azure](https://learn.microsoft.com/en-us/azure/ai-foundry/openai/concepts/models)
- [Microsoft Entra ID and managed identity authentication for Azure OpenAI](https://learn.microsoft.com/en-us/azure/ai-foundry/openai/how-to/managed-identity)
- [Responsible AI practices for Azure OpenAI models](https://learn.microsoft.com/en-us/azure/ai-foundry/responsible-ai/openai/overview)
