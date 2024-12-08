namespace LLMSourceGen.Generators.Groq;

internal sealed record GroqChatRequest(
    string Model,
    GroqMessage[] Messages
);
