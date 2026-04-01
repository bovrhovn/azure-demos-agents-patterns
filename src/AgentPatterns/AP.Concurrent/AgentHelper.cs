using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AP.Concurrent;

public class AgentHelper(IChatClient client)
{
    internal ChatClientAgent GetTranslationAgent(string targetLanguage, IChatClient chatClient) =>
        new(chatClient,
            $"You are a translation assistant who only responds in {targetLanguage}. Respond to any " +
            $"input by outputting the name of the input language and then translating the input to {targetLanguage}.");

    public IEnumerable<ChatClientAgent> GetTranslationAgents() =>
        from lang in (string[])["Slovenian", "Spanish", "English"]
        select GetTranslationAgent(lang, client);
}