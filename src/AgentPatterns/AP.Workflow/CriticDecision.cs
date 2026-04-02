using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AP.Workflow;

/// <summary>
/// Structured output schema for the Critic's decision.
/// Uses JsonPropertyName and Description attributes for OpenAI's JSON schema.
/// </summary>
[Description("Critic's review decision including approval status and feedback")]
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via JSON deserialization")]
internal sealed class CriticDecision
{
    [JsonPropertyName("approved")]
    [Description("Whether the content is approved (true) or needs revision (false)")]
    public bool Approved { get; set; }

    [JsonPropertyName("feedback")]
    [Description("Specific feedback for improvements if not approved, empty if approved")]
    public string Feedback { get; set; } = "";

    // Non-JSON properties for workflow use
    [JsonIgnore]
    public string Content { get; set; } = "";

    [JsonIgnore]
    public int Iteration { get; set; }
}