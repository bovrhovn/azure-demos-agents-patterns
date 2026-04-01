using AP.HumanInTheLoop;
using Microsoft.Agents.AI.Workflows;

namespace AgentPatterns.Tests.HumanInTheLoop;

public class WorkflowFactoryTests
{
    [Fact]
    public void BuildWorkflow_ReturnsNonNullWorkflow()
    {
        Workflow workflow = WorkflowFactory.BuildWorkflow();
        Assert.NotNull(workflow);
    }

    [Fact]
    public void NumberSignal_HasInitValue()
    {
        Assert.Equal(0, (int)NumberSignal.Init);
    }

    [Fact]
    public void NumberSignal_HasAboveValue()
    {
        _ = NumberSignal.Above;
    }

    [Fact]
    public void NumberSignal_HasBelowValue()
    {
        _ = NumberSignal.Below;
    }

    [Fact]
    public void NumberSignal_AllValuesDistinct()
    {
        var values = Enum.GetValues<NumberSignal>();
        Assert.Equal(values.Length, values.Distinct().Count());
    }

    [Fact]
    public void NumberSignal_HasThreeValues()
    {
        var values = Enum.GetValues<NumberSignal>();
        Assert.Equal(3, values.Length);
    }
}
