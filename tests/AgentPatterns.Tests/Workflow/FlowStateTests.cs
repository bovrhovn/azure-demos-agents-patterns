using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AP.Workflow;
using Microsoft.Extensions.AI;

namespace AgentPatterns.Tests.Workflow;

public class FlowStateSharedTests
{
    [Fact]
    public void Scope_HasExpectedValue()
    {
        Assert.Equal("FlowStateScope", FlowStateShared.Scope);
    }

    [Fact]
    public void Key_HasExpectedValue()
    {
        Assert.Equal("singleton", FlowStateShared.Key);
    }

    [Fact]
    public void Scope_And_Key_AreDifferent()
    {
        Assert.NotEqual(FlowStateShared.Scope, FlowStateShared.Key);
    }

    [Fact]
    public void Scope_IsNonEmptyString()
    {
        Assert.False(string.IsNullOrWhiteSpace(FlowStateShared.Scope));
    }

    [Fact]
    public void Key_IsNonEmptyString()
    {
        Assert.False(string.IsNullOrWhiteSpace(FlowStateShared.Key));
    }
}

public class FlowStateTests
{
    [Fact]
    public void Iteration_DefaultsToOne()
    {
        var state = new FlowState();
        Assert.Equal(1, state.Iteration);
    }

    [Fact]
    public void History_DefaultsToEmpty()
    {
        var state = new FlowState();
        Assert.Empty(state.History);
    }

    [Fact]
    public void History_IsNotNull()
    {
        var state = new FlowState();
        Assert.NotNull(state.History);
    }

    [Fact]
    public void Iteration_CanBeUpdated()
    {
        var state = new FlowState();
        state.Iteration = 3;
        Assert.Equal(3, state.Iteration);
    }

    [Fact]
    public void History_CanAddMessage()
    {
        var state = new FlowState();
        state.History.Add(new ChatMessage(ChatRole.Assistant, "Test"));
        Assert.Single(state.History);
    }

    [Fact]
    public void History_CanAddMultipleMessages()
    {
        var state = new FlowState();
        state.History.Add(new ChatMessage(ChatRole.User, "Hello"));
        state.History.Add(new ChatMessage(ChatRole.Assistant, "World"));
        Assert.Equal(2, state.History.Count);
    }

    [Fact]
    public void History_MessageRolesArePreserved()
    {
        var state = new FlowState();
        state.History.Add(new ChatMessage(ChatRole.User, "user msg"));
        state.History.Add(new ChatMessage(ChatRole.Assistant, "assistant msg"));

        Assert.Equal(ChatRole.User, state.History[0].Role);
        Assert.Equal(ChatRole.Assistant, state.History[1].Role);
    }

    [Fact]
    public void Iteration_CanIncrementRepeatedly()
    {
        var state = new FlowState();
        for (int i = 2; i <= 5; i++)
        {
            state.Iteration = i;
            Assert.Equal(i, state.Iteration);
        }
    }

    [Fact]
    public void TwoInstances_HaveIndependentHistories()
    {
        var state1 = new FlowState();
        var state2 = new FlowState();
        state1.History.Add(new ChatMessage(ChatRole.User, "only in state1"));

        Assert.Single(state1.History);
        Assert.Empty(state2.History);
    }
}

public class CriticDecisionTests
{
    [Fact]
    public void Approved_DefaultsToFalse()
    {
        var decision = new CriticDecision();
        Assert.False(decision.Approved);
    }

    [Fact]
    public void Feedback_DefaultsToEmptyString()
    {
        var decision = new CriticDecision();
        Assert.Equal(string.Empty, decision.Feedback);
    }

    [Fact]
    public void Content_DefaultsToEmptyString()
    {
        var decision = new CriticDecision();
        Assert.Equal(string.Empty, decision.Content);
    }

    [Fact]
    public void Iteration_DefaultsToZero()
    {
        var decision = new CriticDecision();
        Assert.Equal(0, decision.Iteration);
    }

    [Fact]
    public void Approved_CanBeSetToTrue()
    {
        var decision = new CriticDecision { Approved = true };
        Assert.True(decision.Approved);
    }

    [Fact]
    public void Feedback_CanBeSet()
    {
        const string text = "Add more concrete examples.";
        var decision = new CriticDecision { Feedback = text };
        Assert.Equal(text, decision.Feedback);
    }

