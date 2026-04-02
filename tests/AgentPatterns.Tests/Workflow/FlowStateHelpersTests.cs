using AP.Workflow;
using Microsoft.Agents.AI.Workflows;
using NSubstitute;

namespace AgentPatterns.Tests.Workflow;

public class FlowStateHelpersTests
{
    private static IWorkflowContext CreateContext() => Substitute.For<IWorkflowContext>();

    // --- ReadFlowStateAsync ---

    [Fact]
    public async Task ReadFlowStateAsync_WhenContextReturnsNull_ReturnsNewFlowState()
    {
        var context = CreateContext();
        context.ReadStateAsync<FlowState>(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<FlowState?>((FlowState?)null));

        FlowState result = await FlowStateHelpers.ReadFlowStateAsync(context);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task ReadFlowStateAsync_WhenContextReturnsNull_DefaultIterationIsOne()
    {
        var context = CreateContext();
        context.ReadStateAsync<FlowState>(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<FlowState?>((FlowState?)null));

        FlowState result = await FlowStateHelpers.ReadFlowStateAsync(context);

        Assert.Equal(1, result.Iteration);
    }

    [Fact]
    public async Task ReadFlowStateAsync_WhenContextReturnsNull_DefaultHistoryIsEmpty()
    {
        var context = CreateContext();
        context.ReadStateAsync<FlowState>(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<FlowState?>((FlowState?)null));

        FlowState result = await FlowStateHelpers.ReadFlowStateAsync(context);

        Assert.Empty(result.History);
    }

    [Fact]
    public async Task ReadFlowStateAsync_WhenContextReturnsExistingState_ReturnsThatInstance()
    {
        var context = CreateContext();
        var existing = new FlowState { Iteration = 3 };
        context.ReadStateAsync<FlowState>(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<FlowState?>(existing));

        FlowState result = await FlowStateHelpers.ReadFlowStateAsync(context);

        Assert.Same(existing, result);
    }

    [Fact]
    public async Task ReadFlowStateAsync_WhenContextReturnsExistingState_PreservesIteration()
    {
        var context = CreateContext();
        var existing = new FlowState { Iteration = 5 };
        context.ReadStateAsync<FlowState>(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<FlowState?>(existing));

        FlowState result = await FlowStateHelpers.ReadFlowStateAsync(context);

        Assert.Equal(5, result.Iteration);
    }

    [Fact]
    public async Task ReadFlowStateAsync_CallsReadStateAsyncWithCorrectKey()
    {
        var context = CreateContext();
        context.ReadStateAsync<FlowState>(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<FlowState?>((FlowState?)null));

        await FlowStateHelpers.ReadFlowStateAsync(context);

        await context.Received(1).ReadStateAsync<FlowState>(
            FlowStateShared.Key,
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReadFlowStateAsync_CallsReadStateAsyncWithCorrectScope()
    {
        var context = CreateContext();
        context.ReadStateAsync<FlowState>(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new ValueTask<FlowState?>((FlowState?)null));

        await FlowStateHelpers.ReadFlowStateAsync(context);

        await context.Received(1).ReadStateAsync<FlowState>(
            Arg.Any<string>(),
            FlowStateShared.Scope,
            Arg.Any<CancellationToken>());
    }

    // --- SaveFlowStateAsync ---

    [Fact]
    public async Task SaveFlowStateAsync_CallsQueueStateUpdateAsync()
    {
        var context = CreateContext();
        var state = new FlowState();

        await FlowStateHelpers.SaveFlowStateAsync(context, state);

        await context.Received(1).QueueStateUpdateAsync(
            Arg.Any<string>(),
            Arg.Any<FlowState>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveFlowStateAsync_UsesCorrectKey()
    {
        var context = CreateContext();
        var state = new FlowState();

        await FlowStateHelpers.SaveFlowStateAsync(context, state);

        await context.Received(1).QueueStateUpdateAsync(
            FlowStateShared.Key,
            Arg.Any<FlowState>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveFlowStateAsync_UsesCorrectScope()
    {
        var context = CreateContext();
        var state = new FlowState();

        await FlowStateHelpers.SaveFlowStateAsync(context, state);

        await context.Received(1).QueueStateUpdateAsync(
            Arg.Any<string>(),
            Arg.Any<FlowState>(),
            FlowStateShared.Scope,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SaveFlowStateAsync_PassesTheStateObjectToContext()
    {
        var context = CreateContext();
        var state = new FlowState { Iteration = 7 };

        await FlowStateHelpers.SaveFlowStateAsync(context, state);

        await context.Received(1).QueueStateUpdateAsync(
            Arg.Any<string>(),
            Arg.Is<FlowState>(s => s.Iteration == 7),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}
