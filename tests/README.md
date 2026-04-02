# Tests

Unit tests for the **AgentPatterns** source code, written with [xUnit](https://xunit.net/) and [NSubstitute](https://nsubstitute.github.io/).

---

## Structure

```
tests/
└── AgentPatterns.Tests/
    ├── AgentPatterns.Tests.csproj
    ├── GlobalUsings.cs
    ├── Sequential/
    │   └── AgentHelperTests.cs                     # AgentHelper factory — translation agents
    ├── Concurrent/
    │   └── AgentHelperTests.cs                     # AgentHelper factory — concurrent variant
    ├── Approval/
    │   └── DeploymentGroupChatManagerTests.cs      # Custom speaker selection logic
    ├── GroupChat/
    │   └── ApprovalBasedManagerTests.cs            # Approval-based termination logic
    ├── HumanInTheLoop/
    │   ├── JudgeExecutorTests.cs                   # Number guess evaluation logic
    │   └── WorkflowFactoryTests.cs                 # Workflow graph construction
    ├── UseMethodForAI/
    │   └── DotnetMethodHelperTests.cs              # In-process tax calculation method
    ├── AzureSearchAsVectorStore/
    │   └── MovieFactoryTests.cs                    # Movie catalogue factory and Movie model
    └── TaxServer/
        └── McpToolsTests.cs                        # MCP tool — tax calculation with ILogger mock
```

---

## Running Tests

From the repository root:

```powershell
# Run all tests
dotnet test tests/AgentPatterns.Tests/AgentPatterns.Tests.csproj

# Run with verbose output
dotnet test tests/AgentPatterns.Tests/AgentPatterns.Tests.csproj --verbosity normal

# Run a specific test class
dotnet test tests/AgentPatterns.Tests/AgentPatterns.Tests.csproj --filter "ClassName=JudgeExecutorTests"
```

Or run all tests in the solution from `src/AgentPatterns/`:

```powershell
dotnet test src/AgentPatterns/AgentPatterns.slnx
```

---

## What Is Tested

| Test File | Covers | Key Assertions |
|-----------|--------|----------------|
| `Sequential/AgentHelperTests` | `AgentHelper` (Sequential) | Returns 3 agents; each contains target language in instructions |
| `Concurrent/AgentHelperTests` | `AgentHelper` (Concurrent) | Same factory behaviour as Sequential variant |
| `Approval/DeploymentGroupChatManagerTests` | `DeploymentGroupChatManager` | First iteration → QA Engineer; subsequent iterations → DevOps Engineer; empty history → exception |
| `GroupChat/ApprovalBasedManagerTests` | `ApprovalBasedManager` | Terminates on approver saying "approve" (case-insensitive); non-approver ignored; empty history → false |
| `HumanInTheLoop/JudgeExecutorTests` | `JudgeExecutor` | Correct guess → `YieldOutput`; too low → `Below` signal; too high → `Above` signal; try count tracked |
| `HumanInTheLoop/WorkflowFactoryTests` | `WorkflowFactory`, `NumberSignal` | Workflow builds non-null; signals have correct values |
| `UseMethodForAI/DotnetMethodHelperTests` | `DotnetMethodHelper.CalculateTax` | "Method" doubles months; "Tax" halves months; unknown customer unchanged; result contains name and count |
| `AzureSearchAsVectorStore/MovieFactoryTests` | `MovieFactory`, `Movie` | Returns 4 movies with unique keys; all fields populated; `Movie` POCO properties set correctly |
| `TaxServer/McpToolsTests` | `McpTools.CalculateTax` | Same tax logic as `DotnetMethodHelper`; `ILogger` receives `Information` calls |

---

## Guidelines

- Mirror the folder structure of [`src/`](../src/README.md) so tests are easy to locate.
- Use `InternalsVisibleTo` in source `.csproj` files to expose internal types to the test assembly.
- Prefer NSubstitute for mocking interfaces; use `System.Reflection` for `internal sealed` types.
- Tests that require Azure credentials should be tagged with `[Trait("Category", "Integration")]` and are excluded from the default test run.