    [Fact]
    public void Content_CanBeSet()
    {
        const string content = "Final polished article text.";
        var decision = new CriticDecision { Content = content };
        Assert.Equal(content, decision.Content);
    }

    [Fact]
    public void Iteration_CanBeSet()
    {
        var decision = new CriticDecision { Iteration = 2 };
        Assert.Equal(2, decision.Iteration);
    }

    // --- JSON attribute tests ---

    [Fact]
    public void Approved_HasJsonPropertyName_Approved()
    {
        var prop = typeof(CriticDecision).GetProperty(nameof(CriticDecision.Approved))!;
        var attr = prop.GetCustomAttribute<JsonPropertyNameAttribute>()!;
        Assert.NotNull(attr);
        Assert.Equal("approved", attr.Name);
    }

    [Fact]
    public void Feedback_HasJsonPropertyName_Feedback()
    {
        var prop = typeof(CriticDecision).GetProperty(nameof(CriticDecision.Feedback))!;
        var attr = prop.GetCustomAttribute<JsonPropertyNameAttribute>()!;
        Assert.NotNull(attr);
        Assert.Equal("feedback", attr.Name);
    }

    [Fact]
    public void Content_HasJsonIgnoreAttribute()
    {
        var prop = typeof(CriticDecision).GetProperty(nameof(CriticDecision.Content))!;
        Assert.NotNull(prop.GetCustomAttribute<JsonIgnoreAttribute>());
    }

    [Fact]
    public void Iteration_HasJsonIgnoreAttribute()
    {
        var prop = typeof(CriticDecision).GetProperty(nameof(CriticDecision.Iteration))!;
        Assert.NotNull(prop.GetCustomAttribute<JsonIgnoreAttribute>());
    }

    // --- Serialization round-trip tests ---

    [Fact]
    public void Serialization_IncludesApprovedField()
    {
        var decision = new CriticDecision { Approved = true };
        var json = JsonSerializer.Serialize(decision);
        Assert.Contains("\"approved\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Serialization_IncludesFeedbackField()
    {
        var decision = new CriticDecision { Feedback = "Too brief." };
        var json = JsonSerializer.Serialize(decision);
        Assert.Contains("\"feedback\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Serialization_DoesNotIncludeContent()
    {
        var decision = new CriticDecision { Approved = true, Content = "secret-content-xyz" };
        var json = JsonSerializer.Serialize(decision);
        Assert.DoesNotContain("secret-content-xyz", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Serialization_DoesNotIncludeIteration()
    {
        var decision = new CriticDecision { Iteration = 99 };
        var json = JsonSerializer.Serialize(decision);
        // "99" can appear in other fields; check key absence instead
        Assert.DoesNotContain("\"iteration\"", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"Iteration\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Deserialization_SetsApprovedAndFeedback()
    {
        const string json = "{\"approved\":true,\"feedback\":\"Well done\"}";
        var decision = JsonSerializer.Deserialize<CriticDecision>(json, JsonSerializerOptions.Web)!;
        Assert.True(decision.Approved);
        Assert.Equal("Well done", decision.Feedback);
    }

    [Fact]
    public void Deserialization_ContentRemainsDefault_WhenNotInJson()
    {
        const string json = "{\"approved\":false,\"feedback\":\"Needs work\"}";
        var decision = JsonSerializer.Deserialize<CriticDecision>(json, JsonSerializerOptions.Web)!;
        Assert.Equal(string.Empty, decision.Content);
    }

    [Fact]
    public void Deserialization_IterationRemainsDefault_WhenNotInJson()
    {
        const string json = "{\"approved\":true,\"feedback\":\"\"}";
        var decision = JsonSerializer.Deserialize<CriticDecision>(json, JsonSerializerOptions.Web)!;
        Assert.Equal(0, decision.Iteration);
    }

    [Theory]
    [InlineData(true, "")]
    [InlineData(false, "Missing examples")]
    [InlineData(true, "Great work!")]
    public void RoundTrip_PreservesApprovedAndFeedback(bool approved, string feedback)
    {
        var original = new CriticDecision { Approved = approved, Feedback = feedback };
        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<CriticDecision>(json, JsonSerializerOptions.Web)!;

        Assert.Equal(approved, restored.Approved);
        Assert.Equal(feedback, restored.Feedback);
    }
}
