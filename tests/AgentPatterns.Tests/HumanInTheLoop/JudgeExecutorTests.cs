using AP.HumanInTheLoop;
using Microsoft.Agents.AI.Workflows;
using NSubstitute;

namespace AgentPatterns.Tests.HumanInTheLoop;

public class JudgeExecutorTests
{
    private static IWorkflowContext CreateContext() => Substitute.For<IWorkflowContext>();

    [Fact]
    public async Task HandleAsync_ExactMatch_YieldsOutput()
    {
        var executor = new JudgeExecutor(42);
        var context = CreateContext();

        await executor.HandleAsync(42, context);

        await context.Received(1).YieldOutputAsync(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ExactMatch_OutputContainsTargetNumber()
    {
        var executor = new JudgeExecutor(42);
        var context = CreateContext();

        await executor.HandleAsync(42, context);

        await context.Received(1).YieldOutputAsync(
            Arg.Is<object>(o => ((string)o).Contains("42", StringComparison.Ordinal)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_TooLow_SendsBelowSignal()
    {
        var executor = new JudgeExecutor(42);
        var context = CreateContext();

        await executor.HandleAsync(10, context);

        await context.Received(1).SendMessageAsync(
            Arg.Is<object>(o => (NumberSignal)o == NumberSignal.Below),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_TooHigh_SendsAboveSignal()
    {
        var executor = new JudgeExecutor(42);
        var context = CreateContext();

        await executor.HandleAsync(100, context);

        await context.Received(1).SendMessageAsync(
            Arg.Is<object>(o => (NumberSignal)o == NumberSignal.Above),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_TooLow_DoesNotYieldOutput()
    {
        var executor = new JudgeExecutor(42);
        var context = CreateContext();

        await executor.HandleAsync(10, context);

        await context.DidNotReceive().YieldOutputAsync(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_TooHigh_DoesNotYieldOutput()
    {
        var executor = new JudgeExecutor(42);
        var context = CreateContext();

        await executor.HandleAsync(100, context);

        await context.DidNotReceive().YieldOutputAsync(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ExactMatch_AfterMultipleTries_OutputContainsTryCount()
    {
        var executor = new JudgeExecutor(42);
        var context = CreateContext();

        // Three guesses before the correct one
        await executor.HandleAsync(10, context);
        await executor.HandleAsync(50, context);
        await executor.HandleAsync(30, context);
        await executor.HandleAsync(42, context);

        await context.Received(1).YieldOutputAsync(
            Arg.Is<object>(o => ((string)o).Contains("4", StringComparison.Ordinal)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_TracksTries_FirstTrySuccess()
    {
        var executor = new JudgeExecutor(7);
        var context = CreateContext();

        await executor.HandleAsync(7, context);

        await context.Received(1).YieldOutputAsync(
            Arg.Is<object>(o => ((string)o).Contains("1", StringComparison.Ordinal)),
            Arg.Any<CancellationToken>());
    }
}
